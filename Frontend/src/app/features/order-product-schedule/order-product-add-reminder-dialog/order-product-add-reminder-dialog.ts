import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule, provideNativeDateAdapter } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { firstValueFrom } from 'rxjs';
import { OrderproductScheduleCalendarService } from '../order-product-schedule-calendar.services';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-order-product-add-reminder-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatIconModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './order-product-add-reminder-dialog.html',
})
export class OrderProductAddReminderDialogComponent {
  reminderForm: FormGroup;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private orderProductScheduleService: OrderproductScheduleCalendarService,
    private notificationService: NotificationService,
    public dialogRef: MatDialogRef<OrderProductAddReminderDialogComponent>
  ) {
    this.reminderForm = this.fb.group({
      date: [null, Validators.required],
      time: [null, Validators.required],
      description: ['', Validators.required]
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  async onSave(): Promise<void> {
    if (this.reminderForm.invalid) return;

    this.isSaving = true;
    const { date, time, description } = this.reminderForm.value;

    const reminderDate = new Date(date);
    const [hours, minutes] = time.split(':').map(Number);
    reminderDate.setHours(hours);
    reminderDate.setMinutes(minutes);

    // 👇 construct string manually in local time (no UTC shift)
    const reminderDateTime = `${reminderDate.getFullYear()}-${(reminderDate.getMonth() + 1)
      .toString()
      .padStart(2, '0')}-${reminderDate
        .getDate()
        .toString()
        .padStart(2, '0')}T${time}:00`;

    const request = {
      reminderDateTime,
      description
    };

    try {
      const response = await firstValueFrom(
        this.orderProductScheduleService.createPatientSelfReminder(request)
      );

      if (response.success) {
        this.notificationService.showSnackBar('Reminder added successfully!', 'success');
        this.dialogRef.close({ success: true, reminder: request });
      } else {
        this.notificationService.showSnackBar(response.message || 'Failed to add reminder.', 'failure');
      }
    } catch (error) {
      console.error('Error creating reminder:', error);
      this.notificationService.showSnackBar('Error creating reminder.', 'failure');
    } finally {
      this.isSaving = false;
    }
  }

  disablePastDates = (date: Date | null): boolean => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date ? date >= today : false;
  };
}
