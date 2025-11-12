import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';


export interface RefundAmountDialogData {
  orderId: string;
  patientId: string;
  currentRefundAmount?: number;
}

@Component({
  selector: 'app-refund-amount-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './refund-amount-dialog.html',
  styleUrl: './refund-amount-dialog.scss'
})
export class RefundAmountDialogComponent implements OnInit{
  refundAmountForm: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<RefundAmountDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RefundAmountDialogData
  ) {
    this.refundAmountForm = this.fb.group({
      refundAmount: [[Validators.required]]
    });
  }

  ngOnInit(): void {
    if (this.data.currentRefundAmount) {
      this.refundAmountForm.patchValue({
        refundAmount: this.data.currentRefundAmount
      });
    }
  }

  onSave(): void {
    if (this.refundAmountForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const refundAmount = this.refundAmountForm.value.refundAmount;

      this.dialogRef.close(refundAmount);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  get refundAmountControl() {
    return this.refundAmountForm.get('refundAmount');
  }
}
