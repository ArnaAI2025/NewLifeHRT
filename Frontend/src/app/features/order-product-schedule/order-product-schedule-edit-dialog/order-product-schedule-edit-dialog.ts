// order-product-schedule-edit-dialog.ts
import { CommonModule } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { OverlayContainer } from '@angular/cdk/overlay';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatNativeDateModule, NativeDateAdapter, provideNativeDateAdapter } from '@angular/material/core';


export interface ScheduleEditDialogData {
  scheduleSummary: {
    orderProductScheduleSummaryId: string;
    productName: string;
    protocol: string;
    frequencyType: string;
    startDate: string;
    days: string;
    times: string[];
  };
  scheduleId: string;
}

export interface TimeSlot {
  time: string;
}

@Component({
  selector: 'app-order-product-schedule-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule
  ],
  providers: [
    provideNativeDateAdapter()
  ],
  templateUrl: './order-product-schedule-edit-dialog.html',
  styleUrl: './order-product-schedule-edit-dialog.scss'
})
export class OrderProductScheduleEditDialogComponent implements OnInit, OnDestroy {
  editScheduleForm!: FormGroup;

  // Days options
  daysOfWeek = [
    { value: 'MO', label: 'Monday' },
    { value: 'TU', label: 'Tuesday' },
    { value: 'WE', label: 'Wednesday' },
    { value: 'TH', label: 'Thursday' },
    { value: 'FR', label: 'Friday' },
    { value: 'SA', label: 'Saturday' },
    { value: 'SU', label: 'Sunday' }
  ];

  // Time options
  hours: string[] = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
  minutes: string[] = ['00', '15', '30', '45'];

