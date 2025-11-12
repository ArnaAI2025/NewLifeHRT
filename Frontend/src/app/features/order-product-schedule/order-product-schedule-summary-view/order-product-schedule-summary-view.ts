// order-product-schedule-summary-view.ts
import { ChangeDetectorRef, Component, OnInit, AfterViewInit, ViewChild, signal } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { OrderproductScheduleCalendarService } from '../order-product-schedule-calendar.services';
import { MatDialog } from '@angular/material/dialog';
import { OrderProductScheduleEditDialogComponent, ScheduleEditDialogData } from '../order-product-schedule-edit-dialog/order-product-schedule-edit-dialog';
import { OrderProductAddReminderDialogComponent } from '../order-product-add-reminder-dialog/order-product-add-reminder-dialog';
import { MatCardModule } from '@angular/material/card';

interface OrderProductScheduleSummary {
  orderProductScheduleSummaryId: string;
  orderName: string;
  orderFulfilled: Date;
  productName: string;
  protocol: string;
  startDate: Date;
  endDate: Date;
  time: string;
  status: string;
}

@Component({
  selector: 'app-order-product-schedule-summary-view',
  imports: [
    MatTableModule,
    FormsModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatInputModule,
    MatSortModule,
    MatPaginatorModule,
    CommonModule,
    DatePipe,
    MatCardModule
  ],
  templateUrl: './order-product-schedule-summary-view.html',
  styleUrl: './order-product-schedule-summary-view.scss'
})
export class OrderProductScheduleSummaryViewComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['orderName', 'orderFulfilled', 'productName', 'protocol', 'startDate', 'endDate', 'time', 'status', 'actions'];
  dataSource = new MatTableDataSource<OrderProductScheduleSummary>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  pagedData: OrderProductScheduleSummary[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private orderProductScheduleCalendarService: OrderproductScheduleCalendarService,
    private confirmationService: ConfirmationDialogService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) { }

  async ngOnInit(): Promise<void> {
    await this.loadScheduleSummary();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (data: OrderProductScheduleSummary, filter: string) => {
      const value = filter.toLowerCase();
      return Object.values(data).some(val => {
        if (Array.isArray(val)) return false;
        if (val instanceof Date) return val.toLocaleDateString().toLowerCase().includes(value);
        return (val || '').toString().toLowerCase().includes(value);
      });
    }

    this.dataSource.sortingDataAccessor = (item: OrderProductScheduleSummary, property: string) => {
      switch (property) {
        case 'orderFulfilled':
          return new Date(item.orderFulfilled);
        case 'startDate':
          return new Date(item.startDate);
        case 'endDate':
          return new Date(item.endDate);
        case 'time':
          return this.convertTimeToSortableFormat(item.time);
        default:
          return (item as any)[property];
      }
    };

    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  private convertTimeToSortableFormat(timeString: string): string {
    try {
      const [time, modifier] = timeString.split(' ');
      let [hours, minutes] = time.split(':').map(Number);

      if (modifier === 'PM' && hours !== 12) {
        hours += 12;
      }
      if (modifier === 'AM' && hours === 12) {
        hours = 0;
      }

      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
    } catch {
      return timeString;
    }
  }

  async loadScheduleSummary(): Promise<void> {
    this.isLoading.set(true);
    try {
      const response = await firstValueFrom(this.orderProductScheduleCalendarService.getPatientScheduleSummary());

      if (response.isPatient) {
        const mappedSchedules: OrderProductScheduleSummary[] = response.schedules.map((dto: any) => ({
          orderProductScheduleSummaryId: dto.orderProductScheduleSummaryId,
          orderName: dto.orderName,
          orderFulfilled: new Date(dto.orderFulfilled),
          productName: dto.productName,
          protocol: dto.protocol,
          startDate: new Date(dto.startDate),
          endDate: new Date(dto.endDate),
          time: dto.time,
          status: dto.status
        }));
        this.dataSource.data = mappedSchedules;
        this.dataSource.filter = '';
        this.updatePagedData();
      } else {
        this.notificationService.showSnackBar(response.message, 'failure');
        this.dataSource.data = [];
      }
    } catch (err) {
      console.error('Failed to load Schedule Summary', err);
      this.notificationService.showSnackBar('Failed to load schedule summary data.', 'failure');
    } finally {
      this.isLoading.set(false);
      this.cdr.detectChanges();
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
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }

  async onEdit(element: OrderProductScheduleSummary): Promise<void> {
    this.isLoading.set(true);
    try {
      const response = await firstValueFrom(
        this.orderProductScheduleCalendarService.getScheduleSummaryById(element.orderProductScheduleSummaryId)
      );

      if (response && response.schedule) {
        const dialogData: ScheduleEditDialogData = {
          scheduleSummary: response.schedule,
          scheduleId: element.orderProductScheduleSummaryId
        };

        const dialogRef = this.dialog.open(OrderProductScheduleEditDialogComponent, {
          width: '90vw',
          maxWidth: '600px',
          height: 'auto',
          maxHeight: '90vh',
          autoFocus: false,
          restoreFocus: false,
          data: dialogData,
          panelClass: 'edit-dialog-responsive'
        });

        dialogRef.afterClosed().subscribe(async result => {
          if (result?.success && result.request) {
            try {
              this.isLoading.set(true);
              const response = await firstValueFrom(
                this.orderProductScheduleCalendarService.updateScheduleSummary(
                  result.scheduleId,
                  result.request
                )
              );

              if (response.success) {
                this.notificationService.showSnackBar('Schedule updated successfully!', 'success');
                await this.loadScheduleSummary();
              } else {
                this.notificationService.showSnackBar(response.message || 'Failed to update schedule.', 'failure');
              }
            } catch (error) {
              console.error('Error updating schedule:', error);
              this.notificationService.showSnackBar('Error updating schedule.', 'failure');
            } finally {
              this.isLoading.set(false);
            }
          }
        });
      } else {
        this.notificationService.showSnackBar('Failed to load schedule details.', 'failure');
      }
    } catch (error) {
      console.error('Error loading schedule details:', error);
      this.notificationService.showSnackBar('Failed to load schedule details.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  private async updateScheduleTime(scheduleIds: string[], time: string): Promise<void> {
    this.isLoading.set(true);

    try {
      const response = await firstValueFrom(
        this.orderProductScheduleCalendarService.updateScheduleTime(scheduleIds, time)
      );

      if (response.success) {
        this.notificationService.showSnackBar('Schedule time updated successfully!', 'success');
        await this.loadScheduleSummary();
      } else {
        this.notificationService.showSnackBar(response.message || 'Failed to update schedule time.', 'failure');
      }
    } catch (error) {
      console.error('Error updating schedule time:', error);
      this.notificationService.showSnackBar('Failed to update schedule time. Please try again.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  navigateToCalendar() {
    this.router.navigate(['/order-product-schedule/calendar']);
  }

  openAddReminderDialog(): void {
    const dialogRef = this.dialog.open(OrderProductAddReminderDialogComponent, {
      width: '400px',
      maxHeight: '90vh',
      autoFocus: false,
      restoreFocus: false,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        this.notificationService.showSnackBar('Reminder added successfully!', 'success');
      }
    });
  }
}
