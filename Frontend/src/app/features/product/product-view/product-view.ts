import { Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
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
import { ProductService } from '../product.services';
import { Observable } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { NotificationService } from '../../../shared/services/notification.service';

interface Product {
  id: string;
  name: string;
  productCode: string;
  parent?: string;
  status?: string;
  modifiedOn: Date;
  selected?: boolean;
}

@Component({
  selector: 'app-product-view',
  templateUrl: './product-view.html',
  styleUrl: './product-view.scss',
  imports : [MatTableModule,FormsModule,MatButtonModule,MatMenuModule,MatIconModule, MatCheckboxModule,MatProgressSpinnerModule,MatInputModule, MatSortModule,MatPaginatorModule, CommonModule]
})
export class ProductViewComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['select', 'name', 'productCode', 'parent', 'status', 'modifiedOn', 'actions'];
  dataSource = new MatTableDataSource<Product>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pagedData: Product[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(private router: Router,private readonly productService: ProductService,private confirmationService: ConfirmationDialogService,private notificationService: NotificationService) {}

  async ngOnInit(): Promise<void>{
    await this.loadProducts();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: Product, filter: string) => {
      const value = filter.toLowerCase();
      return Object.values(data).some(val =>
        (val || '').toString().toLowerCase().includes(value)
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  async loadProducts(): Promise<void> {
  this.isLoading.set(true);
  try {
    const responseDto = await firstValueFrom(this.productService.getAllProducts());
    const mappedProducts: Product[] = responseDto.map(dto => ({
      id: dto.id,
      name: dto.name,
      productCode: dto.productID,
      parent: dto.parentName,
      status: dto.status,
      modifiedOn: new Date(dto.modifiedOn),
      selected: false
    }));
    this.dataSource.data = mappedProducts;
    this.dataSource.filter = '';  
    this.updatePagedData();
  } catch (err) {
    console.error('Failed to load products', err);
  } finally {
    this.isLoading.set(false);
  }
}

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (!this.dataSource.filter) {
      this.dataSource.filter = ''; 
    }
    this.resetPaginator();
  }

async bulkAction(action: string): Promise<void> {
  const selectedProducts = this.dataSource.data.filter(p => p.selected);
  const selectedIds = selectedProducts.map(p => p.id);

  if (!selectedIds.length) return;

  const confirmed = await this.openConfirmationDialog(action, selectedIds.length, true);
  if (!confirmed) return;

  this.isLoading.set(true);

  let requestFn: () => Promise<any>;
  let successMessage = '';

  switch (action) {
    case 'publish':
    case 'activate':
      requestFn = () => firstValueFrom(this.productService.publishProducts(selectedIds));
      successMessage = 'Products published successfully!';
      break;
    case 'deactivate':
      requestFn = () => firstValueFrom(this.productService.deactivateProducts(selectedIds));
      successMessage = 'Products retired successfully!';
      break;
    case 'delete':
      requestFn = () => firstValueFrom(this.productService.deleteProducts(selectedIds));
      successMessage = 'Products deleted successfully!';
      break;
    default:
      this.isLoading.set(false);
      return;
  }

  try {
    await requestFn();
    this.notificationService.showSnackBar(successMessage, 'success');
    await this.loadProducts();
  } catch (err) {
    console.error(`Failed to ${action} products`, err);
    this.notificationService.showSnackBar(`Failed to ${action} products.`, 'failure');
  } finally {
    this.isLoading.set(false);
  }
}

toggleSelectAll(event: any): void {
  const isChecked = event.checked;
  this.dataSource.filteredData.forEach(row => {
    row.selected = isChecked;
  });
}

isAllSelected(): boolean {
  return (
    this.dataSource.filteredData.length > 0 &&
    this.dataSource.filteredData.every(row => row.selected)
  );
}

get selectedCount(): number {
  return this.dataSource.data?.filter(row => row.selected).length ?? 0;
}

isPartialSelected(): boolean {
  const selectedCount = this.dataSource.filteredData.filter(row => row.selected).length;
  return selectedCount > 0 && selectedCount < this.dataSource.filteredData.length;
}

hasSelectedRows(): boolean {
  return this.dataSource.data.some(row => row.selected);
}

async performAction(action: string, product: Product): Promise<void> {

  const confirmed = await this.openConfirmationDialog(action, 1);
  if (!confirmed) return;

  const productId = product.id;
  this.isLoading.set(true);

  let requestFn: () => Promise<any>;
  let successMessage = '';

  switch (action) {
    case 'publish':
    case 'activate':
      requestFn = () => firstValueFrom(this.productService.publishProducts([productId]));
      successMessage = 'Product published successfully!';
      break;
    case 'deactivate':
      requestFn = () => firstValueFrom(this.productService.deactivateProducts([productId]));
      successMessage = 'Product retired successfully!';
      break;
    case 'delete':
      requestFn = () => firstValueFrom(this.productService.deleteProducts([productId]));
      successMessage = 'Product deleted successfully!';
      break;
    default:
      this.isLoading.set(false);
      return;
  }

  try {
    await requestFn();
    this.notificationService.showSnackBar(successMessage, 'success');
    await this.loadProducts();
  } catch (err) {
    console.error(`Failed to ${action} product`, err);
    this.notificationService.showSnackBar(`Failed to ${action} product.`, 'failure');
  } finally {
    this.isLoading.set(false);
  }
}


navigateToAdd(): void {
  this.router.navigate(['/product/add']);
}

navigateToEdit(productId: string): void {
  this.router.navigate(['/product/edit', productId]);
}

async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
   const productText = isBulk
    ? `(${count}) product${count > 1 ? 's' : ''}`
    : 'this product';
    var actiontext = action == 'deactivate' ? 'retire' : action;
   const data: ConfirmationDialogData = {
    title: `${actiontext.charAt(0).toUpperCase() + actiontext.slice(1)} Confirmation`,
    message: `<p>Are you sure you want to <strong>${actiontext}</strong> ${productText}?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const result = await this.confirmationService.openConfirmation(data).toPromise();
  return result ?? false;
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
