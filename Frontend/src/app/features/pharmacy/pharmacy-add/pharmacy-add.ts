import { Component, OnInit, AfterViewInit, ViewChild, signal, ChangeDetectorRef, NgZone, ApplicationRef, ViewChildren, ElementRef, QueryList } from '@angular/core';
import { FormArray, FormControlName, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ActivatedRoute, Router } from '@angular/router';
import { PharmacyService } from '../pharmacy.services';
import { MatSelect, MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { firstValueFrom } from 'rxjs';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { CurrencyResponseDto } from '../model/currency-response-model';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { PriceListItemContainerComponent } from '../../price-list-items/price-list-item-container/price-list-item-container';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  selector: 'app-pharmacy-add',
  templateUrl: './pharmacy-add.html',
  styleUrls: ['./pharmacy-add.scss'],
  standalone: true,
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
      PriceListItemContainerComponent,
      MatTableModule,
      MatButtonToggleModule,
      FullPageLoaderComponent
      ],
})
export class PharmacyAddComponent implements OnInit {
  pharmacyForm!: FormGroup;
  pharmacyId!: string | null;
  currencyOptions = signal<CurrencyResponseDto[]>([]);
  isEditMode = signal(false);
  isSaveAndClose: boolean = false;
  statusValue: 'Active' | 'Deactive' = 'Active';
  activeTab: 'pharmacy' | 'priceList' = 'pharmacy';

  @ViewChildren(FormControlName, { read: ElementRef }) formControls!: QueryList<ElementRef>;

  shippingMethods: DropDownResponseDto[] = [];
  allShippingMethods: DropDownResponseDto[] = [];
  isLoadingShippingMethods = signal(false);

  selectedShippingMethodId: number | null = null;
  isLoadingPage = signal(false);

  constructor(
    private fb: FormBuilder,
    private pharmacyService: PharmacyService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationDialogService,
    private router: Router,
    private route: ActivatedRoute,
    private appRef: ApplicationRef
  ) {}

  async ngOnInit(): Promise<void> {
    this.route.queryParams.subscribe(params => {
      if (params['tab']) {
        this.activeTab = params['tab'] as any;
      }
    });

    this.pharmacyId = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!this.pharmacyId);
    this.initForm();
    this.pharmacyForm.get('hasFixedCommission')?.valueChanges.subscribe(val => {
    const commissionControl = this.pharmacyForm.get('commissionPercentage');
    if (val) {
      commissionControl?.setValidators([Validators.required, Validators.min(0)]);
    } else {
      commissionControl?.clearValidators();
      commissionControl?.setValue(null);
    }
    commissionControl?.updateValueAndValidity();
  });
    await this.getAllShippingMethods();
    await this.fetchDropdownData();

    await this.fetchDropdownData();
    if (this.isEditMode() && this.pharmacyId) {
      await this.loadPharmacyData(this.pharmacyId);
    }
  }



initForm() {
    this.pharmacyForm = this.fb.group({
      name: ['', [Validators.required,NoWhitespaceValidator]],
      startDate: [''],
      endDate: [''],
      description: [''],
      isLab : [false],
      hasFixedCommission : [false],
      commissionPercentage : [null],
      currency: [null, Validators.required],
        status: [{ value: 'Active', disabled: true }],
        shippingMethods: this.fb.array([]),
    },
    { validators: this.dateRangeValidator() }
  );
  }

  get shippingMethodsArray(): FormArray {
    return this.pharmacyForm.get('shippingMethods') as FormArray;
  }

  async getAllShippingMethods() {
    this.isLoadingShippingMethods.set(true);
    this.pharmacyService.getAllShippingMethods().subscribe({
      next: (shippingMethods) => {
        this.allShippingMethods = shippingMethods;
        this.shippingMethods = [...shippingMethods];
        this.isLoadingShippingMethods.set(false);
      },
      error: () => {
        this.isLoadingShippingMethods.set(false);
      },
    });
  }

