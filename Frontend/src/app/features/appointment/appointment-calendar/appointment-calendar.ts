import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, signal, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from "@angular/common";
import { CalendarDateFormatter, CalendarEvent, CalendarView, CalendarModule, CalendarMonthViewDay } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { firstValueFrom, forkJoin, Subject } from 'rxjs';
import { isSameMonth, isSameDay } from 'date-fns';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

import {
  FormsModule,
  ReactiveFormsModule,
} from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatMenuModule } from "@angular/material/menu";
import { MatSelectModule } from "@angular/material/select";
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { AppointmentService } from '../appointment.services';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { NotificationService } from '../../../shared/services/notification.service';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

enum CustomCalendarView {
  Month = 'month',
  Week = 'week',
  Day = 'day',
  Agenda = 'agenda'
}
export interface DoctorOption {
  id: string | number;
  name: string;
}
@Component({
  selector: 'app-appointment-calendar',
  imports: [
    CalendarModule, DatePipe, CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    FormsModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    FullPageLoaderComponent,
    MatButtonToggleModule,
    MatDividerModule,
    MatDialogModule
  ],
  templateUrl: './appointment-calendar.html',
  styleUrl: './appointment-calendar.scss'
})
export class AppointmentCalendarComponent implements OnInit, OnChanges {

  view = signal(CustomCalendarView.Day);
  CalendarView = CustomCalendarView;  // to use in template
  viewDate: Date = new Date();
  activeBtn: 'prev' | 'today' | 'next' = 'today';
  activeBtnTwo: 'month' | 'week' | 'day' | 'agenda' = 'day';

  events: CustomCalendarEvent[] = [];
  selectedDayEvents: CustomCalendarEvent[] = [];
  refresh: Subject<void> = new Subject();
  activeDayIsOpen: boolean = false;

  @ViewChild('eventActionsTemplate', { static: true })
  eventActionsTemplate!: TemplateRef<any>;

  @ViewChild('dayEventTemplate', { static: true })
  dayEventTemplate!: TemplateRef<any>;
  isLoading = signal(false);
  appointments: CustomCalendarEvent[] = [];
  holidays: CustomCalendarEvent[] = [];
  @ViewChild('eventDetailModal') eventDetailModal!: TemplateRef<any>;
  lastDesktopView: CustomCalendarView = CustomCalendarView.Day;
  wasAgendaOnDesktop: boolean = false;
  @Output() doctorsChange = new EventEmitter<DoctorOption[]>();
  @Input() selectedDoctorIds: (string | number)[] = [];
  @Input() filterActive = false;

  constructor(private appointmentService: AppointmentService,
    private router: Router,
    private confirmationService: ConfirmationDialogService,
    private notificationService: NotificationService, private breakpointObserver: BreakpointObserver, private dialog: MatDialog) { }

  ngOnInit() {
    this.loadData();
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe(result => {
      if (result.matches) {
        if (this.view() === CustomCalendarView.Agenda) {
          this.wasAgendaOnDesktop = true;
        } else {
          this.wasAgendaOnDesktop = false;
          this.lastDesktopView = this.view();
        }
        this.setView(CustomCalendarView.Agenda);
      } else {
        if (this.wasAgendaOnDesktop) {
          this.setView(CustomCalendarView.Agenda);
        } else {
          this.setView(this.lastDesktopView);
        }
      }
    });
  }

  loadData() {
    const { start, end } = this.getStartEndRange();

    forkJoin({
      appointments: this.appointmentService.getAppointments({
        startDate: this.formatDateForApi(start),
        endDate: this.formatDateForApi(end),
      }),
      holidays: this.appointmentService.getAllHolidays({
        startDate: this.formatDateForApi(start),
        endDate: this.formatDateForApi(end),
      })
    }).subscribe(({ appointments, holidays }) => {
      this.appointments = appointments.map(a => ({
        start: new Date(a.startDateTime),
        end: new Date(a.endDateTime),
        title: a.title,
        color: { primary: a.colorCode ?? '#1e90ff', secondary: a.colorCode ?? '#D1E8FF', secondaryText: this.getContrastColor(a.colorCode ?? '#1e90ff') },
        patientId: a.patientId,
        appointmentId: a.appointmentId,
        patientName: a.patientName,
        doctorName: a.doctorName,
        serviceName: a.serviceName,
        modeName: a.modeName,
        counselorName: a.counselorName,
        isHoliday: false,
        doctorStart: a.doctorStartDateTime,
        doctorEnd: a.doctorEndDateTime,
        doctorId: a.doctorId
      }));

      this.holidays = holidays.map(h => ({
        start: new Date(h.startDateTime),
        end: new Date(h.endDateTime),
        title: `Holiday - ${h.fullName}${h.description ? ' - ' + h.description : ''}`,
        color: { primary: h.colorCode ?? '#FF6347', secondary: h.colorCode ?? '#FFCCCB', secondaryText: this.getContrastColor(h.colorCode ?? '#1e90ff') },
        isHoliday: true,
        userId: h.userId
      }));

      const map = new Map<string | number, string>();
      for (const a of this.appointments) {
        if (a.doctorId != null && a.doctorName) {
          map.set(a.doctorId, a.doctorName);
        }
      }
      for (const h of this.holidays) {
        if (h.userId != null && h.fullName) {
          map.set(h.userId, h.fullName);
        }
      }
      const options: DoctorOption[] = Array.from(map.entries())
        .map(([id, name]) => ({ id, name }))
        .sort((x, y) => x.name.localeCompare(y.name));

      this.doctorsChange.emit(options);
      this.mergeEvents();
    });
  }

