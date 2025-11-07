import { Component, Inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { DoctorManagementAndMedicalRecommandationService } from '../doctor-management-and-medical-recommandation.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { CounselorNoteDisplay } from '../model/counselor-note-display.model';
import { CounselorNoteRequest } from '../model/counselor-note-request.model';
import { PatientResponseDto } from '../../patient/model/patient-response.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  standalone: true,
  selector: 'app-counselor-notes-add',
  templateUrl: './counselor-notes-add.html',
  styleUrls: ['./counselor-notes-add.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ]
})
export class CounselorNotesAddComponent implements OnInit {
  noteForm!: FormGroup;
  isSubmitting = signal(false);
  isLoading = signal(false);

  patientId: string | null;
  noteData: CounselorNoteDisplay | null;
  isViewMode: boolean;
  private patientDataToLoad: PatientResponseDto | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: { patientId: string | null; noteData: CounselorNoteDisplay | null; isViewMode: boolean },
    private dialogRef: MatDialogRef<CounselorNotesAddComponent>,
    private formBuilder: FormBuilder,
    private readonly doctorManagementAndMedicalRecommandationService: DoctorManagementAndMedicalRecommandationService,
    private readonly notificationService: NotificationService
  ) {
    this.patientId = data?.patientId ?? null;
    this.noteData = data?.noteData ?? null;
    this.isViewMode = data?.isViewMode ?? false;
  }

  ngOnInit(): void {
    this.initializeForm();
    if (this.isViewMode && this.noteData) {
      this.populateFormForView();
    } else {
      this.getPatientById();
    }
  }

  initializeForm(): void {
    this.noteForm = this.formBuilder.group({
      subject: ['', [Validators.required, Validators.maxLength(100),NoWhitespaceValidator]],
      note: ['', [Validators.required, Validators.maxLength(500),NoWhitespaceValidator]],
      sendToAdmin: [false],
      sendToDoctor: [false]
    });

    if (this.isViewMode) {
      this.noteForm.disable();
    }
  }

  populateFormForView(): void {
    if (!this.noteData) return;

    this.noteForm.patchValue({
      subject: this.noteData.subject,
      note: this.noteData.note,
      sendToAdmin: this.noteData.isAdminMailSent,
      sendToDoctor: this.noteData.isDoctorMailSent
    });
  }

  toggleCheckbox(controlName: string): void {
    if (this.isViewMode) return;
    const control = this.noteForm.get(controlName);
    if (control) control.setValue(!control.value);
  }

  getPatientById(): void {
    if (!this.patientId) {
      this.notificationService.showSnackBar('Invalid patient ID.', 'failure');
      return;
    }

    this.isLoading.set(true);
    this.doctorManagementAndMedicalRecommandationService.getPatientById(this.patientId).subscribe({
      next: (response) => {
        this.patientDataToLoad = response;
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error fetching patient:', error);
        const message = error?.error?.message || error?.error || 'Error fetching patient details.';
        this.notificationService.showSnackBar(message, 'failure');
        this.isLoading.set(false);
      }      
    });
  }

  onAddNote(): void {
    if (!this.noteForm.valid || this.isViewMode) {
      this.markFormGroupTouched();
      return;
    }

    this.isSubmitting.set(true);

    const formValue = this.noteForm.value;
    const newNote: CounselorNoteRequest = {
      counselorId: this.patientDataToLoad?.counselorId,
      patientId: this.patientId,
      subject: formValue.subject.trim(),
      note: formValue.note.trim(),
      isAdminMailSent: formValue.sendToAdmin,
      isDoctorMailSent: formValue.sendToDoctor
    };

    this.doctorManagementAndMedicalRecommandationService.createCouncelorNote(newNote).subscribe({
      next: () => {
        this.notificationService.showSnackBar('Counselor note created successfully', 'success');
        this.close();
      },
      error: (error) => {
        console.error('Error creating note:', error);
        const message = error?.error?.message || error?.error || 'Failed to create counselor note';
        this.notificationService.showSnackBar(message, 'failure');
      },
      complete: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  markFormGroupTouched(): void {
    Object.values(this.noteForm.controls).forEach(control => control.markAsTouched());
  }

  getErrorMessage(fieldName: string): string {
    const control = this.noteForm.get(fieldName);
    if (control?.hasError('required')) {
      return `${this.capitalize(fieldName)} is required`;
    }
    if (control?.hasError('maxlength')) {
      return `${this.capitalize(fieldName)} cannot exceed ${control.errors?.['maxlength'].requiredLength} characters`;
    }
    return '';
  }

  capitalize(text: string): string {
    return text.charAt(0).toUpperCase() + text.slice(1);
  }

  getFormTitle(): string {
    return this.isViewMode ? 'View Counselor Note' : 'Add New Counselor Note';
  }

  close(): void {
    this.dialogRef.close();
  }
}
