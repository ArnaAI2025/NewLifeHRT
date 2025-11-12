import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { UserManagementService } from '../../user-management/user-management.service';
import { firstValueFrom } from 'rxjs';
import { DropdownResponseDto } from '../../price-list-items/model/dropdown-response.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HolidayService } from '../holiday.services';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

type DateKey = string;

@Component({
  selector: 'app-holiday-add',
  imports: [CommonModule, FormsModule, MatProgressSpinnerModule, MatIconModule],
  templateUrl: './holiday-add.html',
  styleUrl: './holiday-add.scss'
})
export class HolidayAddComponent implements OnInit {
  users = signal<any[]>([]);
  selectedUser: number | null = null;
  leaveType: 'custom' | 'recurrence' = 'custom';
  viewDate: Date = new Date();
  blanks: any[] = [];
  days: number[] = [];
  selectedKeys = new Set<DateKey>();
  dateTimes: Record<DateKey, { start: string; end: string }> = {};
  bulkStart = '';
  bulkEnd = '';
  description = '';
  isLoading = signal(false);
  submitted = false;

  // Recurrence object
  rec = {
    startDate: '',
    endDate: '',
    startTime: '',
    endTime: '',
    days: [false, false, false, false, false, false, false] as boolean[],
  };

  todayString: string = '';

  constructor(private userService: UserManagementService, private holidayService: HolidayService, private notificationService: NotificationService, private router: Router) { }

  async ngOnInit() {
    this.viewDate.setDate(1);
    this.recomputeCalendar();
    const today = new Date();
    this.todayString = today.toISOString().split('T')[0];
    await this.loadUserDropdownData();
  }

  async loadUserDropdownData() {
    this.isLoading.set(true);
    try {
      const [
        users
      ] = await Promise.all([
        firstValueFrom(this.userService.getAllVacationUsers()),
      ]);

      this.users.set(users);

    } catch (error) {
      this.notificationService.showSnackBar('Failed to load user dropdown data', 'failure');
    }
    finally {
      this.isLoading.set(false);
    }
  }

  get monthLabel(): string {
    return this.viewDate.toLocaleString(undefined, { month: 'long', year: 'numeric' });
  }

  changeMonth(delta: number) {
    this.viewDate.setMonth(this.viewDate.getMonth() + delta);
    this.recomputeCalendar();
  }

  recomputeCalendar() {
    const y = this.viewDate.getFullYear();
    const m = this.viewDate.getMonth();
    const firstDow = new Date(y, m, 1).getDay();
    const daysInMonth = new Date(y, m + 1, 0).getDate();
    this.blanks = Array(firstDow).fill(0);
    this.days = Array.from({ length: daysInMonth }, (_, i) => i + 1);
  }

