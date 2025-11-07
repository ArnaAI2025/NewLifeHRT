import { ChangeDetectorRef,Component ,OnInit, AfterViewInit, ViewChild, signal} from '@angular/core';
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
import { PharmacyService } from '../pharmacy.services';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { MmddyyyyDatePipe } from '../../../shared/pipes/mmddyyyy-date.pipe';

interface Pharmacy {
  id: string;
  name: string;
  currencyName: string;
  startDate?: string;
  endDate?: string;
  selected?: boolean;
  isActive: boolean;
}

@Component({
  selector: 'app-pharmacy-view',
  templateUrl: './pharmacy-view.html',
  styleUrl: './pharmacy-view.scss',
  imports : [MatTableModule,FormsModule,MatButtonModule,MatMenuModule,MatIconModule, MatCheckboxModule,MatProgressSpinnerModule,MatInputModule, MatSortModule,MatPaginatorModule, CommonModule,MmddyyyyDatePipe],
})
export class PharmacyViewComponent implements OnInit, AfterViewInit{
  displayedColumns: string[] = ['select', 'name', 'currency', 'startDate', 'endDate','status','actions'];
  dataSource = new MatTableDataSource<Pharmacy>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pagedData: Pharmacy[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(private router: Router,private cdr: ChangeDetectorRef,private readonly pharmacyService: PharmacyService,private confirmationService: ConfirmationDialogService,private notificationService: NotificationService) {}
  async ngOnInit(): Promise<void>{
    await this.loadPharmacies();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: Pharmacy, filter: string) => {
      const value = filter.toLowerCase();

      const formattedStartDate = data.startDate ? this.formatDateToMMDDYYYY(data.startDate) : '';
      const formattedEndDate = data.endDate ? this.formatDateToMMDDYYYY(data.endDate) : '';

  return (
    data.name?.toLowerCase().includes(value) ||
    data.currencyName?.toLowerCase().includes(value) ||
    (data.isActive ? 'active' : 'inactive').includes(value) ||
    formattedStartDate.includes(value) ||
    formattedEndDate.includes(value)
  );
};

    this.dataSource.sortingDataAccessor = (item, property) => {
    switch (property) {
      case 'currency':
        return item.currencyName?.toLowerCase() || '';
      case 'status':
        return item.isActive ? 'Active' : 'Inactive';
      default:
        return (item as any)[property];
    }
  };

  this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  async loadPharmacies(): Promise<void> {
    this.isLoading.set(true);
    try {
      const responseDto = await firstValueFrom(this.pharmacyService.getAllPharmacy());
      const mappedPharmacies: Pharmacy[] = responseDto.map(dto => ({
        id: dto.id,
        name: dto.name,
        currencyName: dto.currencyName,
        startDate: dto.startDate,
        endDate: dto.endDate,
        isActive: dto.isActive,
        selected: false
      }));
      this.dataSource.data = mappedPharmacies;
      this.dataSource.filter = '';  
      this.updatePagedData();
    } catch (err) {
      console.error('Failed to load Pharmacy', err);
    } finally {
      this.isLoading.set(false);
      this.cdr.detectChanges();
    }
  }

  async performAction(action: string, pharmacy: Pharmacy): Promise<void> {

    const confirmed = await this.openConfirmationDialog(action, 1);
    if (!confirmed) return;

    const pharmacyId = pharmacy.id;
    this.isLoading.set(true);

    let requestFn: () => Promise<any>;
    let successMessage = '';

    switch (action) {
      case 'activate':
      requestFn = () => firstValueFrom(this.pharmacyService.activatePharmacies([pharmacyId]));
      successMessage = 'Pharmacy activated successfully!';
      break;
      case 'deactivate':
      requestFn = () => firstValueFrom(this.pharmacyService.deactivatePharmacies([pharmacyId]));
      successMessage = 'Pharmacy deactivated successfully!';
      break;
      case 'delete':
        requestFn = () => firstValueFrom(this.pharmacyService.deletePharmacies([pharmacyId]));
        successMessage = 'Pharmacy deleted successfully!';
        break;
      default:
        this.isLoading.set(false);
        return;
    }

    try {
      await requestFn();
      this.notificationService.showSnackBar(successMessage, 'success');
      await this.loadPharmacies();
    } catch (err) {
      console.error(`Failed to ${action} pharmacy`, err);
      this.notificationService.showSnackBar(`Failed to ${action} pharmacy.`, 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  async bulkAction(action: string): Promise<void> {
  const selectedPharmacies = this.dataSource.data.filter(p => p.selected);
  const selectedIds = selectedPharmacies.map(p => p.id);

  if (!selectedIds.length) return;

  const confirmed = await this.openConfirmationDialog(action, selectedIds.length, true);
  if (!confirmed) return;

  this.isLoading.set(true);

  let requestFn: () => Promise<any>;
  let successMessage = '';

  switch (action) {
    case 'activate':
      requestFn = () => firstValueFrom(this.pharmacyService.activatePharmacies(selectedIds));
      successMessage = `Pharmac${selectedIds.length>1?'ies':'y'} activated successfully!`;
      break;
    case 'deactivate':
      requestFn = () => firstValueFrom(this.pharmacyService.deactivatePharmacies(selectedIds));
      successMessage = `Pharmac${selectedIds.length>1?'ies':'y'} deactivated successfully!`;
      break;
    case 'delete':
      requestFn = () => firstValueFrom(this.pharmacyService.deletePharmacies(selectedIds));
      successMessage = `Pharmac${selectedIds.length>1?'ies':'y'} deleted successfully!`;
      break;
    default:
      this.isLoading.set(false);
      return;
  }

  try {
    await requestFn();
     this.notificationService.showSnackBar(successMessage, 'success');
    await this.loadPharmacies();
  } catch (err) {
    console.error(`Failed to ${action} pharmacies`, err);
    this.notificationService.showSnackBar(`Failed to ${action} pharmacies.`, 'failure');
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

navigateToAdd(): void {
  this.router.navigate(['/pharmacy/add']);
}

navigateToEdit(pharmacyId: string): void {
  this.router.navigate(['/pharmacy/edit', pharmacyId]);
}

get selectedCount(): number {
  return this.dataSource.data.filter(row => row.selected).length;
}

async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
   const productText = isBulk
    ? `(${count}) pharmac${count > 1 ? 'ies' : 'y'}`
    : 'this pharmacy';

   const data: ConfirmationDialogData = {
    title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
    message: `<p>Are you sure you want to <strong>${action}</strong> ${productText}?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const result = await this.confirmationService.openConfirmation(data).toPromise();
  return result ?? false;
}

formatDateToMMDDYYYY(dateString: string): string {
  const date = new Date(dateString);
  const mm = String(date.getMonth() + 1).padStart(2, '0');
  const dd = String(date.getDate()).padStart(2, '0');
  const yyyy = date.getFullYear();
  return `${mm}/${dd}/${yyyy}`;
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
