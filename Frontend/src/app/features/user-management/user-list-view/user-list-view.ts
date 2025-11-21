import { Component, OnInit, ViewChild, AfterViewInit, Input, inject, signal, computed } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { UserManagementFormComponent } from '../user-management-form/user-management-form';
import { UserRole } from '../../../shared/enums/user-role.enum';
import { UserManagementService } from '../user-management.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { UserDisplay } from '../model/user-display';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { Router } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { SelectionModel } from '@angular/cdk/collections';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserResponseDto } from '../model/user-response.model';
import { createUserResponseDto } from '../model/create-user-response.model';
import { firstValueFrom } from 'rxjs';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';

@Component({
  selector: 'app-user-list-view',
  standalone: true,
  templateUrl: './user-list-view.html',
  styleUrl: './user-list-view.scss',
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatMenuModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCheckboxModule
  ]
})
export class UserListViewComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  @Input() userRoleTypeId!: number;
  public UserRole = UserRole;

  displayedColumns: string[] = ['select', 'fullName', 'userName', 'email', 'mobileNumber', 'country', 'roles', 'status', 'actions'];

  dataSource = new MatTableDataSource<UserDisplay>();
  searchKeyword = '';
  userId!: number;
  isLoading = signal(false);
  isDeleting = signal(false);
  isBulkProcessing = signal(false);
  bulkAction: 'activate' | 'deactivate' | null = null;
  selection = new SelectionModel<UserDisplay>(true, []);
  selectedCount = signal(0);
  pagedData: UserDisplay[] = [];
  pageSize = 5;
  pageIndex = 0;
  constructor(
    private router: Router,
    private readonly userManagementService: UserManagementService,
    private readonly userAccountService: UserAccountService,
    private readonly notificationService: NotificationService,
    private readonly confirmationDialogService:ConfirmationDialogService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.userAccountAsync();
    await this.loadAdminDataAsync();
    this.selection.changed.subscribe(() => {
      this.selectedCount.set(this.selection.selected.length);
    });
  }

  async userAccountAsync(): Promise<void> {
    const user = this.userAccountService.getUserAccount();
    if (!user) {
      await this.router.navigate(['/login']);
      return;
    }
  }
  
  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.sortingDataAccessor = this.customSortingDataAccessor;

    this.dataSource.filterPredicate = (data: UserDisplay, filter: string): boolean => {
      const value = filter.toLowerCase();
      return (
        data.fullName.toLowerCase().includes(value) ||
        data.userName.toLowerCase().includes(value) ||
        data.email.toLowerCase().includes(value) ||
        data.mobileNumber.toLowerCase().includes(value) ||
        data.country.toLowerCase().includes(value) ||
        data.roles.some((role) => role.toLowerCase().includes(value))
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }
    customSortingDataAccessor = (item: UserDisplay, property: string): string | number => {
      switch (property) {
      case 'status':
        return item.isDeleted ? 'Inactive' : 'Active';
      case 'roles':
        return item.roles.join(', ');
      default:
        return (item as any)[property];
      }
    }

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (!this.dataSource.filter) {
      this.dataSource.filter = ''; 
    }
    this.resetPaginator();
  }

  // Checkbox selection methods
  isAllSelected(): boolean {
  const filteredRows = this.dataSource.filteredData;
    return (
      filteredRows.length > 0 &&
      filteredRows.every(row => this.selection.isSelected(row))
    );
  }


  masterToggle(): void {
    if (this.isAllSelected()) {
      this.dataSource.filteredData.forEach(row => this.selection.deselect(row));
    } else {
      this.dataSource.filteredData.forEach(row => this.selection.select(row));
    }
  }

  toggleRowSelection(row: UserDisplay): void {
    this.selection.toggle(row);
  }

  isSelected(row: UserDisplay): boolean {
    return this.selection.isSelected(row);
  }

  formatAdminFromApi(data: any[]): UserDisplay[] {
    return data.map(user => ({
      id: user.id,
      fullName: `${user.firstName} ${user.lastName}`,
      userName: user.userName,
      email: user.email,
      mobileNumber: user.phoneNumber,
      country: user.address?.country || '—',
      isDeleted: user.isDeleted || false,
      roles: this.mapRoleIdsToNames(user.roleIds)
    }));
  }

  private mapRoleIdsToNames(roleIds: number[] | undefined): string[] {
    if (!Array.isArray(roleIds)) {
      return [];
    }

    const roleNames = roleIds
      .map((roleId) => {
        const roleName = (UserRole as Record<number, string>)[roleId];
        return typeof roleName === 'string' ? this.pascalToSpace(roleName) : '';
      })
      .filter((roleName) => !!roleName);

    return Array.from(new Set(roleNames));
  }

