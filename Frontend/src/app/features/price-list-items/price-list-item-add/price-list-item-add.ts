import { ApplicationRef, Component, ElementRef, OnInit, QueryList, signal, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl, FormsModule, ReactiveFormsModule, FormControlName } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PriceListItemService } from '../price-list-item.services';
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
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';


@Component({
  selector: 'app-price-list-item-add',
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
      FullPageLoaderComponent
      ],
  templateUrl: './price-list-item-add.html',
  styleUrl: './price-list-item-add.scss'
})
export class PriceListItemAddComponent implements OnInit {
  priceListItemForm!: FormGroup;
  isEditMode = false;
  priceListItemId: string | null = null;
  statusValue: string = 'Active';
  isSaveAndClose = false;

  lifeFileDrugForms: any[] = [];
  lifeFileQuantityUnits: any[] = [];
  lifeFileScheduleCodes: any[] = [];
  pharmacies: any[] = [];
  products: any[] = [];
  currencies: any[] = [];

context: 'product' | 'pharmacy' | 'standalone' = 'standalone';
entityId: string | null = null;
 @ViewChildren(FormControlName, { read: ElementRef }) formControls!: QueryList<ElementRef>;
 isLoadingPage = signal(false);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private priceListItemService: PriceListItemService,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationDialogService,
    private pharmacyService: PharmacyService,
    private appRef: ApplicationRef
  ) {}

  async ngOnInit() {
  this.route.queryParams.subscribe(params => {
  this.context = params['context'] || 'standalone';
  this.entityId = params['entityId'] || null;
});
  this.priceListItemId = this.route.snapshot.paramMap.get('id');
  this.isEditMode = !!this.priceListItemId;
  this.initForm();

  // Load dropdowns first
  await this.loadDropdownData();

  if (!this.isEditMode) {
    if (this.context === 'product' && this.entityId) {
      this.priceListItemForm.patchValue({ productId: this.entityId });
    } else if (this.context === 'pharmacy' && this.entityId) {
      this.priceListItemForm.patchValue({ pharmacyId: this.entityId });
    }
  }

  // Then load item data if in edit mode
  if (this.isEditMode && this.priceListItemId) {
    await this.loadPriceListItemData();
  }
}

  initForm() {
    this.priceListItemForm = this.fb.group({
      amount: ['', [Validators.required, this.numberValidator]],
      costOfProduct: [null, this.numberValidator],
      lifeFilePharmacyProductId: [null],
      lifeFielForeignPmsId: [null],
      lifeFileDrugFormId: [null],
      lifeFileDrugName: [null],
      lifeFileDrugStrength: [null],
      lifeFileQuantityUnitId: [null],
      lifeFileScheduleCodeId: [null],
      pharmacyId: ['', Validators.required],
      productId: ['', Validators.required],
      status: [{ value: 'Active', disabled: true }],
      currencyId: [{ value: 1, disabled: true }]
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
      drugForms,
      quantityUnits,
      scheduleCodes,
      pharmacies,
      products,
      currencies
    ] = await Promise.all([
      firstValueFrom(this.priceListItemService.getAllLifeFileDrugForms()),
      firstValueFrom(this.priceListItemService.getAllLifeFileQuantityUnits()),
      firstValueFrom(this.priceListItemService.getAllLifeFileScheduleCodes()),
      firstValueFrom(this.priceListItemService.getAllPharmaciesForDropdown()),
      firstValueFrom(this.priceListItemService.getAllProductsForDropdown()),
      firstValueFrom(this.pharmacyService.getAllCurrencies())
    ]);

    this.lifeFileDrugForms = drugForms;
    this.lifeFileQuantityUnits = quantityUnits;
    this.lifeFileScheduleCodes = scheduleCodes;
    this.pharmacies = pharmacies;
    this.products = products;
    this.currencies = currencies;
    const defaultCurrency = currencies.find(c => Number(c.id) === 1);
    if (defaultCurrency) {
      this.priceListItemForm.patchValue({ currencyId: defaultCurrency.id });
    }

  } catch (error) {
    this.notificationService.showSnackBar('Failed to load dropdown data', 'failure');
  }
}

  async loadPriceListItemData() {
    if (!this.priceListItemId) return;

    try {
      this.isLoadingPage.set(true);
      const item = await firstValueFrom(
        this.priceListItemService.getPriceListItemById(this.priceListItemId)
      );

      this.statusValue = item.status;
      this.priceListItemForm.patchValue({
        ...item,
        status: item.status
      });
    } catch (error) {
      this.notificationService.showSnackBar('Failed to load price list item', 'failure');
    }finally{
      this.isLoadingPage.set(false);
    }
  }

  async submitForm(isSaveAndClose = false) {
  this.isSaveAndClose = isSaveAndClose;

  if (this.priceListItemForm.invalid) {
    this.priceListItemForm.markAllAsTouched();
    this.appRef.whenStable().then(() => {
      this.scrollToFirstInvalidControl();
    });
    return;
  }

  const formValue = this.priceListItemForm.getRawValue();
  const payload = {
    ...formValue,
    status: this.statusValue
  };

  try {
    if (this.isEditMode && this.priceListItemId) {
      await firstValueFrom(
        this.priceListItemService.updatePriceListItem(this.priceListItemId, payload)
      );
      this.notificationService.showSnackBar('Price list item updated successfully', 'success');
    } else {
      const response = await firstValueFrom(
        this.priceListItemService.createPriceListItem(payload)
      );
      this.notificationService.showSnackBar('Price list item created successfully', 'success');
      this.priceListItemId = response.id;
    }
    
    if (isSaveAndClose) {
      this.navigateAfterSave();
    } else if (!this.isEditMode) {
      const queryParams: any = {};
        if (this.context) queryParams.context = this.context;
        if (this.entityId) queryParams.entityId = this.entityId;
      // Only navigate to edit page for new items
      this.router.navigate(['/pricelistitem/edit', this.priceListItemId],{queryParams});
    }
  } catch (error) {
    const msg = this.isEditMode
      ? 'Failed to update price list item'
      : 'Failed to create price list item';
    this.notificationService.showSnackBar(msg, 'failure');
  }
}

  async activatePriceListItem() {
    if (!this.priceListItemId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Activate Confirmation',
      message: 'Are you sure you want to <strong>activate</strong> this price list item?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;

    try {
      await firstValueFrom(
        this.priceListItemService.activatePriceListItems([this.priceListItemId])
      );
      this.statusValue = 'Active';
      this.priceListItemForm.patchValue({ status: 'Active' });
      this.notificationService.showSnackBar('Price list item activated successfully', 'success');
    } catch (error) {
      this.notificationService.showSnackBar('Failed to activate price list item', 'failure');
    }
  }

  async deactivatePriceListItem() {
    if (!this.priceListItemId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Deactivate Confirmation',
      message: 'Are you sure you want to <strong>deactivate</strong> this price list item?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;
    try {
      await firstValueFrom(
        this.priceListItemService.deactivatePriceListItems([this.priceListItemId])
      );
      this.statusValue = 'Inactive';
      this.priceListItemForm.patchValue({ status: 'Inactive' });
      this.notificationService.showSnackBar('Price list item deactivated successfully', 'success');
    } catch (error) {
      this.notificationService.showSnackBar('Failed to deactivate price list item', 'failure');
    }
  }

  async deletePriceListItem() {
    if (!this.priceListItemId) return;

    const confirmed = await this.confirmationService.openConfirmation({
      title: 'Delete Confirmation',
      message: 'Are you sure you want to <strong>delete</strong> this price list item?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).toPromise();

    if (!confirmed) return;

    try {
      await firstValueFrom(
        this.priceListItemService.deletePriceListItems([this.priceListItemId])
      );
      this.notificationService.showSnackBar('Price list item deleted successfully', 'success');
      if (this.context === 'product' && this.entityId) {
      this.router.navigate(['/product/edit', this.entityId], {
        queryParams: { tab: 'priceList' }
      });
    } else if (this.context === 'pharmacy' && this.entityId) {
      this.router.navigate(['/pharmacy/edit', this.entityId], {
        queryParams: { tab: 'priceList' }
      });
    } else {
      this.router.navigate(['/pricelistitem/view']);
    }
    } catch (error) {
      this.notificationService.showSnackBar('Failed to delete price list item', 'failure');
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
      queryParams: { tab: 'priceList' }
    });
  } else if (this.context === 'pharmacy' && this.entityId) {
    this.router.navigate(['/pharmacy/edit', this.entityId], {
      queryParams: { tab: 'priceList' }
    });
  } else {
    this.router.navigate(['/pricelistitem/view']);
  }
}

goBack()
{
  if (this.context === 'product' && this.entityId) {
      this.router.navigate(['/product/edit', this.entityId], {
        queryParams: { tab: 'priceList' }
      });
    } else if (this.context === 'pharmacy' && this.entityId) {
      this.router.navigate(['/pharmacy/edit', this.entityId], {
        queryParams: { tab: 'priceList' }
      });
    } else {
      this.router.navigate(['/pricelistitem/view']);
    }
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

addPriceListItem() {
  if (this.context && this.entityId) {
    this.router.navigate(['/pricelistitem/add'], {
      queryParams: {
        context: this.context,
        entityId: this.entityId
      }
    });
  } else {
    this.router.navigate(['/pricelistitem/add']);
  }
}

}
