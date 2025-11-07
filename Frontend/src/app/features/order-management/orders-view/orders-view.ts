import { Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { OrderService } from '../order-service';
import { OrderBulkResponseDto } from '../model/bulk-order-response.model';
import { OrderStatus } from '../../../shared/enums/order-status.enus';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';

@Component({
  selector: 'app-orders-view',
  standalone: true,
  templateUrl: './orders-view.html',
  styleUrls: ['./orders-view.scss'],
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatButtonModule, MatCheckboxModule, MatPaginatorModule,
    MatSortModule, MatMenuModule, MatTableModule, MatProgressSpinnerModule, PatientNavigationBarComponent
  ]
})
export class OrdersView implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns: string[] = [
    'select', 'name', 'patientName', 'pharmacyName', 'counselorName', 'status', 'therapyExpiration', 'actions'
  ];
  dataSource = new MatTableDataSource<OrderBulkResponseDto>([]);
  selection = new SelectionModel<OrderBulkResponseDto>(true, []);
  searchKeyword = '';
  isLoading = signal(false);
  isDeleting = signal(false);
  public OrderStatus = OrderStatus;
  activeRow: OrderBulkResponseDto | null = null;
  patientId: string | null = null;
  pagedData: OrderBulkResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private orderService: OrderService,
    private notificationService: NotificationService,
    private confirmationDialog: ConfirmationDialogService,
    private router: Router,
    private activatedRoute: ActivatedRoute

  ) { }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      this.patientId = params.get('patientId');
    });
    this.loadOrders();

  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data, filter) => {
      const val = filter.toLowerCase();
      return (
        data.name?.toLowerCase().includes(val) ||
        data.patientName?.toLowerCase().includes(val) ||
        data.pharmacyName?.toLowerCase().includes(val) ||
        data.counselorName?.toLowerCase().includes(val) ||
        (data.status ? data.status.toString().toLowerCase().includes(val) : false)
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });
    this.updatePagedData();
  }

  async loadOrders(): Promise<void> {
    this.isLoading.set(true);
    try {
      const orders = await firstValueFrom(this.orderService.getAllOrders(this.patientId ?? undefined));
      this.dataSource.data = orders || [];
      this.updatePagedData();
    } catch (error) {
      console.error('Failed to load orders:', error);
      this.notificationService.showSnackBar('Failed to load orders', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  applyFilter() {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.selection.clear();
    this.resetPaginator();
  }

  isAllSelected() {
    const selectableRows = this.dataSource.filteredData.filter(
      row => row.status !== OrderStatus.LifeFileProcessing
    );
    const numSelected = this.selection.selected.length;
    const numRows = selectableRows.length;
    return numRows > 0 && numSelected === numRows;
  }

  masterToggle() {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.dataSource.filteredData
        .filter(row => row.status !== OrderStatus.LifeFileProcessing)
        .forEach(row => this.selection.select(row));
    }
  }

  toggleRowSelection(row: OrderBulkResponseDto) {
    this.selection.toggle(row);
  }

  isSelected(row: OrderBulkResponseDto) {
    return this.selection.isSelected(row);
  }

  getSelectedCount() {
    return this.selection.selected.length;
  }

  onAddOrder(): void {
    if (this.patientId) {
      this.router.navigate(['/order/add', this.patientId]);
      return;
    }
    this.router.navigate(['/order/add']);
  }

  onEdit(row: OrderBulkResponseDto | null): void {
    if (row && row.id) {
      if (this.patientId) {
        this.router.navigate(['/order/edit', row.id, this.patientId]);
      } else {
        this.router.navigate(['/order/edit', row.id]);
      }
    }
  }


  private async confirmDialog(title: string, message: string): Promise<boolean> {
    return await firstValueFrom(
      this.confirmationDialog.openConfirmation({
        title,
        message,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
      })
    );
  }

  async deleteOrders(ids: string[]) {
    if (!ids.length) return;

    const title = ids.length > 1 ? 'Delete Selected Orders' : 'Delete Order';
    const message = `Are you sure you want to delete ${ids.length > 1 ? 'these orders' : 'this order'}? This action cannot be undone.`;

    const confirmed = await this.confirmDialog(title, message);
    if (!confirmed) return;

    this.isDeleting.set(true);
    try {
      const response = await firstValueFrom(this.orderService.deleteOrders(ids));
      this.selection.clear();
      await this.loadOrders();
      this.notificationService.showSnackBar(
        response.message || `Order${ids.length > 1 ? 's' : ''} deleted successfully`, 'success'
      );
    } catch (error) {
      console.error(`Failed to delete order${ids.length > 1 ? 's' : ''}:`, error);
      this.notificationService.showSnackBar(`Failed to delete order${ids.length > 1 ? 's' : ''}.`, 'failure');
    } finally {
      this.isDeleting.set(false);
    }
  }

  async onDelete(row: OrderBulkResponseDto | null) {
    if (row) {
      await this.deleteOrders([row.id]);
    }
  }

  async onBulkDelete() {
    const ids = this.selection.selected.map(s => s.id);
    await this.deleteOrders(ids);
  }

  isTherapyExpired(therapyExpiration?: string): boolean {
    if (!therapyExpiration) return false;
    return new Date(therapyExpiration) < new Date();
  }

  getFormattedDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  getOrderStatusClass(status: number): string {
    switch (status) {
      case OrderStatus.New:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case OrderStatus.Cancel_noMoney:
        return 'bg-yellow-100 text-yellow-800 border border-yellow-300';
      case OrderStatus.Cancel_rejected:
        return 'bg-red-100 text-red-800 border border-red-300';
      case OrderStatus.Completed:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileProcessing:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileSuccess:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileError:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
    }
  }

getOrderStatusText(status?: number): string {
  switch (status) {
    case OrderStatus.New:
      return 'New';
    case OrderStatus.Completed:
      return 'Accepted';
    case OrderStatus.Cancel_noMoney:
      return 'No Money';
    case OrderStatus.Cancel_rejected:
      return 'Rejected';
    case OrderStatus.LifeFileProcessing:
      return 'LifeFile Processing';
    case OrderStatus.LifeFileSuccess:
      return 'LifeFile Success';
    case OrderStatus.LifeFileError:
      return 'LifeFile Error';
    default:
      return 'Unknown';
  }
}

togglePatientActiveStatus(status: boolean): void {}
  onSaveAndClose(): void {}
  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }
  onSubmit(): void { }
  onClose(): void {
    this.router.navigate(['/patients/view']);
  }

   private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length? this.dataSource.filteredData: this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }
}
