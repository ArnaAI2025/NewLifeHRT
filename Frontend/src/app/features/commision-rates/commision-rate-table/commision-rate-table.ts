import { ChangeDetectorRef, Component, OnInit, AfterViewInit, ViewChild, Input, signal } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { CommisionRateService } from '../commision-rate.services';

interface CommisionRate {
  id: string;
  productId?: string;
  productName?: string;
  fromAmount: number;
  toAmount: number;
  ratePercentage?: number;
  selected?: boolean;
  status: string;
}

@Component({
  selector: 'app-commision-rate-table',
  imports: [MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule, CommonModule],
  templateUrl: './commision-rate-table.html',
  styleUrl: './commision-rate-table.scss'
})
export class CommisionRateTableComponent implements OnInit, AfterViewInit {
  @Input() context: 'standalone' | 'product' = 'standalone';
  @Input() entityId?: string;

  dataSource = new MatTableDataSource<CommisionRate>([]);
  displayedColumns: string[] = [];
  showProductColumn = false;
  isLoading = signal(false);
  searchKeyword = '';
  pagedData: CommisionRate[] = [];
  pageSize = 5;
  pageIndex = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private commisionRateService: CommisionRateService,
    private router: Router,
    private confirmationDialog: ConfirmationDialogService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.configureColumns();
    this.loadData();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: CommisionRate, filter: string) => {
      const value = filter.toLowerCase();
      return !!(
        data.fromAmount?.toString().toLowerCase().includes(value) ||
        data.ratePercentage?.toString().toLowerCase().includes(value) ||
        data.toAmount?.toString().toLowerCase().includes(value) ||
        data.status?.toLowerCase().includes(value) ||
        data.productName?.toLowerCase().includes(value)
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  private configureColumns() {
    switch (this.context) {
      case 'standalone':
        this.displayedColumns = ['select', 'productName', 'fromAmount', 'ratePercentage', 'toAmount', 'status', 'actions'];
        this.showProductColumn = true;
        break;
      case 'product':
        this.displayedColumns = ['select', 'fromAmount', 'ratePercentage', 'toAmount', 'status', 'actions'];
        break;
    }
  }

  private async loadData(): Promise<void> {
    this.isLoading.set(true);

    let apiCall$: Observable<any>;

    switch (this.context) {
      case 'product':
        if (!this.entityId) return;
        apiCall$ = this.commisionRateService.getAllCommisionRatesByProductId(this.entityId);
        break;
      default:
        apiCall$ = this.commisionRateService.getAllCommisionRates();
    }

    try {
      const items = await firstValueFrom(apiCall$);
      this.dataSource.data = items.map((item: any) => ({
        ...item,
        selected: false
      }));
      this.dataSource.filter = '';  
      this.updatePagedData();
    } catch (err) {
      console.error('Failed to load commission rates', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  getProductName(item: CommisionRate): string {
    return (item as any).productName || '';
  }


  getProductId(item: CommisionRate): string | undefined {
    return (item as any).productId;
  }


  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
    if (!this.dataSource.filter) {
      this.dataSource.filter = ''; 
    }
    this.resetPaginator();
  }

  toggleSelectAll(event: any): void {
    debugger;
    const isChecked = event.checked;
    this.dataSource.filteredData.forEach(row => {
      row.selected = isChecked;
    });
  }

  isAllSelected(): boolean {
    return this.dataSource.filteredData.length > 0 && this.dataSource.filteredData.every(row => row.selected);
  }

  isPartialSelected(): boolean {
    const selectedCount = this.dataSource.filteredData.filter(row => row.selected).length;
    return selectedCount > 0 && selectedCount < this.dataSource.filteredData.length;
  }

  hasSelectedRows(): boolean {
    return this.dataSource.data.some(row => row.selected);
  }

  get selectedCount(): number {
    return this.dataSource.data.filter(row => row.selected).length;
  }

  async handleAction(action: 'activate' | 'deactivate' | 'delete', isBulk: boolean, itemId?: string): Promise<void> {
    const selectedIds = isBulk
      ? this.dataSource.data.filter(row => row.selected).map(row => row.id)
      : itemId
        ? [itemId]
        : [];

    if (selectedIds.length === 0) return;

    const confirmed = await this.openConfirmationDialog(action, selectedIds.length, isBulk);
    if (!confirmed) return;

    this.isLoading.set(true);

    let apiCall$: Observable<any>;

    switch (action) {
      case 'activate':
        apiCall$ = this.commisionRateService.activateCommisionRates(selectedIds);
        break;
      case 'deactivate':
        apiCall$ = this.commisionRateService.deactivateCommisionRates(selectedIds);
        break;
      case 'delete':
        apiCall$ = this.commisionRateService.deleteCommisionRates(selectedIds);
        break;
      default:
        this.isLoading.set(false);
        return;
    }

    try {
      await firstValueFrom(apiCall$);
      const label =
        selectedIds.length > 1 ? 'Commission Rates' : 'Commission Rate';
      const actionMap: Record<typeof action, string> = {
        activate: 'activated',
        deactivate: 'deactivated',
        delete: 'deleted'
      };
      this.notificationService.showSnackBar(
        `${label} ${actionMap[action]} successfully.`,
        'success'
      );
      await this.loadData();
    } catch (err) {
      console.error(`${action} failed`, err);
      this.notificationService.showSnackBar(`${action.charAt(0).toUpperCase() + action.slice(1)} failed. Please try again.`,'failure');
      this.isLoading.set(false);
    }
  }

  async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
    const commisionRateText = isBulk
      ? `(${count}) Commission Rate${count > 1 ? 's' : ''}`
      : 'this Commission rate';

    const data: ConfirmationDialogData = {
      title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action}</strong> ${commisionRateText}?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationDialog.openConfirmation(data).toPromise();
    return result ?? false;
  }

  editCommisionRate(id: string) {
    if (this.context !== 'standalone' && this.entityId) {
      this.router.navigate(['/commissionrate/edit', id], {
        queryParams: {
          context: this.context,
          entityId: this.entityId
        }
      });
    } else {
      this.router.navigate(['/commissionrate/edit', id]);
    }
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
