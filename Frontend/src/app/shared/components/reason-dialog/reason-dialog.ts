import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

interface ReasonDialogData {
  title?: string;
}

@Component({
  selector: 'app-reason-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    CommonModule
  ],
  templateUrl: './reason-dialog.html',
  styleUrl: './reason-dialog.scss'
})
export class ReasonDialogComponent {
  reasonForm: FormGroup;

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ReasonDialogComponent>);

  constructor(@Inject(MAT_DIALOG_DATA) public data: ReasonDialogData) {
    this.reasonForm = this.fb.group({
      reason: ['', Validators.required]
    });
  }

  closeDialog() {
    this.dialogRef.close(null);
  }

  onSave() {
    if (this.reasonForm.invalid) {
      this.reasonForm.markAllAsTouched(); 
      return;
    }
  
    this.dialogRef.close(this.reasonForm.value.reason);
    this.reasonForm.reset();
  }

  isFieldInvalid(field: string) {
    const control = this.reasonForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getFieldError(field: string) {
    const control = this.reasonForm.get(field);
    if (!control || !control.errors) return '';
    return control.errors?.['required'] ? `Reason is required` : 'Invalid value';
  }
}
