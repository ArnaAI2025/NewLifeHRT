  import { Component, OnInit, ViewChild, AfterViewInit, signal } from '@angular/core';
  import { MatTableDataSource } from '@angular/material/table';
  import { MatPaginator, PageEvent } from '@angular/material/paginator';
  import { MatSort } from '@angular/material/sort';
  import { SelectionModel } from '@angular/cdk/collections';

  import { UserAccountService } from '../../../shared/services/user-account.service';
  import { NotificationService } from '../../../shared/services/notification.service';
  import { EmailAndSmsService } from '../email-sms.services';
  import { MessageResponseDto } from '../model/message-response.model';

  import { CommonModule } from '@angular/common';
  import { FormsModule } from '@angular/forms';
  import { MatTableModule } from '@angular/material/table';
  import { MatPaginatorModule } from '@angular/material/paginator';
  import { MatSortModule } from '@angular/material/sort';
  import { MatCheckboxModule } from '@angular/material/checkbox';
  import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
  import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

  @Component({
    selector: 'app-unseen-sms',
    standalone: true,
    imports: [
      CommonModule,
      FormsModule,
      MatTableModule,
      MatPaginatorModule,
      MatSortModule,
      MatCheckboxModule,
      MatProgressSpinnerModule,
      MatIconModule,
    ],
    templateUrl: './unseen-sms.html',
    styleUrls: ['./unseen-sms.scss'],
  })
  export class UnSeenSms implements OnInit, AfterViewInit {
    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    userId = signal<number | null>(null);
    dataSource = new MatTableDataSource<MessageResponseDto>([]);
    selection = new SelectionModel<MessageResponseDto>(true, []);
    searchKeyword = '';
    isLoading = signal(false);
    pageSize = 5;
    pageIndex = 0;
    pagedData: MessageResponseDto[] = [];

    displayedColumns = ['select', 'name', 'relatedTo', 'phoneNumber', 'timestamp', 'contentSummary'];

    constructor(
      private router: Router,
      private userAccountService: UserAccountService,
      private emailAndSmsService: EmailAndSmsService,
      private notificationService: NotificationService,
    ) {}

    ngOnInit(): void {
      const user = this.userAccountService.getUserAccount();
      if (user) {
        this.userId.set(user.id);
        this.loadData();
      }
    }

    ngAfterViewInit(): void {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;

      this.dataSource.filterPredicate = (data: MessageResponseDto, filter: string) => {
        const searchText = filter.trim().toLowerCase();
        const name = data.name?.toLowerCase() ?? '';
        const direction = data.direction?.toLowerCase() ?? '';
        const timestamp = new Date(data.timestamp).toLocaleString().toLowerCase();

        return name.includes(searchText) || direction.includes(searchText) || timestamp.includes(searchText);
      };

      this.paginator.page.subscribe((event: PageEvent) => {
        this.pageSize = event.pageSize;
        this.pageIndex = event.pageIndex;
        this.updatePagedData();
      });

      this.updatePagedData();
    }

    applyFilter(): void {
      this.dataSource.filter = this.searchKeyword;
      if (this.dataSource.paginator) {
        this.dataSource.paginator.firstPage();
      }
      this.updatePagedData();
    }

    private loadData(): void {
      const id = this.userId();
      if (id === null) return;

      this.isLoading.set(true);
      this.emailAndSmsService.getUnseenSms(id).subscribe({
        next: (res: MessageResponseDto[]) => {
          this.dataSource.data = res ?? [];
          this.selection.clear();
          this.searchKeyword = '';
          this.updatePagedData();
        },
        error: (error) => {
          console.error('Error loading unseen SMS:', error);
          this.notificationService.showSnackBar('Failed to load unseen SMS messages.', 'failure');
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }

    private updatePagedData(): void {
      const data = this.dataSource.filteredData?.length ? this.dataSource.filteredData : this.dataSource.data;
      const startIndex = this.pageIndex * this.pageSize;
      const endIndex = startIndex + this.pageSize;
      this.pagedData = data.slice(startIndex, endIndex);
    }

    // Selection logic
    isAllSelected(): boolean {
      const numSelected = this.selection.selected.length;
      const numRows = this.dataSource.filteredData.length;
      return numSelected === numRows && numRows > 0;
    }

    masterToggle(): void {
      if (this.isAllSelected()) {
        this.selection.clear();
      } else {
        this.selection.select(...this.dataSource.filteredData);
      }
    }

    toggleRowSelection(row: MessageResponseDto): void {
      this.selection.toggle(row);
    }

    isSelected(row: MessageResponseDto): boolean {
      return this.selection.isSelected(row);
    }

    /** Mark selected unread messages as read **/
    markSelectedMessagesAsRead(): void {
      if (this.selection.isEmpty()) {
        this.notificationService.showSnackBar('No messages selected', 'normal');
        return;
      }

      const ids = this.selection.selected.map(m => m.messageId);
      this.isLoading.set(true);

      this.emailAndSmsService.markMessagesAsRead(ids).subscribe({
        next: () => {
          this.notificationService.showSnackBar('Selected messages marked as read', 'success');
          this.loadData();
        },
        error: (err) => {
          console.error('Mark as read failed', err);
          this.notificationService.showSnackBar('Failed to mark messages as read', 'failure');
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }

getDetailedMediaSummary(message: MessageResponseDto): string {
  if (!message.messageContents || message.messageContents.length === 0) {
    return '';
  }

  const textContent = message.messageContents.find(c => c.contentType === 'text');
  const mediaContents = message.messageContents.filter(c => c.contentType !== 'text');

  let result = '';

  // Text snippet
  if (textContent && textContent.content) {
    result = textContent.content.length > 50 ? textContent.content.substring(0, 50) + '...' : textContent.content;
  }

  // Media counts
  if (mediaContents.length > 0) {
    const counts = mediaContents.reduce((acc, content) => {
      const mainType = content.contentType.split('/')[0];
      acc[mainType] = (acc[mainType] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    const mediaSummary = Object.entries(counts)
      .map(([type, count]) => `${count} ${type}${count > 1 ? 's' : ''}`)
      .join(', ');

    result += result ? `, ${mediaSummary}` : mediaSummary;
  }

  return result;
}
  
    onEdit(request: MessageResponseDto ): void {
      if(request.isPatient)
      this.router.navigateByUrl(`/patient/${request.id}/sms`);
    else if(!request.isPatient)
      this.router.navigateByUrl(`/lead-management/${request.id}/sms`);
    }

  }
