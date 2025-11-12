import { Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { ShippingAddressResponseDto } from '../model/shipping-address-response.model';
import { PatientService } from '../patient.services';
import { ActivatedRoute } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { ShippingAddressAddComponent } from '../shipping-address-add/shipping-address-add';

@Component({
  selector: 'app-shipping-address-view',
  standalone: true,
  templateUrl: './shipping-address-view.html',
  styleUrls: ['./shipping-address-view.scss'],
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatButtonModule, MatCheckboxModule, MatPaginatorModule,
    MatSortModule, MatMenuModule, MatTableModule, MatProgressSpinnerModule,
    PatientNavigationBarComponent,
    ShippingAddressAddComponent 
  ]
})
export class ShippingAddressViewComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  patientId: string | null = null;
  displayedColumns: string[] = ['select', 'addressLine1', 'city', 'postalCode', 'country', 'isActive', 'actions'];
  dataSource = new MatTableDataSource<ShippingAddressResponseDto>([]);
  selection = new SelectionModel<ShippingAddressResponseDto>(true, []);
  searchKeyword = '';
  isLoading = signal(false);
  isDeleting = false;

  activeRow: ShippingAddressResponseDto | null = null;

  // NEW - Modal properties
  showAddressModal = false;
  editingAddressId?: string;

  constructor(
    private patientService: PatientService,
    private notificationService: NotificationService,
    private confirmationDialog: ConfirmationDialogService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.patientId = this.activatedRoute.snapshot.paramMap.get('patientId');
    if (this.patientId) {
      this.loadAddresses();
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data, filter) => {
      const val = filter.toLowerCase();
      return (data.addressLine1?.toLowerCase().includes(val) ||
              data.city?.toLowerCase().includes(val) ||
              data.postalCode?.toLowerCase().includes(val) ||
              data.country?.toLowerCase().includes(val) ||
              (data.isActive ? 'active' : 'inactive').includes(val));
    };
  }

  async loadAddresses(): Promise<void> {
    this.isLoading.set(true);
    try {
      const addresses = await firstValueFrom(this.patientService.getAllAddressBasedOnPatientId(this.patientId!));
      this.dataSource.data = addresses || [];
    } catch {
      this.notificationService.showSnackBar('Failed to load addresses', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  applyFilter() {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.selection.clear();
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.filteredData.length;
    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ? this.selection.clear() : this.dataSource.filteredData.forEach(row => this.selection.select(row));
  }

  toggleRowSelection(row: ShippingAddressResponseDto) {
    this.selection.toggle(row);
  }

  isSelected(row: ShippingAddressResponseDto) {
    return this.selection.isSelected(row);
  }

  getSelectedCount() {
    return this.selection.selected.length;
  }

  onAddAddress() {
    if (this.patientId) {
      this.editingAddressId = undefined; 
      this.showAddressModal = true;
      
    }
  }

  onEdit(row: ShippingAddressResponseDto | null) {
    if (this.patientId && row && row.id) {
      this.editingAddressId = row.id; 
      this.showAddressModal = true;
    }
  }

  onAddressCreated(newAddress: any) {
    console.log('New address created:', newAddress);
    this.showAddressModal = false;
    this.editingAddressId = undefined;
    this.loadAddresses(); // Refresh the list
  }

  onAddressUpdated(updatedAddress: any) {
    console.log('Address updated:', updatedAddress);
    this.showAddressModal = false;
    this.editingAddressId = undefined;
    this.loadAddresses(); // Refresh the list
  }

  onAddressModalClosed() {
    this.showAddressModal = false;
    this.editingAddressId = undefined;
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

  async toggleActive(ids: string[], active: boolean) {
    if (!ids.length) return;
    const action = active ? 'activate' : 'deactivate';
    const title = ids.length > 1 ? `${action.charAt(0).toUpperCase() + action.slice(1)} Addresses` : `Activate Address`;
    const message = `Are you sure you want to ${action} ${ids.length > 1 ? 'these addresses' : 'this address'}?`;

    const confirmed = await this.confirmDialog(title, message);
    if (!confirmed) return;

    this.isLoading.set(true);
    try {
      await firstValueFrom(this.patientService.bulkToggleShippingAddressActive(ids, active));
      if (ids.length === 1) {
        const addr = this.dataSource.data.find(d => d.id === ids[0]);
        if (addr) addr.isActive = active;
      } 
      this.selection.clear();
      this.notificationService.showSnackBar(`Address${ids.length > 1 ? 'es' : ''} ${active ? 'activated' : 'deactivated'} successfully`, 'success');
    } catch {
      this.notificationService.showSnackBar(`Failed to ${action} address${ids.length > 1 ? 'es' : ''}.`, 'failure');
    } finally {
      await this.loadAddresses();
      this.isLoading.set(false);
    }
  }

  async deleteAddresses(ids: string[]) {
    if (!ids.length) return;
    const title = ids.length > 1 ? 'Delete Selected Addresses' : 'Delete Address';
    const message = `Are you sure you want to delete ${ids.length > 1 ? 'these shipping addresses' : 'this shipping address'}?`;

    const confirmed = await this.confirmDialog(title, message);
    if (!confirmed) return;

    this.isDeleting = true;
    try {      
      await firstValueFrom(this.patientService.bulkDeleteShippingAddress(ids));      
      this.selection.clear();
      await this.loadAddresses();
      this.notificationService.showSnackBar(`Address${ids.length > 1 ? 'es' : ''} deleted successfully`, 'success');
    } catch {
      this.notificationService.showSnackBar(`Failed to delete address${ids.length > 1 ? 'es' : ''}.`, 'failure');
    } finally {
      this.isDeleting = false;
    }
  }

  async onToggleActive(row: ShippingAddressResponseDto | null, active: boolean) {
    if(row)
    await this.toggleActive([row.id], active);
  }

  async onBulkToggleActive(active: boolean) {
    const ids = this.selection.selected.map(s => s.id);
    await this.toggleActive(ids, active);
  }

  async onDelete(row: ShippingAddressResponseDto | null) {
    if(row)
    await this.deleteAddresses([row.id]);
  }

  async onBulkDelete() {
    const ids = this.selection.selected.map(s => s.id);
    await this.deleteAddresses(ids);
  }
  togglePatientActiveStatus(status: boolean): void {}
  onSaveAndClose(): void {}
  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }
  onSubmit(): void {}
  onClose(): void {
    this.router.navigate(['/patients/view']);
  }
}