  ngOnChanges(): void {
    console.log('aaaaaaaaaaaaa')
    this.mergeEvents();
  }
  // --- Navigation ---
  clickPrevMonth() {
    this.activeBtn = 'prev';
    this.prevMonth();
  }

  clickToday() {
    this.activeBtn = 'today';
    this.today();
  }

  clickNextMonth() {
    this.activeBtn = 'next';
    this.nextMonth();
  }

  setView(view: CustomCalendarView) {
    this.activeBtnTwo = view;
    this.view.set(view);
    this.loadData();
  }

  prevMonth() {
    const date = new Date(this.viewDate);
    if (this.view() === CustomCalendarView.Month) {
      date.setMonth(date.getMonth() - 1);
    } else if (this.view() === CustomCalendarView.Week) {
      date.setDate(date.getDate() - 7);
    } else if (this.view() === CustomCalendarView.Day) {
      do {
        date.setDate(date.getDate() - 1);
      } while (this.isWeekend(date));
    } else if (this.view() === CustomCalendarView.Agenda) {
      date.setDate(date.getDate() - 7);
    }
    this.viewDate = date;
    this.loadData();
  }

  nextMonth() {
    const date = new Date(this.viewDate);
    if (this.view() === CustomCalendarView.Month) {
      date.setMonth(date.getMonth() + 1);
    } else if (this.view() === CustomCalendarView.Week) {
      date.setDate(date.getDate() + 7);
    } else if (this.view() === CustomCalendarView.Day) {
      do {
        date.setDate(date.getDate() + 1);
      } while (this.isWeekend(date));
    } else if (this.view() === CustomCalendarView.Agenda) {
      date.setDate(date.getDate() + 7);
    }
    this.viewDate = date;
    this.loadData();
  }

  today() {
    this.viewDate = new Date();
    this.loadData();
  }

  getAgendaDays(): Date[] {
    const days: Date[] = [];
    let current = new Date(this.viewDate);
    const day = current.getDay();
    const diffToMonday = day === 0 ? -6 : 1 - day;
    current.setDate(current.getDate() + diffToMonday);

    for (let i = 0; i < 7 && days.length < 5; i++) {
      if (!this.isWeekend(current) && this.getEventsForDay(current).length > 0) {
        days.push(new Date(current));
      }
      current.setDate(current.getDate() + 1);
    }

    return days;
  }

  getEventsForDay(day: Date): CustomCalendarEvent[] {
    return this.events.filter(
      e =>
        e.start.toDateString() === day.toDateString() ||
        (e.end && e.end.toDateString() === day.toDateString())
    );
  }

