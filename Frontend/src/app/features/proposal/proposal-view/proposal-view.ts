import { Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { ProposalBulkResponseDto } from '../model/proposal-bulk-response.model';
import { ProposalService } from '../proposal.service';
import { BulkOperationResponseDto } from '../../../shared/models/bulk-operation-response.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { UserAssigneeComponent } from '../../../shared/components/user-assignee/user-assignee';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { MatDialog } from '@angular/material/dialog';
import { UserRole } from '../../../shared/enums/user-role.enum';
import { Status } from '../../../shared/enums/status.enum';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';

@Component({
  selector: 'app-proposal-view',
  standalone: true,
  templateUrl: './proposal-view.html',
  styleUrls: ['./proposal-view.scss'],
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatButtonModule, MatCheckboxModule, MatPaginatorModule,
    MatSortModule, MatMenuModule, MatTableModule, MatProgressSpinnerModule,PatientNavigationBarComponent
  ]
})
export class ProposalView implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns: string[] = [
    'select', 'name', 'patientName', 'pharmacyName', 'counselorName',
    'therapyExpiration', 'status', 'actions'
  ];
  dataSource = new MatTableDataSource<ProposalBulkResponseDto>([]);
  selection = new SelectionModel<ProposalBulkResponseDto>(true, []);
  searchKeyword = '';
  isLoading = signal(false);
  isDeleting = false;

  activeRow: ProposalBulkResponseDto | null = null;
  public UserRole = UserRole;
  public Status = Status;
  patientId: string | null = null;

  pagedData: ProposalBulkResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private proposalService: ProposalService,
    private notificationService: NotificationService,
    private confirmationDialog: ConfirmationDialogService,
    private router: Router,
    private dialog: MatDialog,
    private activatedRoute : ActivatedRoute

  ) {}

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
    this.patientId = params.get('patientId');
    });
    this.loadProposals();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data, filter) => {
      const val = filter.toLowerCase();
      const statusValue = data.status ?? 0;
      return (
        data.name?.toLowerCase().includes(val) ||
        data.patientName?.toLowerCase().includes(val) ||
        data.pharmacyName?.toLowerCase().includes(val) ||
        data.counselorName?.toLowerCase().includes(val) ||
        this.getStatusDisplayText(statusValue).toLowerCase().includes(val)
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });
    this.updatePagedData();
  }

  async loadProposals(): Promise<void> {
    this.isLoading.set(true);
    try {
      const proposals = await firstValueFrom(this.proposalService.getAllPropsalsOnPatientId(this.patientId ?? undefined));
      this.dataSource.data = proposals || [];
      this.updatePagedData();
    } catch (error) {
      console.error('Failed to load proposals:', error);
      this.notificationService.showSnackBar('Failed to load proposals', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  applyFilter() {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.selection.clear();
    this.resetPaginator();
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.filteredData.length;
    return numRows > 0 && numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ? this.selection.clear() : this.dataSource.filteredData.forEach(row => this.selection.select(row));
  }

  toggleRowSelection(row: ProposalBulkResponseDto) {
    this.selection.toggle(row);
  }

  isSelected(row: ProposalBulkResponseDto) {
    return this.selection.isSelected(row);
  }

  getSelectedCount() {
    return this.selection.selected.length;
  }

  onAddProposal() {
    if(this.patientId)
    {
      this.router.navigate(['/proposal/add', this.patientId]);
      return;
    }
    this.router.navigate(['/proposal/add']);
  }

onEdit(row: ProposalBulkResponseDto | null) {
  if (row?.id) {
    if (this.patientId) {
      this.router.navigate(['/patient', this.patientId, 'proposal', 'edit', row.id]);
    } else {
      this.router.navigate(['/proposal/edit', row.id]);
    }
  }
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

  async deleteProposals(ids: string[]) {
    if (!ids.length) return;

    const title = ids.length > 1 ? 'Delete Selected Proposals' : 'Delete Proposal';
    const message = `Are you sure you want to delete ${ids.length > 1 ? 'these proposals' : 'this proposal'}? This action cannot be undone.`;

    const confirmed = await this.confirmDialog(title, message);
    if (!confirmed) return;

    this.isDeleting = true;
    try {
      const response = await firstValueFrom(this.proposalService.deleteProposal(ids));

      this.selection.clear();
      await this.loadProposals(); // Refresh data
      this.notificationService.showSnackBar(
        response.message || `Proposal${ids.length > 1 ? 's' : ''} deleted successfully`,
        'success'
      );
    } catch (error) {
      console.error(`Failed to delete proposal${ids.length > 1 ? 's' : ''}:`, error);
      this.notificationService.showSnackBar(`Failed to delete proposal${ids.length > 1 ? 's' : ''}.`, 'failure');
    } finally {
      this.isDeleting = false;
    }
  }


  async onDelete(row: ProposalBulkResponseDto | null) {
    if (row) {
      await this.deleteProposals([row.id]);
    }
  }


  async onBulkDelete() {
    const ids = this.selection.selected.map(s => s.id);
    await this.deleteProposals(ids);
  }
  isTherapyExpired(therapyExpiration?: string): boolean {
    if (!therapyExpiration) return false;
    return new Date(therapyExpiration) < new Date();
  }

  getFormattedDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
  onBulkAssign() {
      const dialogRef = this.dialog.open(UserAssigneeComponent, {
        width: '400px',
        data: { isFrom: UserRole.SalesPerson }
      });

      dialogRef.afterClosed().subscribe((counselorId: DropDownResponseDto) => {
        if (counselorId) {
          console.log('Selected counselor ID:', counselorId.id);
          this.onCounselorAssigned(counselorId.id as number);
        } else {
          console.log('Dialog closed without selecting counselor');
        }
      });
    }

  onCounselorAssigned(counselorId: number | null): void {
    if (counselorId !== null) {
      const ids = this.selection.selected.map(row => row.id);
      this.proposalService.bulkAssignee(ids, counselorId).subscribe({
        next: (response: CommonOperationResponseDto) => {
          this.notificationService.showSnackBar(response.message || 'Counselor assigned successfully!', 'success');
          this.loadProposals();
          this.selection.clear();
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to assign counselor.', 'failure');
        }
      });
    }
  }
  getStatusDisplayText(status?: number): string {
    const statusMap = Object.keys(Status).find(
      key => (Status as any)[key] === status
    );
    return statusMap || 'Unknown';
  }

  getStatusClassByEnum(status: number): string {
    switch (status) {
      case Status.Draft:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
      case Status.InReview:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case Status.Approved:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.Canceled:
        return 'bg-yellow-100 text-yellow-800 border border-yellow-300';
      case Status.Rejected:
        return 'bg-red-100 text-red-800 border border-red-300';
      case Status.ApprovedByPatient:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.RejectedByPatient:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
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
