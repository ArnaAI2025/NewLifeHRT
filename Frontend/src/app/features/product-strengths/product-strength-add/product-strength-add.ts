import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { ProductStrengthService } from '../product-strength.services';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ProductStrengthCreateRequestDto } from '../model/product-strength-create-request.model';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../../../shared/services/notification.service';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  selector: 'app-product-strength-add',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './product-strength-add.html',
  styleUrl: './product-strength-add.scss'
})
export class ProductStrengthAddComponent {
  form!: FormGroup;
  isEditMode = false;
  recordId?: string;

  constructor(
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private dialogRef: MatDialogRef<ProductStrengthAddComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { productId: string, productStrength?: any },
    private productStrengthService: ProductStrengthService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required,NoWhitespaceValidator]],
      strengths: [''],
      price: [null]
    });

    if (data.productStrength) {
      this.isEditMode = true;
      this.recordId = data.productStrength.id;
      this.form.patchValue({
        name: data.productStrength.name,
        strengths: data.productStrength.strengths,
        price: data.productStrength.price
      });
    }
  }

async save(): Promise<void> {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    this.notificationService.showSnackBar('Please fill all required fields', 'normal');
    return;
  }

  const formData = this.form.getRawValue();

  const payload: ProductStrengthCreateRequestDto = {
    productID: this.data.productId,
    name: formData.name.trim(),
    strengths: formData.strengths || null,
    price: formData.price ?? null
  };

  try {
    if (this.isEditMode && this.recordId) {
      await firstValueFrom(
        this.productStrengthService.updateProductStrength(this.recordId, payload)
      );
      this.notificationService.showSnackBar('Product Strength updated successfully', 'success');
    } else {
      await firstValueFrom(
        this.productStrengthService.createProductStrength(payload)
      );
      this.notificationService.showSnackBar('Product Strength created successfully', 'success');
    }

    this.dialogRef.close(true); // Close dialog on success

  } catch (error) {
    const msg = this.isEditMode
      ? 'Failed to update product strength'
      : 'Failed to create product strength';

    console.error(msg, error);
    this.notificationService.showSnackBar(msg, 'failure');
  }
}

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName);
    const formattedName = this.formatFieldName(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${formattedName} is required`;
      if (field.errors['whitespace']) return `${formattedName} cannot be empty or only spaces`;
    }
    return '';
  }

  formatFieldName(fieldName: string): string {
    const formatted = fieldName.replace(/([A-Z])/g, ' $1').trim();
    return formatted.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
  }

}
