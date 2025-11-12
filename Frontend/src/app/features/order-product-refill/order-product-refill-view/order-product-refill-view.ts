import { ChangeDetectorRef, Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
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
import { OrderProductRefillAllResponseModel } from '../model/order-products-refill-all-response.model';
import { OrderproductRefillService } from '../order-product-refill.services';

interface OrderProductRefill {
  id: string;
  orderId: string;
  orderName: string;
  createdAt: string;
  productId: string;
  productName: string;
  protocol: string;
  quantity: number;
  orderFulfilledDate?: string | null;
  productRefillDate?: string | null;
  selected?: boolean;
}

@Component({
  selector: 'app-order-product-refill-view',
  imports: [MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule, CommonModule],
  templateUrl: './order-product-refill-view.html',
  styleUrl: './order-product-refill-view.scss'
})

export class OrderProductRefillViewComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['select', 'orderName', 'createdAt', 'productName', 'protocol', 'quantity', 'orderFulfilledDate', 'productRefillDate', 'actions'];
  dataSource = new MatTableDataSource<OrderProductRefill>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pagedData: OrderProductRefill[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(private router: Router, private cdr: ChangeDetectorRef, private orderProductsRefillService: OrderproductRefillService, private confirmationService: ConfirmationDialogService, private notificationService: NotificationService) { }
  async ngOnInit(): Promise<void> {
    await this.loadOrderProductsRefillDetails();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: OrderProductRefill, filter: string) => {
      const value = filter.toLowerCase();
      return Object.values(data).some(val =>
        (val || '').toString().toLowerCase().includes(value)
      );
    }

    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  async loadOrderProductsRefillDetails(): Promise<void> {
    this.isLoading.set(true);
    try {
      const responseDto = await firstValueFrom(this.orderProductsRefillService.getAllOrderProductsRefillDetails());
      const mappedOrderProductsRefillDetail: OrderProductRefill[] = responseDto.map(dto => ({
        id: dto.id,
        orderId: dto.orderId,
        orderName: dto.orderName,
        createdAt: dto.createdAt,
        productId: dto.productId,
        productName: dto.productName,
        protocol: dto.protocol,
        quantity: dto.quantity,
        orderFulfilledDate: dto.orderFulfilledDate,
        productRefillDate: dto.productRefillDate,
        selected: false
      }));
      this.dataSource.data = mappedOrderProductsRefillDetail;
      this.dataSource.filter = '';
      this.updatePagedData();
    } catch (err) {
      console.error('Failed to load Pharmacy', err);
    } finally {
      this.isLoading.set(false);
      this.cdr.detectChanges();
    }
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (!this.dataSource.filter) {
      this.dataSource.filter = '';
    }
    this.resetPaginator();
  }

  toggleSelectAll(event: any): void {
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

  navigateToOrderEdit(orderId: string): void {
    this.router.navigate(['/order/edit', orderId]);
  }

  navigateToProductEdit(productId: string): void {
    this.router.navigate(['/product/edit', productId]);
  }

  get selectedCount(): number {
    return this.dataSource.data.filter(row => row.selected).length;
  }

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length ? this.dataSource.filteredData : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }

  async onDeleteSingle(refillId: string): Promise<void> {
    const confirmed = await firstValueFrom(this.confirmationService.openConfirmation({
      title: 'Confirm Deletion',
      message: 'Are you sure you want to delete this Order Product Refill record?',
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel'
    }));

    if (!confirmed) return;

    this.isLoading.set(true);
    try {
      await firstValueFrom(this.orderProductsRefillService.deleteOrderProductsRefill([refillId]));
      this.notificationService.showSnackBar('Record deleted successfully.', 'success');
      await this.loadOrderProductsRefillDetails();
    } catch (error) {
      console.error(error);
      this.notificationService.showSnackBar('Failed to delete record.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  async onBulkDelete(): Promise<void> {
    const selectedIds = this.dataSource.data
      .filter(x => x.selected)
      .map(x => x.id);

    if (!selectedIds.length) return;

    const confirmed = await firstValueFrom(this.confirmationService.openConfirmation({
      title: 'Confirm Bulk Delete',
      message: `Are you sure you want to delete ${selectedIds.length} record(s)?`,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel'
    }));

    if (!confirmed) return;

    this.isLoading.set(true);
    try {
      await firstValueFrom(this.orderProductsRefillService.deleteOrderProductsRefill(selectedIds));
      this.notificationService.showSnackBar(`${selectedIds.length} record(s) deleted successfully.`, 'success');
      await this.loadOrderProductsRefillDetails();
    } catch (error) {
      console.error(error);
      this.notificationService.showSnackBar('Failed to delete selected records.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  navigateToOrderProductRefillDetailEdit(id:string)
  {
    this.router.navigate(['/order-product-refill/edit', id]);
  }



}