  onDayClick({ day }: { day: CalendarMonthViewDay }): void {
    if (isSameMonth(day.date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, day.date) && this.activeDayIsOpen === true) ||
        day.events.length === 0
      ) {
        this.activeDayIsOpen = false;
        this.selectedDayEvents = [];
      } else {
        this.activeDayIsOpen = true;
        this.selectedDayEvents = day.events;
      }
      this.viewDate = day.date;
    }
  }

  onEventClick(event: CustomCalendarEvent): void {
    if (event.isHoliday) return;
    const patientId = (event as any).patientId;
    window.open(`/patient/edit/${patientId}`, '_blank');
  }

  onEditEvent(event: CustomCalendarEvent): void {
    if (event.isHoliday) return;
    this.dialog.closeAll();
    const appointmentId = (event as any).appointmentId;
    this.router.navigateByUrl(`/appointment/edit/${appointmentId}`);
  }

  async onDeleteEvent(event: CustomCalendarEvent): Promise<void> {
    if (event.isHoliday) return;
    this.dialog.closeAll();
    const confirmed = await this.openConfirmationDialog('delete');
    if (!confirmed) return;

    const appointmentId = (event as any)?.appointmentId;
    if (!appointmentId) {
      console.error('Appointment ID is missing in event:', event);
      this.notificationService.showSnackBar('Invalid appointment data.', 'failure');
      return;
    }

    this.isLoading.set(true);

    try {
      await firstValueFrom(this.appointmentService.deleteAppointment(appointmentId));
      this.notificationService.showSnackBar('Appointment deleted successfully!', 'success');
      this.loadData();
      this.activeDayIsOpen = false;
    } catch (error) {
      console.error('Failed to delete appointment:', error);
      this.notificationService.showSnackBar('Failed to delete appointment.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }


  async openConfirmationDialog(action: string): Promise<boolean> {

    const data: ConfirmationDialogData = {
      title: `${action.charAt(0).toUpperCase() + action.slice(1)} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action}</strong> this appointment?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const result = await this.confirmationService.openConfirmation(data).toPromise();
    return result ?? false;

  }

  private mergeEvents() {
    if (!this.filterActive) {
      this.events = [...this.appointments, ...this.holidays];
    } else if (this.selectedDoctorIds.length === 0) {
      this.events = [];
    } else {
      const sel = new Set(this.selectedDoctorIds);
      const appts = this.appointments.filter(a => a.doctorId != null && sel.has(a.doctorId));
      const hols = this.holidays.filter(h => h.userId != null && sel.has(h.userId));
      this.events = [...appts, ...hols];
    }
    this.refresh.next();
  }

  private getStartEndRange(): { start: Date; end: Date } {
    let start: Date;
    let end: Date;

    switch (this.view()) {
      case CustomCalendarView.Month:
        // Full month
        start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), 1);
        end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 0);
        break;

      case CustomCalendarView.Week:
        const day = this.viewDate.getDay();
        const diffToMonday = day === 0 ? -6 : 1 - day;
        start = new Date(this.viewDate);
        start.setDate(this.viewDate.getDate() + diffToMonday);
        start.setHours(0, 0, 0, 0);

        end = new Date(start);
        end.setDate(start.getDate() + 6);
        end.setHours(23, 59, 59, 999);
        break;

      case CustomCalendarView.Day:
        // Single day
        start = new Date(this.viewDate);
        start.setHours(0, 0, 0, 0);

        end = new Date(this.viewDate);
        end.setHours(23, 59, 59, 999);
        break;

      case CustomCalendarView.Agenda:
        const dayAgenda = this.viewDate.getDay();
        const diffToMondayAgenda = dayAgenda === 0 ? -6 : 1 - dayAgenda;

        start = new Date(this.viewDate);
        start.setDate(this.viewDate.getDate() + diffToMondayAgenda);
        start.setHours(0, 0, 0, 0);

        end = new Date(start);
        end.setDate(start.getDate() + 6);
        end.setHours(23, 59, 59, 999);
        break;

      default:
        // fallback: full month
        start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), 1);
        end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 0);
        break;
    }

    return { start, end };
  }

  private formatDateForApi(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  isPastEvent(event: any): boolean {
    const eventDate = event.doctorStart ? new Date(event.doctorStart) : new Date(event.start);
    const now = new Date();
    const eventDateOnly = new Date(eventDate.getFullYear(), eventDate.getMonth(), eventDate.getDate());
    const nowDateOnly = new Date(now.getFullYear(), now.getMonth(), now.getDate());

    return eventDateOnly < nowDateOnly;
  }

  isWeekend(day: Date): boolean {
    const dayOfWeek = day.getDay();
    return dayOfWeek === 0 || dayOfWeek === 6;
  }

  onDailyWeeklyEventClick(event: CustomCalendarEvent): void {
    if (event.isHoliday) return;

    this.dialog.open(this.eventDetailModal, {
      width: '400px',
      data: event
    });
  }

  onPatientClick(event: CustomCalendarEvent): void {
    if (event.isHoliday) return;
    this.dialog.closeAll();
    const patientId = (event as any).patientId;
    if (patientId) {
      window.open(`/patient/edit/${patientId}`, '_blank');
    }
  }

  getContrastColor(hexColor: string): string {
    hexColor = hexColor.replace('#', '');
    const r = parseInt(hexColor.substr(0, 2), 16);
    const g = parseInt(hexColor.substr(2, 2), 16);
    const b = parseInt(hexColor.substr(4, 2), 16);

    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    return luminance > 0.5 ? '#000000' : '#FFFFFF';
  }

}

interface CustomCalendarEvent extends CalendarEvent {
  isHoliday?: boolean;
  doctorId?: number;
  doctorName?: string;
  userId?: string;
  fullName?: string
}
