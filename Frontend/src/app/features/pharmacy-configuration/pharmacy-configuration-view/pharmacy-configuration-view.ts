import { AfterViewInit, Component, OnInit, signal, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { PharmacyConfigurationService } from '../pharmacy-configuration.services';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { firstValueFrom } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { ConfirmationDialogData } from '../../../shared/models/confirmation-dialog.model';

interface PharmacyConfiguration {
  id: string;
  pharmacyName: string;
  typeName: string;
  status?: string;
  modifiedOn: Date;
  selected?: boolean;
}

@Component({
  selector: 'app-pharmacy-configuration-view',
  imports: [MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule, CommonModule],
  templateUrl: './pharmacy-configuration-view.html',
  styleUrl: './pharmacy-configuration-view.scss'
})
export class PharmacyConfigurationViewComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['select', 'pharmacyName', 'typeName', 'status', 'modifiedOn', 'actions'];
  dataSource = new MatTableDataSource<PharmacyConfiguration>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pagedData: PharmacyConfiguration[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(private router: Router, private readonly pharmacyConfigurationService: PharmacyConfigurationService, private confirmationService: ConfirmationDialogService, private notificationService: NotificationService) { }

  async ngOnInit(): Promise<void> {
    await this.loadPharmacyConfigurations();
  }
  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data: PharmacyConfiguration, filter: string) => {
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
  async loadPharmacyConfigurations(): Promise<void> {
    this.isLoading.set(true);
    try {
      const responseDto = await firstValueFrom(this.pharmacyConfigurationService.getAllPharmacyConfigurations());
      const mappedPharmacyConfigurations: PharmacyConfiguration[] = responseDto.map(dto => ({
        id: dto.id,
        pharmacyName: dto.pharmacyName,
        typeName: dto.typeName,
        status: dto.status,
        modifiedOn: new Date(dto.modifiedOn),
        selected: false
      }));
      this.dataSource.data = mappedPharmacyConfigurations;
      this.dataSource.filter = '';
      this.updatePagedData();
    } catch (err) {
      console.error('Failed to load pharmacy configurations', err);
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

  async bulkAction(action: string): Promise<void> {
    const selectedPharmacyConfigurations = this.dataSource.data.filter(p => p.selected);
    const selectedIds = selectedPharmacyConfigurations.map(p => p.id);

    if (!selectedIds.length) return;

    const confirmed = await this.openConfirmationDialog(action, selectedIds.length, true);
    if (!confirmed) return;

    this.isLoading.set(true);

    let requestFn: () => Promise<any>;
    let successMessage = '';

    switch (action) {
      case 'activate':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.activatePharmacyConfigurations(selectedIds));
        successMessage = 'Pharmacy Configurations activated successfully!';
        break;
      case 'deactivate':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.deactivatePharmacyConfigurations(selectedIds));
        successMessage = 'Pharmacy Configurations deactivated successfully!';
        break;
      case 'delete':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.deletePharmacyConfigurations(selectedIds));
        successMessage = 'Pharmacy Configurations deleted successfully!';
        break;
      default:
        this.isLoading.set(false);
        return;
    }

    try {
      await requestFn();
      this.notificationService.showSnackBar(successMessage, 'success');
      await this.loadPharmacyConfigurations();
    } catch (err) {
      console.error(`Failed to ${action} pharmacy configurations`, err);
      this.notificationService.showSnackBar(`Failed to ${action} pharmacy configurations.`, 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  async performAction(action: string, pharmacyConfiguration: PharmacyConfiguration): Promise<void> {

    const confirmed = await this.openConfirmationDialog(action, 1);
    if (!confirmed) return;

    const pharmacyConfigId = pharmacyConfiguration.id;
    this.isLoading.set(true);

    let requestFn: () => Promise<any>;
    let successMessage = '';

    switch (action) {
      case 'activate':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.activatePharmacyConfigurations([pharmacyConfigId]));
        successMessage = 'Pharmacy Configurations published successfully!';
        break;
      case 'deactivate':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.deactivatePharmacyConfigurations([pharmacyConfigId]));
        successMessage = 'Pharmacy Configurations deactivated successfully!';
        break;
      case 'delete':
        requestFn = () => firstValueFrom(this.pharmacyConfigurationService.deletePharmacyConfigurations([pharmacyConfigId]));
        successMessage = 'Pharmacy Configurations deleted successfully!';
        break;
      default:
        this.isLoading.set(false);
        return;
    }

    try {
      await requestFn();
      this.notificationService.showSnackBar(successMessage, 'success');
      await this.loadPharmacyConfigurations();
    } catch (err) {
      console.error(`Failed to ${action} pharmacy configurations`, err);
      this.notificationService.showSnackBar(`Failed to ${action} pharmacy configurations.`, 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
    const pharmacyConfigurationText = isBulk
      ? `(${count}) pharmacy configuration${count > 1 ? 's' : ''}`
      : 'this pharmacy configuration';
    var actiontext = action;
    const data: ConfirmationDialogData = {
      title: `${actiontext.charAt(0).toUpperCase() + actiontext.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${actiontext}</strong> ${pharmacyConfigurationText}?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationService.openConfirmation(data).toPromise();
    return result ?? false;
  }

  navigateToAdd(): void {
    this.router.navigate(['/pharmacyconfiguration/add']);
  }

  navigateToEdit(pharmacyConfigurationId: string): void {
    this.router.navigate(['/pharmacyconfiguration/edit', pharmacyConfigurationId]);
  }
}