async fetchDropdownData(): Promise<void> {
  try {
    this.currencyOptions.set(await firstValueFrom(this.pharmacyService.getAllCurrencies()));

    if (!this.isEditMode) {
      const defaultCurrency = this.currencyOptions().find(
        (c: CurrencyResponseDto) => c.currencyName.toLowerCase() === 'us dollar'
      );

      if (defaultCurrency) {
        this.pharmacyForm.patchValue({ currency: defaultCurrency.id });
      }
    }
  } catch (error) {
    this.notificationService.showSnackBar(
      'Failed to load dropdown data',
      'failure'
    );
    console.error(error);
  }
}

  async loadPharmacyData(id: string): Promise<void> {
    try {
      this.isLoadingPage.set(true);
      const pharmacy = await firstValueFrom(this.pharmacyService.getPharmacyById(id));
      this.statusValue = pharmacy.isActive ? 'Active' : 'Deactive';

      this.pharmacyForm.patchValue({
        name: pharmacy.name,
        startDate: pharmacy.startDate ? new Date(pharmacy.startDate) : '',
        endDate: pharmacy.endDate ? new Date(pharmacy.endDate) : '',
        description: pharmacy.description,
        isLab : pharmacy.isLab,
        currency: pharmacy.currencyId,
        status: this.statusValue,
        hasFixedCommission : pharmacy.hasFixedCommission,
        commissionPercentage : pharmacy.commissionPercentage
      });

      if (pharmacy.shippingMethods && pharmacy.shippingMethods.length) {
        pharmacy.shippingMethods.forEach((sm: any) => {
          this.shippingMethodsArray.push(
            this.fb.group({
              id: [sm.id],
              shippingMethodId : [sm.shippingMethodId],
              value: [sm.value],
              amount: [sm.amount ?? null, [Validators.required, Validators.min(0)]],
              costOfShipping: [sm.costOfShipping ?? null, [Validators.required, Validators.min(0)]],
              serviceCode: [sm.serviceCode ?? null],
            })
          );

          const selectedIds = pharmacy.shippingMethods.map((sm: any) => sm.shippingMethodId);
          this.shippingMethods = this.shippingMethods.filter(m => !selectedIds.includes(m.id));
        });
      }
    } catch (error) {
      this.notificationService.showSnackBar('Failed to load pharmacy', 'failure');
      console.error(error);
    }finally{
      this.isLoadingPage.set(false);
    }
  }
// async fetchDropdownData(): Promise<void> {
//   try {
//     this.currencyOptions.set(await firstValueFrom(this.pharmacyService.getAllCurrencies()));
//     if (!this.isEditMode()) {
//       const defaultCurrency = this.currencyOptions().find(c => c.currencyName.toLowerCase() === 'us dollar');
//       if (defaultCurrency) {
//         this.pharmacyForm.patchValue({ currency: defaultCurrency.id });
//       }
//     }
//   } catch (error) {
//     this.showError('Failed to load dropdown data');
//     console.error(error);
//   }
// }

  onShippingMethodSelect(selectedId: number | null, select: MatSelect) {
    if (!selectedId) return;

    const method = this.shippingMethods.find(m => m.id === selectedId);
    if (!method) return;

    this.shippingMethodsArray.push(
      this.fb.group({
        shippingMethodId: [method.id],
        value: [method.value],
        amount: [null, [Validators.required, Validators.min(0)]],
        costOfShipping: [null, [Validators.required, Validators.min(0)]],
        serviceCode: [null],
      })
    );

    this.shippingMethods = this.shippingMethods.filter(m => m.id !== method.id);

    select.value = null;
    select.writeValue(null);
  }

removeShippingMethod(index: number) {

  const removed = this.shippingMethodsArray.at(index).value;

  this.shippingMethodsArray.removeAt(index);

  const restoredMethod = this.allShippingMethods.find(m => m.id === removed.shippingMethodId);
  if (restoredMethod) {
    this.shippingMethods = [...this.shippingMethods, restoredMethod];
  }
}


