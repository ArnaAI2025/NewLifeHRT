import { Component, OnInit, ViewChild, AfterViewInit, Input, signal } from '@angular/core';
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
import { PatientAddComponent } from '../patient-add/patient-add';
import { UserRole } from '../../../shared/enums/user-role.enum';
import { PatientService } from '../patient.services';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { UserDisplay } from '../../../features/user-management/model/user-display';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { Router } from '@angular/router';
import { PatientResponseDto } from '../model/patient-response.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { inject, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PatientDisplay } from '../model/patient-display.model';
import { MatCheckboxModule } from '@angular/material/checkbox'; 
import { SelectionModel } from '@angular/cdk/collections'; 
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { firstValueFrom } from 'rxjs';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { MatDialog } from '@angular/material/dialog';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { UserAssigneeComponent } from '../../../shared/components/user-assignee/user-assignee';

@Component({
  selector: 'app-patient-list',
  templateUrl: './patient-view.html',
  styleUrls: ['./patient-view.scss'],
  standalone: true,
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
    MatCheckboxModule,
  ]
})
export class PatientViewComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  private destroyRef = inject(DestroyRef);
  @Input() userRoleTypeId!: number;

  public UserRole = UserRole;

  // Updated displayedColumns to include checkbox
  displayedColumns: string[] = ['select', 'patientNumber', 'fullName', 'email', 'phoneNumber', 'visitTypeName', 'status','createdDate' ,'actions'];

  dataSource = new MatTableDataSource<PatientResponseDto>();
  searchKeyword = '';
  patientId!: string | null;
  isLoading = signal(false);
  isDeleting = signal(false);
  isBulkDeleting = signal(false); // Add bulk delete loading state
  isOpenDialog = signal(false);
  // Selection model for checkboxes
  selection = new SelectionModel<PatientResponseDto>(true, []);
  pagedData: PatientResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private router: Router,
    private readonly patientService: PatientService,
    private notificationService: NotificationService,
    private readonly userAccountService: UserAccountService,
    private confirmationService: ConfirmationDialogService,
    private dialog: MatDialog
  ) {}

  async ngOnInit(): Promise<void> {
    await this.userAccount();
    this.loadData();
  }

  async userAccount(): Promise<void> {
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

  this.dataSource.filterPredicate = (data: PatientResponseDto, filter: string): boolean => {
    if (!filter) return true; // show all if filter is empty

    const value = filter.trim().toLowerCase();

    const status = data.isActive ? 'active' : 'inactive';

    const fieldsToSearch = [
      data.fullName?.toLowerCase() ?? '',
      data.email?.toLowerCase() ?? '',
      data.patientNumber?.toLowerCase() ?? '',
      data.phoneNumber?.toLowerCase() ?? '',
      data.visitTypeName?.toLowerCase() ?? '',
      data.createdBy?.toLowerCase() ?? '',
      status,
    ];

    return fieldsToSearch.some(field => field.includes(value));
  };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
}

  customSortingDataAccessor = (item: PatientResponseDto, property: string): string | number => {
      switch (property) {
      case 'status':
        return item.isActive ? 'Inactive' : 'Active';
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

  loadData(): void {
    this.isLoading.set(true);
    // Clear selection when loading new data
    this.selection.clear();

    this.patientService.getAllPatients()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (apiResponse) => {
          this.dataSource.data = apiResponse;
          this.dataSource.sort = this.sort;
          this.dataSource.paginator = this.paginator;
          this.dataSource.filter = '';
          this.updatePagedData();
          this.isLoading.set(false);

        },
        error: (err) => {
          this.isLoading.set(false);
          this.notificationService.showSnackBar('Failed to load patients.', 'failure');
        },
      });
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

  toggleRowSelection(row: PatientResponseDto): void {
    this.selection.toggle(row);
  }

  isSelected(row: PatientResponseDto): boolean {
    return this.selection.isSelected(row);
  }

  // Get count of selected items
  getSelectedCount(): number {
    return this.selection.selected.length;
  }

  // Clear all selections
  clearSelection(): void {
    this.selection.clear();
  }



  onEdit(request: PatientDisplay | PatientResponseDto): void {
    this.router.navigateByUrl(`/patient/edit/${request.id}`);
  }

  showForm( id: string | null): void {
  if (id == null) {
    this.router.navigateByUrl('/patient/add');
  } else {
    this.router.navigateByUrl(`/patient/edit/${id}`);
  }
}

  handleBack(): void {
    this.loadData();
  }

openCounselorDialog(isBulk:boolean = true,rowId?:any) {
    const dialogRef = this.dialog.open(UserAssigneeComponent, {
      width: '400px',
       data: { isFrom: UserRole.SalesPerson }
    });

    dialogRef.afterClosed().subscribe((counselorId: DropDownResponseDto) => {
      if (counselorId) {
        this.onCounselorAssigned(counselorId.id as number,isBulk,rowId);
      }
    });
  }

onCounselorAssigned(counselorId: number | null,isBulk:boolean,rowId?:any): void {
  if (counselorId !== null) {
    var ids = [];
    if(rowId != null){
      ids = [rowId];
    }else{

      ids = this.selection.selected.map(row => row.id);
    }
    this.patientService.bulkAssignee(ids, counselorId).subscribe({
      next: (response: CommonOperationResponseDto) => {
        this.notificationService.showSnackBar(response.message || 'Counselor assigned successfully!', 'success');
        this.loadData();
        this.clearSelection();
      },
      error: () => {
        this.notificationService.showSnackBar('Failed to assign counselor.', 'failure');
      }
    });
  }
}


 async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
     const userText = isBulk
      ? `(${count}) patient${count > 1 ? 's' : ''}`
      : `this patient`;

     const data: ConfirmationDialogData = {
      title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action}</strong> ${userText}?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationService.openConfirmation(data).toPromise();
    return result ?? false;
  }

  async performAction(action: string, patientOrIsBulk: any, isBulk: boolean = false): Promise<void> {
    const patientIds = isBulk? this.selection.selected.map(user => user.id): [patientOrIsBulk.id];

    if (!patientIds.length) return;

    if (action !== 'assign-counsellor') {
      const confirmed = await this.openConfirmationDialog(action, patientIds.length, isBulk);
      if (!confirmed) return;
    }

    this.isLoading.set(true);

    let requestFn: (() => Promise<any>) | null = null;
    const isDeactivate = action === 'deactivate';
    let successMessage = '';

    switch (action) {
      case 'activate':
        requestFn = () => firstValueFrom(this.patientService.bulkToggleActive(patientIds, isDeactivate));
        successMessage = isBulk ? 'Patient(s) activated successfully!' : 'Patient activated successfully!';
        break;

      case 'deactivate':
        requestFn = () => firstValueFrom(this.patientService.bulkToggleActive(patientIds, isDeactivate));
        successMessage = isBulk ? 'Patient(s) deactivated successfully!' : 'Patient deactivated successfully!';
        break;

      case 'delete':
        requestFn = () => firstValueFrom(this.patientService.bulkDeletePatients(patientIds));
        successMessage = isBulk ? 'Patient(s) deleted successfully!' : 'Patient deleted successfully!';
        break;

      case 'assign-counsellor':
        if (isBulk) {
          this.openCounselorDialog(true);
        } else {
          this.openCounselorDialog(false, patientIds[0]);
        }
        this.isLoading.set(false);
        return;

      default:
        this.isLoading.set(false);
        return;
    }

    try {
      if (requestFn) {
        await requestFn();
        this.notificationService.showSnackBar(successMessage, 'success');
        await this.loadData();
      }
    } catch (err) {
      console.error(`Failed to ${action} patient${isBulk ? '(s)' : ''}`, err);
      this.notificationService.showSnackBar(`Failed to ${action} patient${isBulk ? '(s)' : ''}.`, 'failure');
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