async loadAdminDataAsync(): Promise<void> {
  this.isLoading.set(true);
  this.selection.clear();
  try {
    const apiResponse = await firstValueFrom(
      this.userManagementService.getAllUsers([this.userRoleTypeId])
    );
    this.dataSource.data = this.formatAdminFromApi(apiResponse);
    this.dataSource.filter = '';  
    this.updatePagedData();
  } catch (err) {
    this.notificationService.showSnackBar('Failed to load users.', 'failure');
  } finally {
    this.isLoading.set(false);
    this.isBulkProcessing.set(false);
    this.bulkAction = null;
  }
}


clearSelection(): void {
  this.selection.clear();
  // Optionally reset bulk action state when clearing selection
  if (!this.isBulkProcessing()) {
    this.bulkAction = null;
  }
}


  onEdit(request: UserDisplay): void {
    this.userId = request.id;
    this.router.navigate(['/edit', this.UserRole[this.userRoleTypeId], this.userId]);
  }

  showForm(show: boolean, id: number | 0): void {
    this.userId = id;
    this.router.navigate(['/add', this.UserRole[this.userRoleTypeId]]);
  }

  handleBack(): void {
    this.loadAdminDataAsync();
  }

  pascalToSpace(str: string): string {
  return str
    .replace(/([A-Z])/g, ' $1')
    .trim();
  }


  async bulkActionAsync(action: string): Promise<void> {
  const selectedIds = this.selection.selected.map(user => user.id);
  if (!selectedIds.length) return;
  
  const confirmed = await this.openConfirmationDialog(action, selectedIds.length, true);
  if (!confirmed) return;
  
  this.isLoading.set(true);
  
  let requestFn: () => Promise<any>;
  let successMessage = '';
  
  var isAction = action === 'activate';
  switch (action) {
    case 'delete':
      requestFn = () => firstValueFrom(this.userManagementService.bulkDelete(selectedIds));
      successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) deleted successfully!`;
      break;
    case 'activate':
      requestFn = () => firstValueFrom(this.userManagementService.bulkToggleActive(selectedIds,isAction));
      successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) activated successfully!`;
      break;
    case 'deactivate':
      requestFn = () => firstValueFrom(this.userManagementService.bulkToggleActive(selectedIds,isAction));
      successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) deactivated successfully!`;
      break;
    default:
      this.isLoading.set(false);
      return;
  }

  try {
    await requestFn();
     this.notificationService.showSnackBar(successMessage, 'success');
    await this.loadAdminDataAsync();
  } catch (err) {
    console.error(`Failed to ${action} ${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s)`, err);
    this.notificationService.showSnackBar(`Failed to ${action} ${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s).`, 'failure');
  } finally {
    this.isLoading.set(false);
  }
 }

  async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
     const userText = isBulk
      ? `(${count}) ${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}${count > 1 ? 's' : ''}`
      : `this ${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}`;

     const data: ConfirmationDialogData = {
      title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action}</strong> ${userText}?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationDialogService.openConfirmation(data).toPromise();
    return result ?? false;
  }

  async performAction(action: string, user: any): Promise<void> {
  
    const confirmed = await this.openConfirmationDialog(action, 1);
    if (!confirmed) return;
  
    const userId = user.id;
    this.isLoading.set(true);
  
    let requestFn: () => Promise<any>;
    let successMessage = '';
  
    var isAction = action === 'activate';
    switch (action) {
      case 'activate':
        requestFn = () => firstValueFrom(this.userManagementService.bulkToggleActive([userId],isAction));
        successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) activated successfully!`;
      break;
      case 'deactivate':
        requestFn = () => firstValueFrom(this.userManagementService.bulkToggleActive([userId],isAction));
        successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) deactivated successfully!`;
      break;
      case 'delete':
        requestFn = () => firstValueFrom(this.userManagementService.bulkDelete([userId]));
        successMessage = `${this.pascalToSpace(this.UserRole[this.userRoleTypeId])}(s) deleted successfully!`;
      break;
      default:
      this.isLoading.set(false);
      return;
    }
  
    try {
      await requestFn();
      this.notificationService.showSnackBar(successMessage, 'success');
      await this.loadAdminDataAsync();
    } catch (err) {
      this.notificationService.showSnackBar(
        `Failed to ${action} ${this.pascalToSpace(
          this.UserRole[this.userRoleTypeId]
        )}(s).`,
        'failure'
      );
    } finally {
      this.isLoading.set(false);
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
