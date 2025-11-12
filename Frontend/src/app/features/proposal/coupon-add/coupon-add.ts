import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatMenuModule } from '@angular/material/menu';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserManagementService } from '../../user-management/user-management.service';
import { CouponRequest } from '../model/coupon-request.model';
import { ProposalService } from '../proposal.service';
import { CouponResponse } from '../model/coupon-response.model';
import { DateValidators } from '../../../shared/validators/date-validators'; // ADDED: Import shared validators
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { PercentageDirective } from '../../../shared/directives/percentage.directive';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  selector: 'app-coupon-add',
  templateUrl: './coupon-add.html',
  styleUrls: ['./coupon-add.scss'],
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatDatepickerModule, MatNativeDateModule, MatButtonModule,
    MatAutocompleteModule, MatMenuModule,PercentageDirective
  ]
})
export class CouponAddComponent implements OnInit {

  couponForm!: FormGroup;
  couponId: string | null = null;
  isActive: boolean = false;

  // UPDATED: Using shared DateValidators utility
  minDate = DateValidators.getToday();
  maxDate = DateValidators.getDateFromToday(365 * 5); // 5 years from today

  // Dropdown Data
  counselorList: DropDownResponseDto[] = [];
  filteredCounselorList: DropDownResponseDto[] = [];
  isLoadingCounselors = signal(false);
  private isFormSaved = false; 
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly userManagementService = inject(UserManagementService);
  private readonly proposalService = inject(ProposalService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationDialogService = inject(ConfirmationDialogService);

  ngOnInit(): void {
    this.initForm();
    this.loadCounselors();
    this.route.paramMap.subscribe(async params => {
      this.couponId = params.get('id');
      if (this.couponId) {
        await this.loadCouponForEdit(this.couponId);
      }
    });
  }

  // UPDATED: Using shared DateValidators
  private initForm(): void {
    this.couponForm = this.fb.group({
      couponName: [null, [Validators.required,NoWhitespaceValidator]],
      expiryDate: [null, [
        Validators.required, 
        DateValidators.minDateValidator(), 
        DateValidators.maxDateValidator(this.maxDate) 
      ]], 
      amount: [null, Validators.required],
      percentage: [null],
      buget: [null],
    });
  }

  private async loadCounselors(): Promise<void> {
    this.isLoadingCounselors.set(true);
    try {
      const response = await firstValueFrom(this.userManagementService.getAllActiveSalesPerson());
      this.counselorList = response;
      this.filteredCounselorList = response;
      this.isLoadingCounselors.set(false);
    } catch (err) {
      console.error(err);
      this.notificationService.showSnackBar('Failed to load counselors', 'failure');
      this.isLoadingCounselors.set(false);
    } 
  }

  private async loadCouponForEdit(id: string): Promise<void> {
    try {
      const coupon = await firstValueFrom(this.proposalService.getCouponById(id));
      if (coupon) {
        this.patchForm(coupon);
        this.isActive = !!coupon.isActive;
      }
    } catch (err) {
      console.error(err);
      this.notificationService.showSnackBar('Failed to load coupon details', 'failure');
    }
  }

  private patchForm(coupon: any): void {
    this.couponForm.patchValue({
      couponName: coupon.couponName,
      expiryDate: coupon.expiryDate ? new Date(coupon.expiryDate) : null,
      amount: coupon.amount,
      percentage: coupon.percentage,
      buget: coupon.buget
    });
  }

  counselorDisplayFn = (id: number): string => {
    const c = this.counselorList.find(x => x.id === id);
    return c ? c.value : '';
  };

  isFieldInvalid(field: string): boolean {
    const control = this.couponForm.get(field);
    return !!control && control.invalid && (control.dirty || control.touched);
  }

  getFieldError(field: string): string | null {
    const control = this.couponForm.get(field);
    if (!control || !control.errors) return null;
    var formattedName = this.formatFieldName(field);
    if (control.errors['required']) return `${formattedName} is required`;
    if (control.errors['whitespace']) return `${formattedName} cannot be empty or only spaces`;
    
    if (field === 'expiryDate') {
      return DateValidators.getDateErrorMessage(control, 'Expiry date');
    }
    
    return null;
  }

  isSaveAndClose = false; 

async onSubmit(): Promise<void> {
  if (!this.couponForm.valid) {
    this.couponForm.markAllAsTouched();
    this.notificationService.showSnackBar(
      'Please fill all the required fields.',
      'normal'
    );
    this.isSaveAndClose = false;
    return;
  }

  const payload: CouponRequest = this.preparePayload();

  try {
    let response: CommonOperationResponseDto;

    if (this.couponId) {
      response = await firstValueFrom(
        this.proposalService.updateCoupon(this.couponId, payload)
      );
    } else {
      response = await firstValueFrom(this.proposalService.addCoupon(payload));
    }

    const isSuccess =
      !!response.id && response.id !== '00000000-0000-0000-0000-000000000000';

    this.notificationService.showSnackBar(
      response.message,
      isSuccess ? 'success' : 'failure'
    );

    if (isSuccess) {
      if (this.isSaveAndClose) {
        this.isSaveAndClose = false;
        this.couponId
          ? this.onClose()
          : this.router.navigate(['/coupons/view']);
        return;
      }

      if (!this.couponId) {
        this.router.navigate(['/coupon/edit', response.id]);
      }

      this.isFormSaved = true;
    }

  } catch (err) {
    console.error('Coupon save failed:', err);
    this.notificationService.showSnackBar(
      'Unexpected error occurred!',
      'failure'
    );
  } finally {
    this.isSaveAndClose = false;
  }
}



  onSaveAndClose(): void {
    this.isSaveAndClose = true;
    this.onSubmit();
  }

  onClose(): void {
    if (this.hasUnsavedChanges()) {
      this.confirmationDialogService.openConfirmation({
        title: 'Discard Changes',
        message: 'You have unsaved changes. Are you sure you want to leave without saving?'
      }).subscribe(confirmed => {
        if (confirmed) {
          this.router.navigate(['/coupons/view']);
        }
      });
    } else {
      this.router.navigate(['/coupons/view']);
    }
  }

  onAddCoupon(): void {
    if (this.hasUnsavedChanges()) {
      this.confirmationDialogService.openConfirmation({
        title: 'Discard Changes',
        message: 'You have unsaved changes. Are you sure you want to leave without saving?'
      }).subscribe(confirmed => {
        if (confirmed) {
          this.router.navigate(['/coupon/add']);
        }
      });
    } else {
      this.router.navigate(['/coupon/add']);
    }
  }

  private preparePayload(): CouponRequest {
    const raw = this.couponForm.value;
    return {
      couponName: raw.couponName.trim(),
      expiryDate: raw.expiryDate instanceof Date ? raw.expiryDate.toISOString() : raw.expiryDate,
      amount: Number(raw.amount),
      percentage: raw.percentage != null ? Number(raw.percentage) : null,
      buget: raw.buget != null ? Number(raw.buget) : null
    };
  }

  private hasUnsavedChanges(): boolean {
    if (this.isFormSaved) return false; 
    return this.couponForm.dirty || this.couponForm.touched;
  }

  onDeleteClick(): void {
    if (!this.couponId) return;
    this.confirmationDialogService.openConfirmation({
      title: 'Delete Coupon',
      message: 'Are you sure you want to delete this coupon?'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.deleteCoupon();
      }
    });
  }

