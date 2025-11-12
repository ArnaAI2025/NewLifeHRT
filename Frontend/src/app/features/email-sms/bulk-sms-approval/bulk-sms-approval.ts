import { Component, OnInit, ViewChild, AfterViewInit, signal } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { EmailAndSmsService } from '../email-sms.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { Router } from '@angular/router';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

import { BatchMessageResponseDto } from '../model/batch-message-response.model';
import { Status } from '../../../shared/enums/status.enum';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';

@Component({
  selector: 'app-bulk-sms-approval',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatMenuModule,
  ],
  templateUrl: './bulk-sms-approval.html',
  styleUrls: ['./bulk-sms-approval.scss'],
})
export class BulkSmsApproval implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  isLoading = signal(false);
  filterKeyword = '';
  pageSize = 5;
  pageIndex = 0;

  dataSource = new MatTableDataSource<BatchMessageResponseDto>([]);
  pagedData: BatchMessageResponseDto[] = [];
  Status = Status;
  displayedColumns: string[] = ['relatedTo', 'subject', 'message', 'createdBy', 'createdAt', 'status', 'actions'];

  activeRow?: BatchMessageResponseDto;

  constructor(
    private router: Router,
    private emailAndSmsService: EmailAndSmsService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationDialogService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (data: BatchMessageResponseDto, filter: string) => {
      if (!filter) return true;

      const keyword = filter.trim().toLowerCase();
      return (
        data.subject?.toLowerCase().includes(keyword) ||
        data.message?.toLowerCase().includes(keyword) ||
        data.createdByUser?.toLowerCase().includes(keyword) ||
        (data.createdAt ? new Date(data.createdAt).toLocaleString().toLowerCase().includes(keyword) : false)
      );
    };

    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  applyFilter(): void {
    this.dataSource.filter = this.filterKeyword.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
    this.updatePagedData();
  }

  private loadData(): void {
    this.isLoading.set(true);
    this.emailAndSmsService.getAllBatchMessages().subscribe({
      next: (res: BatchMessageResponseDto[]) => {
        this.dataSource.data = res ?? [];
        this.filterKeyword = '';
        this.updatePagedData();
      },
      error: (error) => {
        console.error('Error loading bulk SMS:', error);
        this.notificationService.showSnackBar('Failed to load bulk SMS approvals.', 'failure');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  private updatePagedData(): void {
    const data = this.dataSource.filteredData.length ? this.dataSource.filteredData : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    this.pagedData = data.slice(startIndex, startIndex + this.pageSize);
  }

  onEdit(row: BatchMessageResponseDto): void {
    if (!row || !row.batchMessageId) return;
    this.router.navigateByUrl(`/edit-bulk/sms/${row.batchMessageId}`);
  }

  onDelete(row: BatchMessageResponseDto): void {
    if (!row || !row.batchMessageId) return;

    this.confirmationService.openConfirmation({
      title: 'Confirm Delete',
      message: 'Are you sure you want to delete this bulk SMS?',
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
    }).subscribe(confirmed => {
      if (confirmed) {        
        this.emailAndSmsService.deleteBatchMessages([row.batchMessageId!]).subscribe({
          next: () => {
            this.notificationService.showSnackBar('Bulk SMS deleted successfully.', 'success');
            this.loadData();
          },
          error: () => this.notificationService.showSnackBar('Delete failed.', 'failure'),
        });
        
      }
    });
  }

  getStatusDisplayText(status?: number): string {
    const statusMap = Object.keys(Status).find(key => (Status as any)[key] === status);
    return statusMap || 'Unknown';
  }

  getStatusClassByEnum(status: number): string {
    switch (status) {
      case Status.Draft:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
      case Status.InProgress:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case Status.Approved:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.Canceled:
        return 'bg-yellow-100 text-yellow-800 border border-yellow-300';
      case Status.Rejected:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
    }
  }
}
