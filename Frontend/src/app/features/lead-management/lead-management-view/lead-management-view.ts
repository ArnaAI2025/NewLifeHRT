import { Component, OnInit, ViewChild, AfterViewInit, inject, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { MatMenuTrigger } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { firstValueFrom } from 'rxjs';

import { NotificationService } from '../../../shared/services/notification.service';
import { LeadDisplay } from '../model/lead-display.model';
import { LeadManagementService } from '../lead-management.service';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { MatDialog } from '@angular/material/dialog';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { UserAssigneeComponent } from '../../../shared/components/user-assignee/user-assignee';
import { UserRole } from '../../../shared/enums/user-role.enum';



@Component({
  selector: 'app-lead-management-view',
  standalone: true,
  templateUrl: './lead-management-view.html',
  styleUrls: ['./lead-management-view.scss'],
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatTableModule

  ],
})
export class LeadManagementView implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatMenuTrigger) menuTrigger!: MatMenuTrigger;

  displayedColumns: string[] = [
    'select',
    'fullName',
    'email',
    'mobileNumber',
    'topic',
    'owner',
    'isQualified',
    'status',
    'createdDate',
    'actions',
  ];
  public UserRole = UserRole;
  dataSource = new MatTableDataSource<LeadDisplay>([]);
  selection = new SelectionModel<LeadDisplay>(true, []);
  isLoading = signal(false);
  isBulkProcessing = signal(false);
  searchKeyword = '';
  selectedLeadId: number | null = null;
  selectedRow: LeadDisplay | null = null;
  isOpenDialog = signal(false);
  selectedCount = signal(0);
  pagedData: LeadDisplay[] = [];
  pageSize = 5;
  pageIndex = 0;
  constructor(
    private leadManagementService:LeadManagementService,
    private notificationService : NotificationService,
    private confirmationDialogService : ConfirmationDialogService,
    private router : Router,
    private dialog : MatDialog
  ){}

  ngOnInit(): void {
    this.updateSelectedCount();
    this.loadLeads();
  }

ngAfterViewInit(): void {
  this.dataSource.paginator = this.paginator;
  this.dataSource.sort = this.sort;

  this.dataSource.sortingDataAccessor = (item: LeadDisplay, property: string) => {
    switch (property) {
      case 'fullName':
        return item.fullName?.toLowerCase() || '';
      case 'email':
        return item.email?.toLowerCase() || '';
      case 'phoneNumber': // used for Mobile Number column
      case 'mobileNumber':
        return item.phoneNumber?.toLowerCase() || '';
      case 'subject': // topic
      case 'topic':
        return item.subject?.toLowerCase() || '';
      case 'ownerFullName':
      case 'owner':
        return item.ownerFullName?.toLowerCase() || '';
      case 'isQualified':
        return item.isQualified === true ? 1 :
               item.isQualified === false ? -1 : 0;
      case 'isActive':
      case 'status':
        return item.isActive ? 1 : 0;
      case 'createdAt':
      case 'createdDate':
        return item.createdAt ? new Date(item.createdAt).getTime() : 0;
      default:
        return (item as any)[property] ?? '';
    }
  };

  this.dataSource.filterPredicate = (data: LeadDisplay, filter: string): boolean => {
    const value = filter.toLowerCase();
    return (
      (data.subject ?? '').toLowerCase().includes(value) ||
      (data.fullName ?? '').toLowerCase().includes(value) ||
      (data.email ?? '').toLowerCase().includes(value) ||
      (data.phoneNumber ?? '').toLowerCase().includes(value) ||
      (data.ownerFullName ?? '').toLowerCase().includes(value) ||
      (data.isActive ? 'Active' : 'Inactive').toLowerCase().includes(value) ||
      (data.isQualified === true
          ? 'Qualified'
          : data.isQualified === false
            ? 'Not Qualified'
            : 'Not Set'
      ).toLowerCase().includes(value)
    );
  };
  this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
}

  updateSelectedCount() {
      this.selectedCount.set(this.selection.selected.length);
  }

  async loadLeads(): Promise<void> {
    this.clearSelection();
    this.isLoading.set(true);
    try {
      const response = await firstValueFrom(this.leadManagementService.getAllLeads());
      this.dataSource.data = response.map(lead => ({
        id: lead.id,
        subject: lead.subject,
        firstName: lead.firstName,
        lastName: lead.lastName,
        fullName: `${lead.firstName} ${lead.lastName}`,
        email: lead.email,
        phoneNumber: lead.phoneNumber,
        dateOfBirth: lead.dateOfBirth,
        gender: lead.gender,
        tags: lead.tags,
        address: lead.address,
        addressId: lead.addressId,
        ownerId: lead.ownerId,
        ownerFullName: lead.ownerFullName,
        createdAt: lead.createdAt,
        createdBy : lead.createdBy,
        isActive: lead.isActive ?? true,
        isQualified: lead.isQualified,
      }));
      this.dataSource.paginator = this.paginator;
      if (this.paginator) {
        this.paginator.length = this.dataSource.data.length;
      }
      this.dataSource.sort = this.sort;
      this.updatePagedData();
    } catch (error) {
      this.notificationService.showSnackBar('Failed to load leads.', 'failure');
    } finally {
      this.isLoading.set(false);
      this.isBulkProcessing.set(false);
    }
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (!this.dataSource.filter) {
      this.dataSource.filter = '';
    }
    this.resetPaginator();
  }

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
    this.updateSelectedCount();
  }

  toggleRowSelection(row: LeadDisplay): void {
    this.selection.toggle(row);
    this.updateSelectedCount();
  }

  clearSelection(): void {
    this.selection.clear();
    this.updateSelectedCount();
  }

  showForm(show = true, leadId: number | null = null): void {
    this.selectedLeadId = leadId;
  }

  handleBack(): void {
    this.selectedLeadId = null;
    this.loadLeads();
  }

  onEdit(row: LeadDisplay | null): void {
    this.selectedRow = row;
    if (row) {
      this.router.navigate(['/lead-management/edit', row.id]);
    }
  }
  isSelected(row: LeadDisplay): boolean {
  return this.selection.isSelected(row);
}

