import { Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ProposalBulkResponseDto } from '../../proposal/model/proposal-bulk-response.model';
import { ProposalService } from '../../proposal/proposal.service';
import { Status } from '../../../shared/enums/status.enum';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../../shared/services/auth.service';
import { AppSettingsService } from '../../../shared/services/app-settings.service';

@Component({
  selector: 'app-proposal-action-panel',
  templateUrl: './proposal-action-panel.html',
  styleUrl: './proposal-action-panel.scss',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatButtonModule, MatPaginatorModule, MatSortModule,
    MatTableModule, MatProgressSpinnerModule, MatTooltipModule, MatMenuModule
  ]
})
export class ProposalActionPanelComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  // Updated columns - removed 'select' and bulk actions
  displayedColumns: string[] = [
    'name', 'patientName', 'pharmacyName', 'counselorName',
    'therapyExpiration', 'status', 'actions'
  ];

  dataSource = new MatTableDataSource<ProposalBulkResponseDto>([]);
  searchKeyword = '';
  isLoading = signal(false);
  Status = Status;

  pagedData: ProposalBulkResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;
  activeRow: ProposalBulkResponseDto | null = null;
  isDeleting = signal<boolean>(false);
  constructor(
    private proposalService: ProposalService,
    private notificationService: NotificationService,
    private confirmationDialog: ConfirmationDialogService,
    private appSettingService: AppSettingsService,
    private router: Router
  ) { }

  ngOnInit(): void {
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
      const isPatient = this.appSettingService.isUserPatient();
      const patientId = isPatient ? this.appSettingService.getPatientUserId() : null;

      const proposals = await firstValueFrom(
        this.proposalService.getAllProposals([Status.InReview, Status.ApprovedByPatient, Status.RejectedByPatient], patientId)
      );

      this.dataSource.data = (proposals || []).filter(p =>
        p.status != null &&
        [Status.Draft, Status.InReview, Status.Approved, Status.Canceled, Status.Rejected, Status.ApprovedByPatient, Status.RejectedByPatient].includes(p.status)
      );
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
    this.resetPaginator();
  }

  onAddProposal() {
    this.router.navigate(['/proposal/add']);
  }

  onEdit(row: ProposalBulkResponseDto | null) {
    if (row && row.id) {
      this.router.navigate(['/proposal/edit', row.id], {
        queryParams: {
          fromDashboard: true
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


  getProposalStatusClass(status: number): string {
    switch (status) {
      case Status.InReview:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case Status.ApprovedByPatient:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.RejectedByPatient:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'hidden';
    }
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

  async onDelete(row: ProposalBulkResponseDto | null) {
    if (row) {
      await this.deleteProposals([row.id]);
    }
  }

  async deleteProposals(ids: string[]) {
    if (!ids.length) return;

    const title = ids.length > 1 ? 'Delete Selected Proposals' : 'Delete Proposal';
    const message = `Are you sure you want to delete ${ids.length > 1 ? 'these proposals' : 'this proposal'}? This action cannot be undone.`;

    const confirmed = await this.confirmDialog(title, message);
    if (!confirmed) return;

    this.isDeleting.set(true);
    try {
      const response = await firstValueFrom(this.proposalService.deleteProposal(ids));

      await this.loadProposals(); // Refresh data
      this.notificationService.showSnackBar(
        response.message || `Proposal${ids.length > 1 ? 's' : ''} deleted successfully`,
        'success'
      );
    } catch (error) {
      console.error(`Failed to delete proposal${ids.length > 1 ? 's' : ''}:`, error);
      this.notificationService.showSnackBar(`Failed to delete proposal${ids.length > 1 ? 's' : ''}.`, 'failure');
    } finally {
      this.isDeleting.set(false);
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
}
