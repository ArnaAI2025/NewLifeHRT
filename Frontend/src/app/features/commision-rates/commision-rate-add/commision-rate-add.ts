import { ApplicationRef, Component, ElementRef, OnInit, QueryList, signal, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl, FormsModule, ReactiveFormsModule, AbstractControl, ValidationErrors, FormControlName } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog'
import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { PharmacyService } from '../../pharmacy/pharmacy.services';
import { CommisionRateService } from '../commision-rate.services';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { PercentageDirective } from '../../../shared/directives/percentage.directive';

@Component({
  selector: 'app-commision-rate-add',
  imports: [MatFormFieldModule,
    CommonModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDividerModule,
    FormsModule,
    ReactiveFormsModule,
    FullPageLoaderComponent,
    PercentageDirective
  ],
  templateUrl: './commision-rate-add.html',
  styleUrl: './commision-rate-add.scss'
})
export class CommisionRateAddComponent implements OnInit {
  commisionRateForm!: FormGroup;
  isEditMode = false;
  commisionRateId: string | null = null;
  statusValue: string = 'Active';
  isSaveAndClose = false;

  products = signal<any[]>([]);
  context: 'product' | 'standalone' = 'standalone';
  entityId: string | null = null;
  @ViewChildren(FormControlName, { read: ElementRef }) formControls!: QueryList<ElementRef>;
  isLoadingPage = signal(false);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private commisionRateService: CommisionRateService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationDialogService,
    private pharmacyService: PharmacyService,
    private appRef: ApplicationRef
  ) { }

  async ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.context = params['context'] || 'standalone';
      this.entityId = params['entityId'] || null;
    });
    this.commisionRateId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.commisionRateId;
    this.initForm();
    this.commisionRateForm.valueChanges.subscribe(() => {
      this.amountRangeValidator(this.commisionRateForm);
    });

    // Load dropdowns first
    await this.loadDropdownData();

    if (!this.isEditMode) {
      if (this.context === 'product' && this.entityId) {
        this.commisionRateForm.patchValue({ productId: this.entityId });
      }
    }

    // Then load item data if in edit mode
    if (this.isEditMode && this.commisionRateId) {
      await this.loadCommisionRateData();
    }
  }

  initForm() {
    this.commisionRateForm = this.fb.group({
      fromAmount: ['', [Validators.required, this.numberValidator]],
      toAmount: ['', [Validators.required, this.numberValidator]],
      ratePercentage: [null, [this.numberValidator]],
      productId: ['', Validators.required],
      status: [{ value: 'Active', disabled: true }],
    });
  }

  numberValidator(control: FormControl) {
    const value = control.value;
    if (value === null || value === undefined || value === '') {
      return null;
    }
    if (isNaN(value)) {
      return { invalidNumber: true };
    }

    return null;
  }

  async loadDropdownData() {
    try {
      const [
        products
      ] = await Promise.all([
        firstValueFrom(this.commisionRateService.getAllProductsForDropdown()),
      ]);

      this.products.set(products);

    } catch (error) {
      this.notificationService.showSnackBar('Failed to load dropdown data', 'failure');
    }
  }

  async loadCommisionRateData() {
    if (!this.commisionRateId) return;

    try {
      this.isLoadingPage.set(true);
      const item = await firstValueFrom(
        this.commisionRateService.getCommisionRateById(this.commisionRateId)
      );
      this.statusValue = item.status;
      this.commisionRateForm.patchValue({
        ...item,
        status: item.status
      });
    } catch (error) {
      this.notificationService.showSnackBar('Failed to load commission rates', 'failure');
    }finally{
      this.isLoadingPage.set(false);
    }
  }

  async submitForm(isSaveAndClose = false) {
    this.isSaveAndClose = isSaveAndClose;

    if (this.commisionRateForm.invalid) {
      this.commisionRateForm.markAllAsTouched();
      this.appRef.whenStable().then(() => {
      this.scrollToFirstInvalidControl();
    });
      const errors = this.getAllErrors();

      if (errors.includes('required')) {
        this.notificationService.showSnackBar('Please fill all required fields', 'normal');
      } else {
        this.notificationService.showSnackBar('Please fix the highlighted errors', 'normal');
      }
      return;
    }

    const formValue = this.commisionRateForm.getRawValue();
    const payload = {
      ...formValue,
      status: this.statusValue
    };

    try {
      if (this.isEditMode && this.commisionRateId) {
        await firstValueFrom(
          this.commisionRateService.updateCommisionRate(this.commisionRateId, payload)
        );
        this.notificationService.showSnackBar('Commission Rate updated successfully', 'success');
      } else {
        const response = await firstValueFrom(
          this.commisionRateService.createCommisionRate(payload)
        );
        this.notificationService.showSnackBar('Commission Rate created successfully', 'success');
        this.commisionRateId = response.id;

        const queryParams: any = {};
        if (this.context) queryParams.context = this.context;
        if (this.entityId) queryParams.entityId = this.entityId;
        if (!this.isEditMode) {
          this.router.navigate(
            ['/commissionrate/edit', this.commisionRateId],
            { queryParams }
          );
        }
      }

      if (isSaveAndClose) {
        this.navigateAfterSave();
      }
    } catch (error) {
      const msg = this.isEditMode
      ? 'Failed to update commission rate'
      : 'Failed to create commission rate';
      this.notificationService.showSnackBar(msg, 'failure');
    }
  }

  async activateCommisionRate() {
    if (!this.commisionRateId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Activate Confirmation',
      message: 'Are you sure you want to <strong>activate</strong> this commission rate?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;

    try {
      await firstValueFrom(
        this.commisionRateService.activateCommisionRates([this.commisionRateId])
      );
      this.statusValue = 'Active';
      this.commisionRateForm.patchValue({ status: 'Active' });
      this.notificationService.showSnackBar('Commission Rate activated successfully', 'success');
    } catch (error) {
      this.notificationService.showSnackBar('Failed to activate commission rate', 'failure');
    }
  }

  async deactivateCommisionRate() {
    if (!this.commisionRateId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Deactivate Confirmation',
      message: 'Are you sure you want to <strong>deactivate</strong> this commission rate?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;
    try {
      await firstValueFrom(
        this.commisionRateService.deactivateCommisionRates([this.commisionRateId])
      );
      this.statusValue = 'Inactive';
      this.commisionRateForm.patchValue({ status: 'Inactive' });
      this.notificationService.showSnackBar('Commission Rate deactivated successfully', 'success');
    } catch (error) {
      this.notificationService.showSnackBar('Failed to deactivate commission rate', 'failure');
    }
  }

  async deleteCommisionRate() {
    if (!this.commisionRateId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Delete Confirmation',
      message: 'Are you sure you want to <strong>delete</strong> this commission rate?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;

    try {
      await firstValueFrom(
        this.commisionRateService.deleteCommisionRates([this.commisionRateId])
      );
      this.notificationService.showSnackBar('Commission Rate deleted successfully', 'success');
      if (this.context === 'product' && this.entityId) {
        this.router.navigate(['/product/edit', this.entityId], {
          queryParams: { tab: 'commissionRate' }
        });
      } else {
        this.router.navigate(['/commissionrate/view']);
      }
    } catch (error) {
      this.notificationService.showSnackBar('Failed to delete commission rate', 'failure');
    }
  }

  preventNonNumeric(event: KeyboardEvent) {
    const allowedKeys = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete', '.'];
    if (allowedKeys.includes(event.key) || (event.key >= '0' && event.key <= '9')) {
      return;
    }
    event.preventDefault();
  }

  private navigateAfterSave() {
    if (this.context === 'product' && this.entityId) {
      this.router.navigate(['/product/edit', this.entityId], {
        queryParams: { tab: 'commissionRate' }
      });
    } else {
      this.router.navigate(['/commissionrate/view']);
    }
  }

  goBack() {
    if (this.context === 'product' && this.entityId) {
      this.router.navigate(['/product/edit', this.entityId], {
        queryParams: { tab: 'commissionRate' }
      });
    } else {
      this.router.navigate(['/commissionrate/view']);
    }
  }

  private amountRangeValidator = (group: AbstractControl): ValidationErrors | null => {
    const fromAmountControl = group.get('fromAmount');
    const toAmountControl = group.get('toAmount');

    if (!fromAmountControl || !toAmountControl) return null;

    const fromAmount = fromAmountControl.value;
    const toAmount = toAmountControl.value;

    if (toAmountControl.errors?.['amountRange']) {
      delete toAmountControl.errors['amountRange'];
      if (Object.keys(toAmountControl.errors).length === 0) {
        toAmountControl.setErrors(null);
      }
    }
    if (fromAmount === null || fromAmount === '' || toAmount === null || toAmount === '') {
      return null;
    }
    const from = Number(fromAmount);
    const to = Number(toAmount);
    if (isNaN(from) || isNaN(to)) {
      return null;
    }
    if (from > to) {
      toAmountControl.setErrors({ ...(toAmountControl.errors || {}), amountRange: true });
      return { amountRange: true };
    }
    return null;
  };

  getAllErrors(): string[] {
    const errors: string[] = [];

    Object.keys(this.commisionRateForm.controls).forEach(key => {
      const controlErrors = this.commisionRateForm.get(key)?.errors;
      if (controlErrors) {
        Object.keys(controlErrors).forEach(errorKey => {
          if (!errors.includes(errorKey)) {
            errors.push(errorKey);
          }
        });
      }
    });

    return errors;
  }

private scrollToFirstInvalidControl() {
  for (const control of this.formControls.toArray()) {
    if (control.nativeElement.classList.contains('ng-invalid')) {
      control.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
      control.nativeElement.focus();
      break;
    }
  }
}

  addCommisionRate() {
      if (this.context && this.entityId) {
        this.router.navigate(['/commissionrate/add'], {
          queryParams: {
            context: this.context,
            entityId: this.entityId
          }
        });
      } else {
        this.router.navigate(['/commissionrate/add']);
      }
  }

}