async onConvertToPatient(row: LeadDisplay | null): Promise<void> {
  if (!row) return;

  if (row.isQualified === true) {
    this.confirmationDialogService.openConfirmation({
      title: 'Already Qualified',
      message: 'Lead is already qualified as a patient.',
      confirmButtonText: 'OK',
    }).subscribe();
    return;
  }

  if (row.isQualified === false) {
    this.confirmationDialogService.openConfirmation({
      title: 'Already Disqualified',
      message: 'Lead is already disqualified.',
      confirmButtonText: 'OK',
    }).subscribe();
    return;
  }

  await this.runConvertToPatient(
    [row.id],
    'Convert Lead to Patient',
    'Are you sure you want to convert this lead to a patient?'
  );
}

async onBulkConvertToPatient(): Promise<void> {
  const selectedLeads = this.selection.selected as LeadDisplay[];
  if (!selectedLeads.length) return;

  const eligibleLeads = selectedLeads.filter(
    l => l.isQualified === null || l.isQualified === undefined
  );

  if (!eligibleLeads.length) {
    this.confirmationDialogService.openConfirmation({
      title: 'No Eligible Leads',
      message: 'Only leads without a qualification status can be converted to patients.',
      confirmButtonText: 'OK',
    }).subscribe();
    return;
  }

  const countQualified = selectedLeads.filter(l => l.isQualified === true).length;
  const countDisqualified = selectedLeads.filter(l => l.isQualified === false).length;

this.confirmationDialogService.openConfirmation({
  title: 'Convert Selected Leads',
  message: `Convert ${eligibleLeads.length} lead(s) with no qualification status?
(${countQualified} qualified, ${countDisqualified} disqualified will be skipped)`,
  confirmButtonText: 'Yes',
  cancelButtonText: 'No'
}).subscribe(async (confirmed: boolean) => {
  if (confirmed) {
    await this.runConvertToPatient(
      eligibleLeads.map(l => l.id),
      'Convert Selected Leads',
      'Are you sure you want to convert the selected leads into patients?'
    );
  }
});

}

async runConvertToPatient(
  ids: string[],
  dialogTitle: string,
  dialogMessage: string
): Promise<void> {
  this.confirmationDialogService.openConfirmation({
    title: dialogTitle,
    message: dialogMessage
  }).subscribe(async (confirmed: boolean) => {
    if (confirmed) {
      this.isBulkProcessing.set(true);
      try {
        await firstValueFrom(this.leadManagementService.bulkConvertToPatient(ids));
        this.notificationService.showSnackBar(
          ids.length > 1
            ? `Leads successfully converted into patients`
            : `Lead successfully converted to patient.`,
          'success'
        );
        this.clearSelection?.();
        await this.loadLeads();
      } catch (e) {
        this.notificationService.showSnackBar(
          'Failed to convert lead(s) into patient(s)', 'failure'
        );
      } finally {
        this.isBulkProcessing.set(false);
      }
    }
  });
}

  onAddLead(): void {
    this.router.navigate(['/lead-management/add']);
  }

async onDisqualifyLead(row: LeadDisplay | null): Promise<void> {
  if (!row) return;
  await this.confirmAndProcessDisqualification([row]);
}

async onBulkDisqualifyLeads(): Promise<void> {
  const selectedLeads = this.selection.selected as LeadDisplay[];
  if (!selectedLeads.length) return;
  await this.confirmAndProcessDisqualification(selectedLeads);
}