  private toKey(day: number): DateKey {
    const y = this.viewDate.getFullYear();
    const m = this.viewDate.getMonth() + 1;
    return `${y}-${m.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;
  }

  isSelected(day: number): boolean {
    return this.selectedKeys.has(this.toKey(day));
  }

  toggleDate(day: number) {
    const key = this.toKey(day);
    if (this.selectedKeys.has(key)) {
      this.selectedKeys.delete(key);
      delete this.dateTimes[key];
    } else {
      this.selectedKeys.add(key);
      this.dateTimes[key] = { start: '', end: '' };
    }
    if (!this.bulkStart) this.bulkStart = '09:00';
    if (!this.bulkEnd) this.bulkEnd = '17:00';
  }

  get selectedKeysSorted(): DateKey[] {
    return Array.from(this.selectedKeys).sort();
  }

  removeDate(key: DateKey) {
    this.selectedKeys.delete(key);
    delete this.dateTimes[key];
  }

  clearAll() {
    this.selectedKeys.clear();
    this.dateTimes = {};
  }

  formatDate(key: DateKey): string {
    const [y, m, d] = key.split('-').map(Number);
    const localDate = new Date(y, m - 1, d); // force local timezone
    return localDate.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
  }

  applyToAll() {
    for (const key of this.selectedKeys) {
      if (this.bulkStart) this.dateTimes[key].start = this.bulkStart;
      if (this.bulkEnd) this.dateTimes[key].end = this.bulkEnd;
    }
  }

  resetFormAndValues(form: NgForm) {
    form.resetForm({
      leaveTypeSelect: 'custom'
    });

    this.selectedUser = null;
    this.description = '';
    this.selectedKeys.clear();
    this.dateTimes = {};
    this.bulkStart = '';
    this.bulkEnd = '';
    this.rec = {
      startDate: '',
      endDate: '',
      startTime: '',
      endTime: '',
      days: [false, false, false, false, false, false, false]
    };
    this.submitted = false;
    this.viewDate = new Date();
    this.viewDate.setDate(1);
    this.recomputeCalendar();
  }

  async submit(form: NgForm) {
    this.submitted = true;

    if (!this.isFormValid()) {
      this.notificationService.showSnackBar('Please fill all required fields correctly', 'failure');
      return;
    }

    if (!this.selectedUser) {
      this.notificationService.showSnackBar('Please select a user', 'failure');
      return;
    }

    let request: any = {
      userId: this.selectedUser,
      leaveType: this.leaveType === 'custom' ? 1 : 2,
      description: this.description
    };

    if (this.leaveType === 'custom') {
      request.holidayDates = this.selectedKeysSorted.map(key => {
        const times = this.dateTimes[key];
        return {
          date: key,
          startTime: times.start + ':00',
          endTime: times.end + ':00'
        };
      });
    } else if (this.leaveType === 'recurrence') {
      request.recurrence = {
        startDate: this.rec.startDate,
        endDate: this.rec.endDate,
        startTime: this.rec.startTime + ':00',
        endTime: this.rec.endTime + ':00',
        recurrenceDays: this.rec.days
          .map((checked, index) => (checked ? index : null))
          .filter(v => v !== null)
      };
    }

    this.isLoading.set(true);
    try {
      const res = await firstValueFrom(this.holidayService.createHoliday(request));
      this.notificationService.showSnackBar('Holiday created successfully', 'success');
      this.resetFormAndValues(form);
    } catch (err) {
      console.error(err);
      this.notificationService.showSnackBar('Failed to create holiday', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  isFormValid(): boolean {
    if (!this.selectedUser) return false;

    if (this.leaveType === 'custom') {
      if (this.selectedKeysSorted.length === 0) return false;

      for (const key of this.selectedKeysSorted) {
        const times = this.dateTimes[key];
        if (!times.start || !times.end) return false;
        if (times.start >= times.end) {
          return false;
        }
      }
      return true;
    }

    if (this.leaveType === 'recurrence') {
      if (!this.rec.startDate || !this.rec.endDate || !this.rec.startTime || !this.rec.endTime) return false;
      if (this.rec.startDate > this.rec.endDate) return false;
      if (this.rec.startTime >= this.rec.endTime) return false;
      if (!this.rec.days.some(d => d)) return false;
      return true;
    }

    return false;
  }
  isStartDateValid(): boolean {
    return !this.rec.startDate || !this.rec.endDate || this.rec.startDate < this.rec.endDate;
  }

  isStartTimeValid(): boolean {
    return !this.rec.startTime || !this.rec.endTime || this.rec.startTime < this.rec.endTime;
  }

  isDateTimeValid(key: string): boolean {
    const times = this.dateTimes[key];
    return !times.start || !times.end || times.start < times.end;
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }
  hasRecurrenceDays(): boolean {
    return this.rec.days.some(d => d);
  }
  isPastDate(day: number): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const date = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), day);
    if (date < today) return true;
    const dayOfWeek = date.getDay();
    if (dayOfWeek === 0 || dayOfWeek === 6) return true;

    return false;
  }
}
