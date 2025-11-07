import {
  ChangeDetectorRef,
  Component,
  OnInit,
  AfterViewInit,
  ViewChild,
  signal,
} from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import {
  MatPaginator,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

import { NotificationService } from '../../../shared/services/notification.service';
import { CommissionService } from '../commission-service';
import { PoolDetailResponseDto } from '../model/pool-detail-response-model';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MmddyyyyDatePipe } from '../../../shared/pipes/mmddyyyy-date.pipe';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-commission-pool-view',
  templateUrl: './commission-pool-view.html',
  styleUrl: './commission-pool-view.scss',
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
    MmddyyyyDatePipe,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  standalone: true,
})
export class CommissionPoolViewComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'counselorName',
    'week',
    'fromDate',
    'toDate',
    'actions',
  ];
  dataSource = new MatTableDataSource<PoolDetailResponseDto>();
  isLoading = signal(false);
  searchKeyword = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  pagedData: PoolDetailResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;

  customFromDate?: Date;
  customToDate?: Date;

  constructor(
    private cdr: ChangeDetectorRef,
    private readonly poolDetailService: CommissionService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    await this.loadCurrentWeekDetails();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (
      data: PoolDetailResponseDto,
      filter: string
    ) => {
      const value = filter.toLowerCase();
      return (
        (data.counselorName || '').toLowerCase().includes(value) ||
        data.week.toString().includes(value) ||
        this.formatDateToMMDDYYYY(data.fromDate).includes(value) ||
        this.formatDateToMMDDYYYY(data.toDate).includes(value)
      );
    };

    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  // -----------------------------
  // ✅ Local Time–Safe Date Ranges
  // -----------------------------
  getCurrentWeekRange(today: Date = new Date()): { from: Date; to: Date } {
    const base = new Date(
      today.getFullYear(),
      today.getMonth(),
      today.getDate()
    );
    const day = base.getDay(); // 0=Sun...6=Sat
    const daysSinceFriday = (day - 5 + 7) % 7;

    const from = new Date(base);
    from.setDate(base.getDate() - daysSinceFriday);

    const to = new Date(from);
    to.setDate(from.getDate() + 6);

    return {
      from: new Date(from.getFullYear(), from.getMonth(), from.getDate()),
      to: new Date(to.getFullYear(), to.getMonth(), to.getDate()),
    };
  }

  getLastWeekRange(today: Date = new Date()): { from: Date; to: Date } {
    const { from: currentFrom } = this.getCurrentWeekRange(today);
    const from = new Date(currentFrom);
    from.setDate(currentFrom.getDate() - 7);
    const to = new Date(from);
    to.setDate(from.getDate() + 6);
    return { from, to };
  }

  getCurrentMonthRange(today: Date = new Date()): { from: Date; to: Date } {
    const from = new Date(today.getFullYear(), today.getMonth(), 1);
    const to = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    return { from, to };
  }

  getLastMonthRange(today: Date = new Date()): { from: Date; to: Date } {
    const from = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    const to = new Date(today.getFullYear(), today.getMonth(), 0);
    return { from, to };
  }

  // -----------------------------
  // ✅ Quick Filter Handlers
  // -----------------------------
  async onPreviousWeek(): Promise<void> {
    const { from, to } = this.getLastWeekRange();
    await this.loadPoolDetails(from, to);
  }

  async onPreviousMonth(): Promise<void> {
    const { from, to } = this.getLastMonthRange();
    await this.loadPoolDetails(from, to);
  }

  async onApplyCustomDates(): Promise<void> {
    if (!this.customFromDate || !this.customToDate) {
      this.notificationService.showSnackBar(
        'Select both From and To dates',
        'failure'
      );
      return;
    }

    const from = new Date(this.customFromDate);
    const to = new Date(this.customToDate);

    if (isNaN(from.getTime()) || isNaN(to.getTime())) {
      this.notificationService.showSnackBar(
        'Invalid date selection',
        'failure'
      );
      return;
    }

    await this.loadPoolDetails(from, to);
  }

  // -----------------------------
  // ✅ Loaders
  // -----------------------------
  async loadCurrentWeekDetails(): Promise<void> {
    const { from, to } = this.getCurrentWeekRange();
    await this.loadPoolDetails(from, to);
  }

  async loadPoolDetails(fromDate?: Date, toDate?: Date): Promise<void> {
    this.isLoading.set(true);
    try {
      this.customFromDate = fromDate;
      this.customToDate = toDate;

      const responseDto = await firstValueFrom(
        this.poolDetailService.getAllPoolDetails(fromDate, toDate)
      );

      const mapped = responseDto.map((dto) => ({
        ...dto,
        selected: false,
      }));

      this.dataSource.data = mapped;
      this.dataSource.filter = '';
      this.updatePagedData();
    } catch (err) {
      console.error('Failed to load Pool Details', err);
      this.notificationService.showSnackBar(
        'Failed to load Pool Details',
        'failure'
      );
    } finally {
      this.isLoading.set(false);
      this.cdr.detectChanges();
    }
  }

  // -----------------------------
  // ✅ Table + Utility
  // -----------------------------
  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.resetPaginator();
  }

  formatDateToMMDDYYYY(dateString: string): string {
    const date = new Date(dateString);
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    const yyyy = date.getFullYear();
    return `${mm}/${dd}/${yyyy}`;
  }

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length
      ? this.dataSource.filteredData
      : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }

  onEdit(element: PoolDetailResponseDto): void {
    if (!element?.poolDetailId) return;
    this.router.navigateByUrl(
      `/counselor-order-wise-commissions/view/${element.poolDetailId}`
    );
  }
}
