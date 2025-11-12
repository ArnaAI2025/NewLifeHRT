import { Component, OnInit, signal, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { CalendarModule, CalendarView, CalendarEvent, CalendarMonthViewDay } from 'angular-calendar';
import { Subject, firstValueFrom, forkJoin } from 'rxjs';
import { isSameMonth, isSameDay } from 'date-fns';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { OrderproductScheduleCalendarService } from '../order-product-schedule-calendar.services';
import { OrderProductScheduleFilterRequestDto } from '../model/order-product-schedule-filter-request.model';
import { OrderProductScheduleResponseDto } from '../model/order-product-schedule-filter-response.model';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { NotificationService } from '../../../shared/services/notification.service';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AppSettingsService } from '../../../shared/services/app-settings.service';
import { AppointmentService } from '../../appointment/appointment.services';
import { AppointmentGetByPatientIdResponseDto } from '../../appointment/model/appointment-get-by-patientId-response.model';

enum CustomCalendarView {
  Month = 'month',
  Week = 'week',
  Day = 'day',
  Agenda = 'agenda'
}

interface OrderScheduleEvent extends CalendarEvent {
  scheduleId?: string;
  originalTime?: string;
  isAppointment?: boolean;
  appointmentId?: string;
}

@Component({
  selector: 'app-order-product-schedule-calendar',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    CalendarModule,
    MatButtonModule,
    MatIconModule,
    MatButtonToggleModule,
    FullPageLoaderComponent,
    MatDividerModule
  ],
  templateUrl: './order-product-schedule-calendar.html',
  styleUrl: './order-product-schedule-calendar.scss'
})
export class OrderProductScheduleCalendarComponent implements OnInit {

  view = signal(CustomCalendarView.Day);
  CalendarView = CustomCalendarView;
  viewDate: Date = new Date();
  activeDayIsOpen = false;
  refresh: Subject<void> = new Subject();
  isLoading = signal(false);
  activeBtnTwo: 'month' | 'week' | 'day' | 'agenda' = 'day';
  lastDesktopView: CustomCalendarView = CustomCalendarView.Day;
  wasAgendaOnDesktop: boolean = false;

  events: OrderScheduleEvent[] = [];

  @ViewChild('eventActionsTemplate', { static: true })
  eventActionsTemplate!: TemplateRef<any>;

  constructor(
    private orderProductScheduleService: OrderproductScheduleCalendarService,
    private breakpointObserver: BreakpointObserver,
    private notificationService: NotificationService,
    private dialog: MatDialog,
    private router: Router,
    private appsettingService: AppSettingsService,
    private appointmentService: AppointmentService
  ) { }

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

