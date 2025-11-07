import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';


export interface SettleUpDialogData {
  orderId: string;
  patientId: string;
  outstandingRefundBalance: number;
  maxSettleAmount: number;
  orderGrandTotal: number;
}

@Component({
  selector: 'app-settle-up-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './settle-up-dialog.html',
  styleUrl: './settle-up-dialog.scss'
})
export class SettleUpDialogComponent implements OnInit {
  settleUpForm: FormGroup;
  isSubmitting = false;
  actualMaxSettleAmount: number = 0;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<SettleUpDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SettleUpDialogData
  ) {
    this.settleUpForm = this.fb.group({
      settleAmount: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    // Calculate the actual maximum settle amount based on the logic
    this.actualMaxSettleAmount = this.calculateActualMaxSettleAmount();

    // Set validators
    const validators = [Validators.required, Validators.min(0.01), Validators.max(this.actualMaxSettleAmount)];

    this.settleUpForm.get('settleAmount')?.setValidators(validators);
    this.settleUpForm.get('settleAmount')?.updateValueAndValidity();
  }

  private calculateActualMaxSettleAmount(): number {
    const absoluteOutstandingBalance = Math.abs(this.data.outstandingRefundBalance);

    if (this.data.outstandingRefundBalance >= 0) {
      // For positive outstanding balance:
      // Settle amount cannot exceed both outstanding balance AND order grand total
      return Math.min(absoluteOutstandingBalance, this.data.orderGrandTotal);
    } else {
      // For negative outstanding balance (Amount retains from Patient):
      // Settle amount cannot exceed outstanding balance only
      return absoluteOutstandingBalance;
    }
  }

  onSave(): void {
    if (this.settleUpForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const settleAmount = this.settleUpForm.value.settleAmount;

      this.dialogRef.close({ settleAmount });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  get settleAmountControl() {
    return this.settleUpForm.get('settleAmount');
  }

  getMaxSettleAmount(): number {
    return this.actualMaxSettleAmount;
  }

  getAbsoluteOutstandingBalance(): number {
    return Math.abs(this.data.outstandingRefundBalance);
  }

  getOutstandingBalanceDisplayString(): string {
    return this.data.outstandingRefundBalance < 0 ? 'Amount retains from Patient' : 'Outstanding Balance';
  }

  get popupTitle(): string {
    return this.data.outstandingRefundBalance < 0
      ? 'Settle Outstanding Payment'
      : 'Settle Outstanding Refund';
  }

  get balanceLabel(): string {
    return this.data.outstandingRefundBalance < 0
      ? 'Outstanding Payment Balance:'
      : 'Refund Balance:';
  }

  get instructionText(): string {
    const amount = this.getMaxSettleAmount().toFixed(2);
    return this.data.outstandingRefundBalance < 0
      ? `You may collect up to $${amount} from the customer.`
      : `You are required to refund up to $${amount} to the customer.`;
  }

  get fieldLabel(): string {
    return this.data.outstandingRefundBalance < 0
      ? 'Payment Amount'
      : 'Refund Amount';
  }


}