get getSelectedShippingMethodsControls(): FormGroup[] {
  return this.shippingMethodsArray.controls as FormGroup[];
}


  isFieldInvalid(fieldName: string): boolean {
    const field = this.pharmacyForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  getFieldError(fieldName: string): string {
    const fieldLabels: { [key: string]: string } = {
      name: 'Name',
      startDate: 'Start Date',
      endDate: 'End Date',
      description: 'Description',
      currency: 'Currency',
    };
    const label = fieldLabels[fieldName] || fieldName;
    const field = this.pharmacyForm.get(fieldName);

    if (field?.errors) {
      if (field.errors['required']) return `${label} is required`;
      if (field.errors['email']) return `Please enter a valid ${label}`;
      if (field.errors['minlength'])
        return `${label} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['pattern']) return `${label} format is invalid`;
      if (field.errors['dateOrderInvalid']) return 'Start Date must be before End Date';
      if (field.errors['whitespace']) return `${label} cannot be empty or only spaces`;
    }
    return '';
  }

  dateRangeValidator() {
    return (formGroup: FormGroup) => {
      const startDate = formGroup.get('startDate')?.value;
      const endDate = formGroup.get('endDate')?.value;

      if (startDate && endDate) {
        const start = new Date(startDate);
        const end = new Date(endDate);

        if (start > end) {
          const startDateControl = formGroup.get('startDate');
          const endDateControl = formGroup.get('endDate');

          const endTouched = endDateControl?.dirty || endDateControl?.touched;
          if (endTouched) {
            endDateControl?.setErrors({ dateOrderInvalid: true });
            startDateControl?.setErrors(null);
          } else {
            startDateControl?.setErrors({ dateOrderInvalid: true });
            endDateControl?.setErrors(null);
          }
        } else {
          formGroup.get('startDate')?.setErrors(null);
          formGroup.get('endDate')?.setErrors(null);
        }
      } else {
        formGroup.get('startDate')?.setErrors(null);
        formGroup.get('endDate')?.setErrors(null);
      }
      return null;
    };
  }

  async submitForm(isSaveAndClose = false): Promise<void> {
    if (this.pharmacyForm.invalid) {
      this.pharmacyForm.markAllAsTouched();
      this.appRef.whenStable().then(() => {
      this.scrollToFirstInvalidControl();
    });
  this.pharmacyForm.updateValueAndValidity();

      const errors = this.getAllErrors();

      if (errors.includes('required')) {
        this.notificationService.showSnackBar('Please fill all required fields', 'normal');
      } else if (errors.includes('dateOrderInvalid')) {
        this.notificationService.showSnackBar('Start Date must be before End Date', 'normal');
      } else {
        this.notificationService.showSnackBar('Please fix the highlighted errors', 'normal');
      }
      return;
    }

    const formData = this.pharmacyForm.getRawValue();
    const payload = {
      name: formData.name.trim(),
      startDate: formData.startDate ? this.formatDate(formData.startDate) : null,
      endDate: formData.endDate ? this.formatDate(formData.endDate) : null,
      currencyId: formData.currency,
      description: formData.description || null,
      isLab : formData.isLab || false,
      hasFixedCommission : formData.hasFixedCommission || false,
      commissionPercentage : formData.hasFixedCommission ? formData.commissionPercentage : null,
      shippingMethods: formData.shippingMethods?.map((sm: any) => ({
        shippingMethodId: sm.shippingMethodId,
        id : sm.id,
        amount: sm.amount,
        costOfShipping: sm.costOfShipping,
        serviceCode: sm.serviceCode,
      })),
    };

    try {
      if (this.isEditMode() && this.pharmacyId) {
        await firstValueFrom(this.pharmacyService.updatePharmacy(this.pharmacyId, payload));
        this.notificationService.showSnackBar('Pharmacy updated successfully', 'success', 'Close');

        if (isSaveAndClose) {
          this.router.navigate(['/pharmacy/view']);
        }
      } else {
        const response = await firstValueFrom(this.pharmacyService.createPharmacy(payload));
        this.notificationService.showSnackBar('Pharmacy created successfully', 'success', 'close');

        const newPharmacyId = response.id;
        if (isSaveAndClose) {
          this.router.navigate(['/pharmacy/view']);
        } else {
          this.router.navigate([`/pharmacy/edit/${newPharmacyId}`]);
        }
      }
    } catch (error) {
      const message = this.isEditMode() ? 'Failed to update Pharmacy' : 'Failed to create pharmacy';
      this.notificationService.showSnackBar(message, 'failure');
      console.error(error);
    }
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }



  getAllErrors(): string[] {
    const errors: string[] = [];

    Object.keys(this.pharmacyForm.controls).forEach((key) => {
      const controlErrors = this.pharmacyForm.get(key)?.errors;
      if (controlErrors) {
        Object.keys(controlErrors).forEach((errorKey) => {
          if (!errors.includes(errorKey)) {
            errors.push(errorKey);
          }
        });
      }
    });

    return errors;
  }
    goBack() {
    this.router.navigate(['/pharmacy/view']);
  }

  // Other methods like activatePharmacy, deactivatePharmacy, deletePharmacy, openConfirmation, updateTab...
async activatePharmacy(): Promise<void> {
  if (!this.pharmacyId) return;

  const confirmed = await this.openConfirmation('Activate');
  if (!confirmed) return;
  try {
    await firstValueFrom(this.pharmacyService.activatePharmacies([this.pharmacyId]));
    this.notificationService.showSnackBar('Pharmacy activated successfully', 'success');
    this.pharmacyForm.get('status')?.patchValue('Active');
  } catch (error) {
    this.notificationService.showSnackBar('Failed to activate Pharmacy', 'failure');
    console.error(error);
  }
}

async deactivatePharmacy(): Promise<void> {
  if (!this.pharmacyId) return;
   const confirmed = await this.openConfirmation('Deactivate');
   if (!confirmed) return;
  try {
    await firstValueFrom(this.pharmacyService.deactivatePharmacies([this.pharmacyId]));
    this.notificationService.showSnackBar('Pharmacy deactivated successfully', 'success');
    this.pharmacyForm.get('status')?.patchValue('Deactive');
  } catch (error) {
    this.notificationService.showSnackBar('Failed to deactivated pharmacy', 'failure');
    console.error(error);
  }
}

async deletePharmacy(): Promise<void> {
  if (!this.pharmacyId) return;
  const confirmed = await this.openConfirmation('Delete');
  if (!confirmed) return;
  try {
    await firstValueFrom(this.pharmacyService.deletePharmacies([this.pharmacyId]));
    this.notificationService.showSnackBar('Pharmacy deleted successfully', 'success');
    this.router.navigate(['/pharmacy/view']);
  } catch (error) {
    this.notificationService.showSnackBar('Failed to delete Pharmacy', 'failure');
    console.error(error);
  }
}

async openConfirmation(action: string): Promise<boolean> {
  const data: ConfirmationDialogData = {
    title: `${action} Confirmation`,
    message: `<p>Are you sure you want to <strong>${action.toLowerCase()}</strong> this pharmacy?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const confirmed = await firstValueFrom(this.confirmationService.openConfirmation(data));
  return confirmed ?? false;
}

updateTab(event:MatButtonToggleChange):void {
  const tabName = event.value as 'pharmacy' | 'priceList';
  this.activeTab = tabName;
  this.router.navigate([], {
    relativeTo: this.route,
    queryParams: { tab: tabName },
    queryParamsHandling: 'merge'
  });
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
navigateToAddPharmacy() {
  this.router.navigate(['/pharmacy/add']);
}

}
