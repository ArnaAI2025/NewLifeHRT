import { ChangeDetectorRef,Component ,OnInit, AfterViewInit, ViewChild, Input, signal } from '@angular/core';
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
import { PriceListItemService } from '../price-list-item.services';

interface PriceListItem {
  id: string;
  productId?: string;
  productName?: string;
  pharmacyId?: string;
  pharmacyName?: string;
  amount: number;
  selected?: boolean;
  status: string;
}

@Component({
  selector: 'app-price-list-item-table',
  imports: [MatTableModule,FormsModule,MatButtonModule,MatMenuModule,MatIconModule, MatCheckboxModule,MatProgressSpinnerModule,MatInputModule, MatSortModule,MatPaginatorModule, CommonModule],
  templateUrl: './price-list-item-table.html',
  styleUrl: './price-list-item-table.scss'
})
export class PriceListItemTableComponent implements OnInit, AfterViewInit {
@Input() context: 'standalone' | 'product' | 'pharmacy' = 'standalone';
@Input() entityId?: string;

dataSource = new MatTableDataSource<PriceListItem>([]);
displayedColumns: string[] = [];
showProductColumn = false;
showPharmacyColumn = false;
isLoading = signal(false);
searchKeyword = '';
pagedData: PriceListItem[] = [];
pageSize = 5;
pageIndex = 0;

@ViewChild(MatPaginator) paginator!: MatPaginator;
@ViewChild(MatSort) sort!: MatSort;

constructor(
    private priceListItemService: PriceListItemService,
    private router: Router,
    private confirmationDialog: ConfirmationDialogService,
    private notificationService: NotificationService
  ) {}

 ngOnInit() {
    this.configureColumns();
    this.loadData();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: PriceListItem, filter: string) => {
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


 private configureColumns() {
    switch (this.context) {
      case 'standalone':
        this.displayedColumns = ['select','productName', 'pharmacyName', 'amount', 'status','actions'];
        this.showProductColumn = true;
        this.showPharmacyColumn = true;
        break;
      case 'product':
        this.displayedColumns = ['select','pharmacyName', 'amount', 'status','actions'];
        this.showPharmacyColumn = true;
        break;
      case 'pharmacy':
        this.displayedColumns = ['select','productName', 'amount', 'status','actions'];
        this.showProductColumn = true;
        break;
    }
  }

private async loadData(): Promise<void> {
  this.isLoading.set(true);

  let apiCall$: Observable<any>;

  switch (this.context) {
    case 'product':
      if (!this.entityId) return;
      apiCall$ = this.priceListItemService.getAllPriceListItemsByProductId(this.entityId);
      break;
    case 'pharmacy':
      if (!this.entityId) return;
      apiCall$ = this.priceListItemService.getAllPriceListItemsByPharmacyId(this.entityId);
      break;
    default:
      apiCall$ = this.priceListItemService.getAllPriceListItems();
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
    console.error('Failed to load price list items', err);
  } finally {
    this.isLoading.set(false);
  }
}

  getProductName(item: PriceListItem): string {
    return (item as any).productName || '';
  }

  getPharmacyName(item: PriceListItem): string {
    return (item as any).pharmacyName || '';
  }

  getProductId(item: PriceListItem): string | undefined {
    return (item as any).productId;
  }

  getPharmacyId(item: PriceListItem): string | undefined {
    return (item as any).pharmacyId;
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
      apiCall$ = this.priceListItemService.activatePriceListItems(selectedIds);
      break;
    case 'deactivate':
      apiCall$ = this.priceListItemService.deactivatePriceListItems(selectedIds);
      break;
    case 'delete':
      apiCall$ = this.priceListItemService.deletePriceListItems(selectedIds);
      break;
    default:
      this.isLoading.set(false);
      return;
  }

  try {
    await firstValueFrom(apiCall$);
    this.notificationService.showSnackBar(`Price List Items ${action.charAt(0).toUpperCase() + action.slice(1)} successful.`,'success');
    await this.loadData(); // make loadData also async
  } catch (err) {
    console.error(`${action} failed`, err);
    this.notificationService.showSnackBar(`${action.charAt(0).toUpperCase() + action.slice(1)} failed. Please try again.`,'failure');
    this.isLoading.set(false);
  }
}

async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
   const productText = isBulk
    ? `(${count}) PriceListItem${count > 1 ? 's' : ''}`
    : 'this price list item';

   const data: ConfirmationDialogData = {
    title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
    message: `<p>Are you sure you want to <strong>${action}</strong> ${productText}?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const result = await this.confirmationDialog.openConfirmation(data).toPromise();
  return result ?? false;
}

editPriceListItem(id: string) {
  if (this.context !== 'standalone' && this.entityId) {
    this.router.navigate(['/pricelistitem/edit', id], {
      queryParams: {
        context: this.context,
        entityId: this.entityId
      }
    });
  } else {
    this.router.navigate(['/pricelistitem/edit', id]);
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