  async loadData() {
    this.isLoading.set(true);
    const { start, end } = this.getStartEndRange();

    const filterRequest: OrderProductScheduleFilterRequestDto = {
      startDate: this.formatDateForApi(start),
      endDate: this.formatDateForApi(end)
    };

    try {
      const patientId = this.appsettingService.getPatientUserId();

      if (!patientId) {
        this.notificationService.showSnackBar('Patient ID not found. Please login again.', 'failure');
        this.isLoading.set(false);
        return;
      }

      const [scheduleResponse, appointmentResponse, reminderResponse] = await firstValueFrom(
        forkJoin([
          this.orderProductScheduleService.getAllScheduleOfOrderProductsForLoggedInPatient(filterRequest),
          this.appointmentService.getAppointmentsByPatientId(patientId),
          this.orderProductScheduleService.getPatientSelfReminders(filterRequest)
        ])
      );

      let scheduleEvents: OrderScheduleEvent[] = [];
      if (scheduleResponse && scheduleResponse.isPatient === false) {
        this.notificationService.showSnackBar(scheduleResponse.message || 'The logged-in user is not a patient.', 'failure');
      } else {
        const data = scheduleResponse.schedules || scheduleResponse;
        const datePipe = new DatePipe('en-US');
        scheduleEvents = data.map((item: OrderProductScheduleResponseDto) => {
          const startDate = new Date(item.occurrenceDateAndTime!);
          const formattedTime = datePipe.transform(startDate, 'h:mm a');

          return {
            start: startDate,
            end: startDate,
            title: `${item.productName}${item.protocol ? ' - ' + item.protocol : ''} (${formattedTime})`,
            color: { primary: '#1e90ff', secondary: '#D1E8FF' },
            scheduleId: item.orderProductScheduleId,
            originalTime: formattedTime,
            isAppointment: false
          };
        });
      }

      const appointmentEvents: OrderScheduleEvent[] = this.mapAppointmentsToEvents(appointmentResponse || []);

      const reminderEvents: OrderScheduleEvent[] = (reminderResponse?.reminders || []).map((reminder: any) => {
      const startDate = new Date(reminder.reminderDateTime);
      const formattedTime = new DatePipe('en-US').transform(startDate, 'h:mm a');
      return {
        start: startDate,
        end: startDate,
        title: `${reminder.description} (${formattedTime})`,
        color: { primary: '#008000', secondary: '#CFFFCF' },
        isAppointment: false
      };
    });

      // Combine both event arrays
      this.events = [...scheduleEvents, ...appointmentEvents, ...reminderEvents];
      this.refresh.next();

    } catch (error) {
      console.error('Error loading schedule and appointments:', error);
      this.notificationService.showSnackBar('Failed to load data. Please try again later.', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  private mapAppointmentsToEvents(appointments: AppointmentGetByPatientIdResponseDto[]): OrderScheduleEvent[] {
    const datePipe = new DatePipe('en-US');

    return appointments.map((appointment: AppointmentGetByPatientIdResponseDto) => {
      const startDate = new Date(appointment.utcStartDateTime);
      const endDate = new Date(appointment.utcEndDateTime);
      const doctorStartDateTime = appointment.doctorStartDateTime;
      const doctorEndDateTime = appointment.doctorEndDateTime;

      const startTime = datePipe.transform(startDate, 'hh:mm a');
      const endTime = datePipe.transform(endDate, 'hh:mm a');
      const doctorStartTime = datePipe.transform(doctorStartDateTime, 'hh:mm a');
      const doctorEndTime = datePipe.transform(doctorEndDateTime, 'hh:mm a');
      const patientName = appointment.patientName;
      const title = `${patientName} - ${appointment.doctorName} - ${appointment.serviceName} - ${doctorStartTime} to ${doctorEndTime} EST`;

      return {
        start: startDate,
        end: endDate,
        title: title,
        color: { primary: '#FFA500', secondary: '#FFE5B4' },
        appointmentId: appointment.appointmentId,
        isAppointment: true
      };
    });
  }

  isPastEvent(event: OrderScheduleEvent): boolean {
    const eventDate = new Date(event.start);
    const now = new Date();
    const eventDateOnly = new Date(eventDate.getFullYear(), eventDate.getMonth(), eventDate.getDate());
    const nowDateOnly = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    return eventDateOnly < nowDateOnly;
  }

  private extractTimeFromTitle(title: string): string {
    const timeMatch = title.match(/\(([^)]+)\)/);
    return timeMatch ? timeMatch[1] : '8:00 AM';
  }

  clickPrevMonth() {
    this.shiftView(-1);
  }
  clickToday() {
    this.viewDate = new Date();
    this.loadData();
  }
  clickNextMonth() {
    this.shiftView(1);
  }

  setView(view: CustomCalendarView) {
    this.activeBtnTwo = view;
    this.view.set(view);
    this.loadData();
  }

  shiftView(offset: number) {
    const date = new Date(this.viewDate);

    if (this.view() === CustomCalendarView.Month) {
      date.setMonth(date.getMonth() + offset);
      date.setDate(1);
    } else if (this.view() === CustomCalendarView.Week || this.view() === CustomCalendarView.Agenda) {
      date.setDate(date.getDate() + (7 * offset));
    } else {
      date.setDate(date.getDate() + offset);
    }

    this.viewDate = date;
    this.loadData();
  }

  getAgendaDays(): Date[] {
    const days: Date[] = [];
    const start = new Date(this.viewDate);
    start.setDate(start.getDate() - start.getDay());
    for (let i = 0; i < 7; i++) {
      days.push(new Date(start));
      start.setDate(start.getDate() + 1);
    }
    return days;
  }

  getEventsForDay(day: Date): OrderScheduleEvent[] {
    return this.events.filter(
      e => e.start.toDateString() === day.toDateString()
    );
  }

  onDayClick({ day }: { day: CalendarMonthViewDay }) {
    if (isSameMonth(day.date, this.viewDate)) {
      this.activeDayIsOpen = !(
        isSameDay(this.viewDate, day.date) &&
        this.activeDayIsOpen
      );
      this.viewDate = day.date;
    }
  }
  private getStartEndRange(): { start: Date; end: Date } {
    let start: Date;
    let end: Date;

    switch (this.view()) {
      case CustomCalendarView.Month:
        start = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), 1);
        end = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 0);
        break;

      case CustomCalendarView.Week:
      case CustomCalendarView.Agenda: {
        const currentDay = this.viewDate.getDay();
        start = new Date(this.viewDate);
        start.setDate(this.viewDate.getDate() - currentDay);
        end = new Date(start);
        end.setDate(start.getDate() + 6);
        break;
      }

      default:
        start = new Date(this.viewDate);
        end = new Date(this.viewDate);
        break;
    }
    return { start, end };
  }

  private formatDateForApi(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  getAgendaHeaderRange(): string {
    const { start, end } = this.getStartEndRange();
    const datePipe = new DatePipe('en');
    const startStr = datePipe.transform(start, 'MMM d');
    const endStr = datePipe.transform(end, 'MMM d, y');
    return `${startStr} - ${endStr}`;
  }

  goBack() {
    this.router.navigate(['/order-product-schedule/view']);
  }
}
