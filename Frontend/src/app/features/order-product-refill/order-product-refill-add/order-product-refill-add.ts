import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom, Subscription } from 'rxjs';
import { NotificationService } from '../../../shared/services/notification.service';
import { PharmacyService } from '../../pharmacy/pharmacy.services';
import { ConfirmationDialogData } from '../../../shared/models/confirmation-dialog.model';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { HttpErrorResponse } from '@angular/common/http';
import { OrderproductRefillService } from '../order-product-refill.services';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-order-product-refill-add',
  imports: [
    MatFormFieldModule,
    CommonModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonToggleModule,
    MatDatepickerModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './order-product-refill-add.html',
  styleUrl: './order-product-refill-add.scss'
})
export class OrderProductRefillAddComponent {

  orderProductRefillDetailForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private orderProductRefillDetailService: OrderproductRefillService,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationDialogService
  ) { }

  ngOnInit(): void {
    this.orderProductRefillDetailForm = this.fb.group({
      productName: [{ value: '', disabled: true }], // readonly
      daysSupply: [null],
      doseAmount: [null],
      doseUnit: [''],
      frequencyPerDay: [null],
      bottleSizeMl: [null],
      refillDate: [null],
      status: [''],
      assumption: ['']
    });

    this.loadOrderProductRefillDetail();
  }

  private async loadOrderProductRefillDetail(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    try {
      const response = await firstValueFrom(
        this.orderProductRefillDetailService.getOrderProductRefillDetailsById(id)
      );

      if (response) {
        this.orderProductRefillDetailForm.patchValue({
          productName: response.productName,
          daysSupply: response.daysSupply,
          doseAmount: response.doseAmount,
          doseUnit: response.doseUnit,
          frequencyPerDay: response.frequencyPerDay,
          bottleSizeMl: response.bottleSizeML,
          refillDate: new Date(response.refillDate + 'T00:00:00'),
          status: response.status,
          assumption: response.assumption
        });
      }
    } catch (error) {
      console.error(error);
      this.notificationService.showSnackBar('Failed to load Order Product Refill details.', 'failure');
    }
  }



  goBack(): void {
    this.router.navigate(['/order-product-refill/view']);
  }

  async submitForm(closeAfterSave = false): Promise<void> {
    if (this.orderProductRefillDetailForm.invalid) {
      this.notificationService.showSnackBar('Please fill all required fields.', 'failure');
      return;
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.notificationService.showSnackBar('Invalid record Id.', 'failure');
      return;
    }

    const formValue = this.orderProductRefillDetailForm.getRawValue(); // includes readonly productName
    const updateRequest = {
      daysSupply: formValue.daysSupply,
      doseAmount: formValue.doseAmount,
      doseUnit: formValue.doseUnit,
      frequencyPerDay: formValue.frequencyPerDay,
      bottleSizeMl: formValue.bottleSizeMl,
      refillDate: formValue.refillDate
        ? `${formValue.refillDate.getFullYear()}-${String(formValue.refillDate.getMonth() + 1).padStart(2, '0')}-${String(formValue.refillDate.getDate()).padStart(2, '0')}`
        : null,
      status: formValue.status,
      assumption: formValue.assumption
    };

    try {
      await firstValueFrom(
        this.orderProductRefillDetailService.updateOrderProductRefillDetail(id, updateRequest)
      );

      this.notificationService.showSnackBar('Order Product Refill Detail updated successfully.', 'success');

      if (closeAfterSave) {
        this.router.navigate(['/order-product-refill/view']);
      }
    } catch (error) {
      console.error(error);
      this.notificationService.showSnackBar('Failed to update Order Product Refill Detail.', 'failure');
    }
  }

}