private async confirmAndProcessDisqualification(leads: LeadDisplay[]): Promise<void> {
  const eligibleLeads = leads.filter(
    l => l.isQualified === null || l.isQualified === undefined
  );

  if (!eligibleLeads.length) {
    this.confirmationDialogService.openConfirmation({
      title: 'No Eligible Leads',
      message: 'Only leads without a qualification status can be disqualified. None selected meet this criteria.',
      confirmButtonText: 'OK',
    }).subscribe();
    return;
  }

  // Prepare message depending on mode
  if (leads.length > 1) {
    const countIneligible = leads.length - eligibleLeads.length;

this.confirmationDialogService.openConfirmation({
  title: 'Disqualify Selected Leads',
  message: `Disqualify ${eligibleLeads.length} lead(s) with no qualification status?
(${countIneligible} will be skipped)`,
  confirmButtonText: 'Yes',
  cancelButtonText: 'No'
}).subscribe(async (confirmed: boolean) => {
      if (confirmed) {
        await this.executeDisqualification(eligibleLeads.map(l => l.id));
      }
    });
  } else {
    this.confirmationDialogService.openConfirmation({
      title: 'Disqualify Lead',
      message: 'Are you sure you want to disqualify this lead?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).subscribe(async (confirmed: boolean) => {
      if (confirmed) {
        await this.executeDisqualification(eligibleLeads.map(l => l.id));
      }
    });
  }
}

private async executeDisqualification(ids: string[]): Promise<void> {
  try {
    const res = await firstValueFrom(this.leadManagementService.bulkToggleIsQualified(ids));

    if (res?.id ) {
      this.notificationService.showSnackBar(res.message || 'Leads have been disqualified successfully.', 'success');
    } else {
      this.notificationService.showSnackBar(res?.message || 'Failed to disqualify leads.', 'failure');
    }

    await this.loadLeads();
  } catch (err) {
    this.notificationService.showSnackBar('Something went wrong while disqualifying leads.', 'failure');
    console.error(err);
  }
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
    this.leadManagementService.bulkAssignee(ids, counselorId).subscribe({
      next: (response: CommonOperationResponseDto) => {
        this.notificationService.showSnackBar(response.message || 'Counselor assigned successfully', 'success');
        this.loadLeads();
        this.clearSelection();
      },
      error: () => {
        this.notificationService.showSnackBar('Failed to assign counselor.', 'failure');
      }
    });
  }
}

async bulkActionAsync(action: string): Promise<void> {
  const selectedIds = this.selection.selected.map(user => user.id);
  if (!selectedIds.length) return;

  if (action !== 'assign-counsellor') {
    const confirmed = await this.openConfirmationDialog(action, selectedIds.length, true);
    if (!confirmed) return;
  }

  this.isLoading.set(true);

  let requestFn: (() => Promise<any>) | null = null;
  let successMessage = '';

  const isAction = action === 'activate';

  switch (action) {
    case 'delete':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkDeleteLeads(selectedIds));
      successMessage = `Lead(s) deleted successfully`;
      break;

    case 'activate':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkToggleActive(selectedIds, isAction));
      successMessage = `Lead(s) activated successfully`;
      break;

    case 'deactivate':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkToggleActive(selectedIds, isAction));
      successMessage = `Lead(s) deactivated successfully`;
      break;

    case 'assign-counsellor':
      this.openCounselorDialog(true);
      this.isLoading.set(false);
      return;
  }

  try {
    if (requestFn) {
      await requestFn();
      this.notificationService.showSnackBar(successMessage, 'success');
      await this.loadLeads();
    }
  } catch (err) {
    console.error(`Failed to ${action} patient(s)`, err);
    this.notificationService.showSnackBar(`Failed to ${action} Lead(s).`, 'failure');
  } finally {
    this.isLoading.set(false);
  }
}


 async openConfirmationDialog(action: string, count: number = 1, isBulk = false): Promise<boolean> {
     const userText = isBulk
      ? `(${count}) Lead${count > 1 ? 's' : ''}`
      : `this Lead`;

     const data: ConfirmationDialogData = {
      title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action}</strong> ${userText}?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationDialogService.openConfirmation(data).toPromise();
    return result ?? false;
  }

  async performAction(action: string, lead: any): Promise<void> {
  if (action !== 'assign-counsellor') {
    const confirmed = await this.openConfirmationDialog(action, 1);
    if (!confirmed) return;
  }

  const leadId = lead.id;
  this.isLoading.set(true);

  let requestFn: (() => Promise<any>) | null = null;
  let successMessage = '';

  const isAction = action === 'activate';

  switch (action) {
    case 'activate':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkToggleActive([leadId], isAction));
      successMessage = `Lead activated successfully`;
      break;

    case 'deactivate':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkToggleActive([leadId], isAction));
      successMessage = `Lead deactivated successfully`;
      break;

    case 'delete':
      requestFn = () => firstValueFrom(this.leadManagementService.bulkDeleteLeads([leadId]));
      successMessage = `Lead deleted successfully`;
      break;

    case 'assign-counsellor':
      this.openCounselorDialog(false, leadId);
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
      await this.loadLeads();
    }
  } catch (err) {
    console.error(`Failed to ${action} lead`, err);
    this.notificationService.showSnackBar(`Failed to ${action} lead.`, 'failure');
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
