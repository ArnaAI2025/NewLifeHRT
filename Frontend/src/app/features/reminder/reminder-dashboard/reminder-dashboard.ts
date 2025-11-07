import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ReminderDashboard } from '../model/reminder-dashboard.response.model';
import { ReminderService } from '../reminder.services';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-reminder-dashboard',
  imports: [CommonModule, MatButtonToggleModule, MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule],
  templateUrl: './reminder-dashboard.html',
  styleUrl: './reminder-dashboard.scss'
})
export class ReminderDashboardComponent implements OnInit, AfterViewInit {
  activeTab: 'patient' | 'lead' = 'patient';

  displayedColumns: string[] = ['name', 'reminderDateTime', 'reminderTypeName', 'description', 'isRecurring', 'recurrenceEndDate', 'actions'];
  dataSource = new MatTableDataSource<ReminderDashboard>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  pagedData: ReminderDashboard[] = [];
  pageSize = 10;
  pageIndex = 0;

  constructor(private reminderService: ReminderService, private router: Router, private confirmationDialogService: ConfirmationDialogService, private notificationService: NotificationService) { }

  ngOnInit(): void {
    this.loadRemindersForActiveTab();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (data: ReminderDashboard, filter: string) => {
      const value = filter.toLowerCase();

      const reminderDateStr = this.formatDateForFilter(data.reminderDateTime);
      const recurrenceEndStr = this.formatDateForFilter(data.recurrenceEndDate);

      return (
        (data.name || '').toLowerCase().includes(value) ||
        (data.reminderTypeName || '').toLowerCase().includes(value) ||
        (data.description || '').toLowerCase().includes(value) ||
        reminderDateStr.toLowerCase().includes(value) ||
        recurrenceEndStr.toLowerCase().includes(value)
      );
    };

    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  async updateTab(event: MatButtonToggleChange): Promise<void> {
    this.activeTab = event.value as 'patient' | 'lead';
    await this.loadRemindersForActiveTab();
  }

  async loadRemindersForActiveTab(): Promise<void> {
    this.isLoading.set(true);
    try {
      let response: ReminderDashboard[] = [];
      if (this.activeTab === 'patient') {
        response = await firstValueFrom(this.reminderService.getAllActiveRemindersForPatients());
      } else {
        response = await firstValueFrom(this.reminderService.getAllActiveRemindersForLeads());
      }
      this.dataSource.data = response.map(r => ({
        ...r,
      }));
      this.dataSource.filter = '';
      this.resetPaginator();
    } catch (err) {
      console.error('Failed to load reminders', err);
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

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length ? this.dataSource.filteredData : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    if (this.paginator) {
      this.paginator.firstPage();
    }
    this.pageIndex = 0;
    this.updatePagedData();
  }
  openEditPage(element: ReminderDashboard) {
    let url = '';
    if (this.activeTab === 'patient') {
      url = `/patient/edit/${element.patientId}`;
    } else {
      url = `/lead-management/edit/${element.leadId}`;
    }
    window.open(url, '_blank');
  }

  async markAsCompleted(reminderId: string) {
    const confirmed = await firstValueFrom(
      this.confirmationDialogService.openConfirmation({
        title: 'Confirm',
        message: 'Are you sure you want to mark this reminder as completed?',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
        showCancelButton: true
      })
    );

    if (!confirmed) return;

    try {
      await firstValueFrom(this.reminderService.markReminderAsCompleted(reminderId));
      this.notificationService.showSnackBar('Reminder marked as completed', 'success');
      await this.loadRemindersForActiveTab();
    } catch (err) {
      console.error('Failed to mark reminder as completed', err);
      this.notificationService.showSnackBar('Failed to mark reminder as completed', 'failure');
    }
  }

  private formatDateForFilter(date: string | Date | null |undefined): string {
    if (!date) return '';
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