  private deleteCoupon(): void {
    this.proposalService.deleteCoupon([this.couponId!]).subscribe({
      next: () => {
        this.notificationService.showSnackBar('Coupon deleted successfully!', 'success');
        this.router.navigate(['/coupons/view']);
      },
      error: err => {
        console.error(err);
        this.notificationService.showSnackBar('Failed to delete coupon', 'failure');
      }
    });
  }

  onToggleActive(active: boolean): void {
    if (!this.couponId) return;
    this.confirmationDialogService.openConfirmation({
      title: `${active ? 'Activate' : 'Deactivate'} Coupon`,
      message: `Are you sure you want to ${active ? 'activate' : 'deactivate'} this coupon?`
    }).subscribe(confirmed => {
      if (confirmed) {
        this.toggleStatus(active);
      }
    });
  }

  private toggleStatus(active: boolean): void {
     this.proposalService.bulkToggleActive([this.couponId!], active).subscribe({
      next: () => {
        this.isActive = active;
        this.notificationService.showSnackBar(
          `Coupon ${active ? 'activated' : 'deactivated'} successfully!`, 'success'
        );
      },
      error: err => {
        console.error(err);
        this.notificationService.showSnackBar('Failed to change status', 'failure');
      }
    });
  }

  formatFieldName(fieldName: string): string {
    const formatted = fieldName.replace(/([A-Z])/g, ' $1').trim();
    return formatted.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
  }
}