  originalDaysCount: number = 0;
  noteMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private overlayContainer: OverlayContainer,
    public dialogRef: MatDialogRef<OrderProductScheduleEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ScheduleEditDialogData,

  ) { }

  ngOnInit(): void {
    this.initializeForm();
    this.overlayContainer.getContainerElement().classList.add('edit-dialog-overlay');
    this.editScheduleForm.get('startDate')?.valueChanges.subscribe(() => {
      this.checkStartDateAgainstDays();
    });

    this.editScheduleForm.get('days')?.valueChanges.subscribe(() => {
      this.checkStartDateAgainstDays();
    });
  }

  private checkStartDateAgainstDays(): void {
    const startDate: Date = this.editScheduleForm.get('startDate')?.value;
    const selectedDaysArray = (this.editScheduleForm.get('days') as FormArray).controls
      .map((ctrl, i) => (ctrl.value ? this.daysOfWeek[i].value : null))
      .filter(Boolean) as string[];

    if (!startDate || selectedDaysArray.length === 0) {
      this.noteMessage = null;
      return;
    }
    const dayMap = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'];
    const startDayCode = dayMap[startDate.getDay()];

    if (!selectedDaysArray.includes(startDayCode)) {
      this.noteMessage = 'Start Date day is not one of the selected days, so the schedule calculation will start on the next selected weekday.';
    } else {
      this.noteMessage = null;
    }
  }

  ngOnDestroy(): void {
    this.overlayContainer.getContainerElement().classList.remove('edit-dialog-overlay');
  }

  private initializeForm(): void {
    const schedule = this.data.scheduleSummary;
    const selectedDays = schedule.days
      ? schedule.days.split(',').map(d => d.trim().toUpperCase())
      : [];
    this.originalDaysCount = selectedDays.length;

    const daysControls = this.daysOfWeek.map(day =>
      this.fb.control(selectedDays.includes(day.value))
    );

    const timeSlots = this.createTimeSlotsFromResponse(schedule.times);

    this.editScheduleForm = this.fb.group({
      startDate: [new Date(schedule.startDate), Validators.required],
      days: this.fb.array(daysControls, [this.daysValidator.bind(this)]),
      times: this.fb.array([], [this.timesValidator.bind(this)])
    });

    timeSlots.forEach(timeSlot => {
      this.addTimeSlot(timeSlot);
    });

    this.editScheduleForm.setValidators([this.formValidator.bind(this)]);
  }

  private createTimeSlotsFromResponse(times: string[]): TimeSlot[] {
    return times.map(time => ({
      time: this.convertTo24HourFormat(time)
    }));
  }

  private convertTo24HourFormat(timeString: string): string {
    const match = timeString.match(/(\d{1,2}):(\d{2})\s*(AM|PM)?/i);
    if (!match) return '08:00';

    let hour = parseInt(match[1]);
    const minute = match[2];
    const period = match[3]?.toUpperCase();

    if (period === 'PM' && hour < 12) hour += 12;
    if (period === 'AM' && hour === 12) hour = 0;

    return `${hour.toString().padStart(2, '0')}:${minute}`;
  }

  private parseTimeString(timeString: string): { hour: string; minute: string; period: string } {
    try {
      const cleanTime = timeString.trim().toUpperCase();
      const timeMatch = cleanTime.match(/(\d{1,2}):(\d{2})\s*(AM|PM)?/);

      if (timeMatch) {
        let hour = parseInt(timeMatch[1]);
        const minute = timeMatch[2];
        let period = timeMatch[3] || 'AM';

        if (period === 'PM' && hour > 12) {
          hour -= 12;
        } else if (period === 'AM' && hour === 0) {
          hour = 12;
        }

        return {
          hour: hour.toString().padStart(2, '0'),
          minute: minute,
          period: period
        };
      }
    } catch (error) {
      console.error('Error parsing time:', error);
    }

    return {
      hour: '08',
      minute: '00',
      period: 'AM'
    };
  }

  get daysArray(): FormArray {
    return this.editScheduleForm.get('days') as FormArray;
  }

  get timesArray(): FormArray {
    return this.editScheduleForm.get('times') as FormArray;
  }

  addTimeSlot(timeSlot?: TimeSlot): void {
    const timeGroup = this.fb.group({
      time: [timeSlot?.time || '08:00', Validators.required]
    });
    this.timesArray.push(timeGroup);
  }

  removeTimeSlot(index: number): void {
    this.timesArray.removeAt(index);
  }

  // Custom validators
  private daysValidator(control: AbstractControl): ValidationErrors | null {
    const daysArray = control as FormArray;
    const selectedCount = daysArray.controls.filter(c => c.value).length;

    if (selectedCount !== this.originalDaysCount) {
      return { daysCount: `Please select exactly ${this.originalDaysCount} days` };
    }

    return null;
  }

  private timesValidator(control: AbstractControl): ValidationErrors | null {
    const timesArray = control as FormArray;

    if (timesArray.length === 0) {
      return { noTimes: 'At least one time slot is required' };
    }

    const timeValues = timesArray.controls
      .map(timeGroup => timeGroup.get('time')?.value)
      .filter((t: string | null) => !!t)
      .map(t => t.trim());

    const uniqueTimes = new Set(timeValues);
    if (uniqueTimes.size !== timeValues.length) {
      return { duplicateTimes: 'Duplicate times are not allowed' };
    }

    return null;
  }

  private formValidator(control: AbstractControl): ValidationErrors | null {
    const form = control as FormGroup;

    if (!form.get('startDate')?.valid) {
      return { startDateRequired: 'Start date is required' };
    }

    if (!form.get('days')?.valid) {
      return { daysInvalid: 'Days selection is invalid' };
    }

    if (!form.get('times')?.valid) {
      return { timesInvalid: 'Times are invalid' };
    }

    return null;
  }

  onSave(): void {
    if (this.editScheduleForm.invalid) {
      this.editScheduleForm.markAllAsTouched();
      return;
    }

    const formValue = this.editScheduleForm.value;

    const selectedDays = this.daysOfWeek
      .filter((_, i) => formValue.days[i])
      .map(day => day.value);

    const timesArray = formValue.times.map((t: any) => t.time);

    const requestModel = {
      startDate: this.formatDateToDateOnly(formValue.startDate),
      selectedDays: selectedDays,
      times: timesArray
    };

    this.dialogRef.close({
      success: true,
      scheduleId: this.data.scheduleId,
      request: requestModel
    });
  }

  private formatDateToDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private formatTimeForDisplay(time: string): string {
    const [hourStr, minute] = time.split(':');
    let hour = parseInt(hourStr);
    const period = hour >= 12 ? 'PM' : 'AM';
    if (hour > 12) hour -= 12;
    if (hour === 0) hour = 12;
    return `${hour}:${minute} ${period}`;
  }

  onClose(): void {
    this.dialogRef.close({ success: false });
  }

  getSelectedDaysCount(): number {
    return this.daysArray.controls.filter(control => control.value).length;
  }

  getRequiredDaysCount(): number {
    return this.originalDaysCount;
  }

  disablePastDates = (date: Date | null): boolean => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date ? date >= today : false;
  };
}
