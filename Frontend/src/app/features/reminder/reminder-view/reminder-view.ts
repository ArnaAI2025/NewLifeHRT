import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { AppointmentGetByPatientIdResponseDto } from '../../appointment/model/appointment-get-by-patientId-response.model';
import { SelectionModel } from '@angular/cdk/collections';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { AppointmentService } from '../../appointment/appointment.services';
import { LeadNavigationBar } from '../../lead-management/lead-navigation-bar/lead-navigation-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { ReminderService } from '../reminder.services';
import { ReminderTypesResponseDto } from '../model/get-all-reminder-types.response.model';
import { RecurrenceRulesResponseDto } from '../model/get-all-recurrence-rule.response.model';
import { forkJoin } from 'rxjs';
import { CreateReminderRequestDto } from '../model/create-reminder.request.model';
import { NgxMatTimepickerModule } from 'ngx-mat-timepicker';

@Component({
  selector: 'app-reminder-view',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    PatientNavigationBarComponent,
    LeadNavigationBar,
    FullPageLoaderComponent,
    NgxMatTimepickerModule
  ],
  templateUrl: './reminder-view.html',
  styleUrl: './reminder-view.scss'
})
export class ReminderViewComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  patientId: string | null = null;
  leadId: string | null = null;
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private reminderService = inject(ReminderService);
  private fb = inject(FormBuilder);
  private notificationService = inject(NotificationService);

  searchKeyword = '';
  isLoading = signal(false);
  reminderForm!: FormGroup;
  reminderTypes = signal<ReminderTypesResponseDto[]>([]);
  recurrenceRules = signal<RecurrenceRulesResponseDto[]>([]);

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('patientId');
    this.leadId = this.route.snapshot.paramMap.get('leadId');

    this.initializeForm();
    this.loadInitialData();

    this.reminderForm.get('isRecurring')?.valueChanges.subscribe(isRecurring => {
      const recurrenceRuleControl = this.reminderForm.get('recurrenceRuleId');
      const recurrenceEndControl = this.reminderForm.get('recurrenceEndDateTime');

      if (isRecurring) {
        recurrenceRuleControl?.setValidators([Validators.required]);
        recurrenceEndControl?.setValidators([Validators.required]);
      } else {
        recurrenceRuleControl?.clearValidators();
        recurrenceRuleControl?.setValue(null);

        recurrenceEndControl?.clearValidators();
        recurrenceEndControl?.setValue(null);
      }

      recurrenceRuleControl?.updateValueAndValidity();
      recurrenceEndControl?.updateValueAndValidity();
    });
    this.reminderForm.get('reminderDate')?.valueChanges.subscribe(() => this.updateCombinedDateTime());
    this.reminderForm.get('reminderTime')?.valueChanges.subscribe(() => this.updateCombinedDateTime());
  }

  initializeForm(): void {
    this.reminderForm = this.fb.group({
      reminderDate: [null, [Validators.required]],
      reminderTime: [null, [Validators.required]],
      reminderDateTime: [null, [Validators.required]],
      reminderTypeId: [null, [Validators.required]],
      isRecurring: [false],
      recurrenceRuleId: [null],
      recurrenceEndDateTime: [null],
      description: ['']
    });
    this.updateCombinedDateTime();
  }

  formatTime(date: Date): string {
    let hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')} ${ampm}`;
  }

  onDateSelected(): void {
    this.updateCombinedDateTime();
  }

  onTimeSelected(): void {
    this.updateCombinedDateTime();
  }

  updateCombinedDateTime(): void {
    const date: Date = this.reminderForm.get('reminderDate')?.value;
    const time: string = this.reminderForm.get('reminderTime')?.value;

    if (date && time) {
      const combinedDateTime = this.combineDateAndTime(date, time);
      if (!isNaN(combinedDateTime.getTime())) {
        this.reminderForm.patchValue({
          reminderDateTime: combinedDateTime
        }, { emitEvent: false });
      }
    }
  }

  private combineDateAndTime(date: Date, time: string): Date {
    const combined = new Date(date);
    if (!time) return combined;

    let hours: number, minutes: number;
    const timeParts = time.match(/(\d+):(\d+)\s*(AM|PM)?/i);
    if (!timeParts) return new Date(NaN);

    hours = parseInt(timeParts[1], 10);
    minutes = parseInt(timeParts[2], 10);
    const meridiem = timeParts[3]?.toUpperCase();

    if (meridiem === 'PM' && hours < 12) hours += 12;
    if (meridiem === 'AM' && hours === 12) hours = 0;

    combined.setHours(hours, minutes, 0, 0);
    return combined;
  }

  loadInitialData(): void {
    this.isLoading.set(true);

    forkJoin({
      types: this.reminderService.getAllReminderTypes(),
      rules: this.reminderService.getAllRecurrenceRules()
    }).subscribe({
      next: (res) => {
        this.reminderTypes.set(res.types);
        this.recurrenceRules.set(res.rules);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading initial data:', err);
        this.notificationService.showSnackBar('Failed to load reminder data', 'failure');
        this.isLoading.set(false);
      }
    });
  }

  onSubmitReminder(): void {
    if (this.reminderForm.invalid) {
      this.reminderForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const formValue = this.reminderForm.value;
    const reminderDateTime = this.reminderForm.value.reminderDateTime;
    const recurrenceEnd = this.reminderForm.value.recurrenceEndDateTime;

    const request: CreateReminderRequestDto = {
      reminderDateTime: this.formatDateTime(reminderDateTime)!,
      reminderTypeId: formValue.reminderTypeId,
      description: formValue.description,
      isRecurring: formValue.isRecurring,
      recurrenceRuleId: formValue.isRecurring ? formValue.recurrenceRuleId : null,
      recurrenceEndDateTime: formValue.isRecurring ? this.formatDateTime(recurrenceEnd) : null,
      leadId: this.leadId || undefined,
      patientId: this.patientId || undefined
    };

    this.reminderService.createReminder(request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.notificationService.showSnackBar('Reminder created successfully!', 'success');
        this.onClose();
      },
      error: (err) => {
        this.isLoading.set(false);
        console.error('Error creating reminder:', err);
        this.notificationService.showSnackBar(
          err?.error?.message || 'Failed to create reminder',
          'failure'
        );
      }
    });
  }

  private formatDateTime(date: Date | null): string | null {
    if (!date || isNaN(date.getTime())) return null;

    const pad = (n: number) => n.toString().padStart(2, '0');
    const yyyy = date.getFullYear();
    const MM = pad(date.getMonth() + 1);
    const dd = pad(date.getDate());
    const hh = pad(date.getHours());
    const mm = pad(date.getMinutes());
    const ss = pad(date.getSeconds());

    return `${yyyy}-${MM}-${dd}T${hh}:${mm}:${ss}`;
  }

  togglePatientActiveStatus(status: boolean): void {
    console.log('Patient active status toggled:', status);
  }

  onSaveAndClose(): void {
    console.log('Save and Close clicked');
  }

  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }

  toggleLeadActiveStatus(event: Event): void {
    const status = (event.target as HTMLInputElement).checked;
    console.log('Lead active status toggled:', status);
  }

  onClickAddLead(): void {
    this.router.navigate(['/lead-management/add']);
  }

  onSubmit(): void {
    console.log('Submit clicked');
  }

  onClose(): void {
    if (this.patientId) {
      this.router.navigate(['/patients/view']);
    } else if (this.leadId) {
      this.router.navigate(['/lead-management/view']);
    } else {
      this.router.navigate(['/']);
    }
  }
}
