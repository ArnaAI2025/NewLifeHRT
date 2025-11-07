import { Component, OnInit, AfterViewInit, ViewChild, signal, ApplicationRef, ViewChildren, ElementRef, QueryList } from '@angular/core';
import { FormBuilder, FormControlName, FormGroup, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../product.services';
import { Observable } from 'rxjs';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ProductTypeResponseDto } from '../model/product-type-response.model';
import { ProductResponseDto } from '../model/product-response.model';
import { ProductCategoriesResponseDto } from '../model/product-categories-response.model';
import { ProductWebFormResponseDto } from '../model/product-webform-response.model';
import { ParentResponseDto } from '../model/parent-response-model';
import { provideNgxMask } from 'ngx-mask';
import { CreateProductRequestDto } from '../model/create-product-request.model';
import { ProductStrengthViewComponent } from '../../product-strengths/product-strength-view/product-strength-view';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { PriceListItemTableComponent } from '../../price-list-items/price-list-item-table/price-list-item-table';
import { PriceListItemContainerComponent } from '../../price-list-items/price-list-item-container/price-list-item-container';
import { CommisionRateContainerComponent } from '../../commision-rates/commision-rate-container/commision-rate-container';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  standalone: true,
  providers: [provideNgxMask()],
  selector: 'app-product-add',
  imports: [MatFormFieldModule,
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
    ProductStrengthViewComponent,
    PriceListItemContainerComponent,
    CommisionRateContainerComponent,
    FullPageLoaderComponent        
  ],
  templateUrl: './product-add.html',
  styleUrl: './product-add.scss'
})
export class ProductAddComponent implements OnInit {
  productForm!: FormGroup;
  productId!: string | null;
  parentOptions = signal<ParentResponseDto[]>([]);
  typeOptions = signal<ProductTypeResponseDto[]>([]);
  categoryOptions = signal<ProductCategoriesResponseDto[]>([]);
  webformOptions = signal<ProductWebFormResponseDto[]>([]);
  activeTab: 'product' | 'strengths' | 'priceList' | 'commissionRate' = 'product';
  isEditMode = signal(false);
  isSaveAndClose: boolean = false;
  showPublish = signal(false);
  showActivate = signal(false);
  showDeactivate = signal(false);
  isLoadingPage = signal(false);

  yesNoDefaultOptions = [
    { value: true, label: 'Yes' },
    { value: false, label: 'No' }
  ];

   @ViewChildren(FormControlName, { read: ElementRef }) formControls!: QueryList<ElementRef>;
   
  constructor(private fb: FormBuilder, private productService: ProductService,private confirmationService: ConfirmationDialogService,
    private router: Router, private route: ActivatedRoute, private readonly notificationService : NotificationService,private appRef: ApplicationRef) {}

  async ngOnInit(): Promise<void> {
    this.route.queryParams.subscribe(params => {
  if (params['tab']) {
    this.activeTab = params['tab'] as any;
  }
});
    this.productId = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!this.productId);
    this.initForm();

    await this.fetchDropdownData();
    if (this.isEditMode() && this.productId) {
      await this.loadProductData(this.productId);
    }

  }

async loadProductData(id: string): Promise<void> {
  try {
    this.isLoadingPage.set(true);
    const product = await firstValueFrom(this.productService.getProductById(id));

    this.productForm.patchValue({
      productId: product.productID,
      name: product.name,
      labCode: product.labCode,
      status: product.statusName,
      labCorp: product.labCorp,
      isColdStorageProduct : product.isColdStorageProduct,
      parent: product.parentId,
      type: product.typeId,
      category1: product.category1Id,
      category2: product.category2Id,
      category3: product.category3Id,
      description: product.description,
      protocol: product.protocol,
      scheduled: product.scheduled,
      webProductName: product.webProductName,
      webProductDescription: product.webProductDescription,
      webPopularMedicine: product.webPopularMedicine,
      webForm: product.webFormId,
      webStrengths: product.webStrengths,
      webCost: product.webCost,
      enableCalculator: product.enableCalculator,
      newEnableCalculator: product.newEnableCalculator,
      pbpEnable: product.pbpEnable
    });

    this.setButtonVisibility(product.statusName);
  } catch (error) {
    this.showError('Failed to load product');
    console.error(error);
  }finally{
    this.isLoadingPage.set(false);
  }
}

  async fetchDropdownData(): Promise<void> {
  try {
    const products =  await firstValueFrom(this.productService.getAllProducts());
    this.parentOptions.set(products);
    const types = await firstValueFrom(this.productService.getTypeOptions());
    this.typeOptions.set(types);
    const categories = await firstValueFrom(this.productService.getCategoriesOptions());
    this.categoryOptions.set(categories); 
    const webforms = await firstValueFrom(this.productService.getWebFormOptions());
    this.webformOptions.set(webforms);
  } catch (error) {
    this.showError('Failed to load dropdown data');
    console.error(error);
  }
}

  initForm() {
    this.productForm = this.fb.group({
      productId: ['', [Validators.required,NoWhitespaceValidator]],
      name: ['', [Validators.required,NoWhitespaceValidator]],
      status: [{ value: 'Draft', disabled: true }],
      parent: [null],
      type: [null, Validators.required  ],
      category1: [null],
      category2: [null],
      category3: [null],
      scheduled: [null],
      webPopularMedicine: [null],
      enableCalculator: [null],
      newEnableCalculator: [null],
      pbpEnable: [null],
      webForm: [null],
      labCode: [''],
      description: [''],
      protocol: [''],
      webProductName: [''],
      webProductDescription: [''],
      webStrengths: [''],
      webCost: [''],
      labCorp: [false],
      isColdStorageProduct : [false],
    });
  }

async submitForm(isSaveAndClose = false): Promise<void> {
  if (this.productForm.invalid) {
    this.productForm.markAllAsTouched();
    this.appRef.whenStable().then(() => {
      this.scrollToFirstInvalidControl();
    });
    this.notificationService.showSnackBar('Please fill all required fields','normal');
    return;
  }

  const formData = this.productForm.getRawValue();
  const payload: CreateProductRequestDto = {
    productID: formData.productId.trim(),
    name: formData.name.trim(),
    isLabCorp: formData.labCorp,
    isColdStorageProduct : formData.isColdStorageProduct,
    labCode: formData.labCode || null,
    parentId: formData.parent || null,
    typeId: formData.type || null,
    category1Id: formData.category1 || null,
    category2Id: formData.category2 || null,
    category3Id: formData.category3 || null,
    productDescription: formData.description || null,
    protocol: formData.protocol || null,
    isScheduled: formData.scheduled,
    webProductName: formData.webProductName || null,
    webProductDescription: formData.webProductDescription || null,
    isWebPopularMedicine: formData.webPopularMedicine,
    webFormId: formData.webForm || null,
    webStrength: formData.webStrengths || null,
    webCost: formData.webCost || null,
    isEnabledCalculator: formData.enableCalculator,
    isNewEnabledCalculator: formData.newEnableCalculator,
    isPBPEnabled: formData.pbpEnable
  };

  try {
    if (this.isEditMode() && this.productId) {
      await firstValueFrom(this.productService.updateProduct(this.productId, payload));
      this.notificationService.showSnackBar('Product updated successfully', 'success','Close');

      if (isSaveAndClose) {
        this.router.navigate(['/product/view']);
      }

    } else {
      const response = await firstValueFrom(this.productService.createProduct(payload));
      this.notificationService.showSnackBar('Product created successfully', 'success', 'close');

      const newProductId = response.id;
      if (isSaveAndClose) {
        this.router.navigate(['/product/view']);
      } else {
        this.router.navigate([`/product/edit/${newProductId}`]);
      }
    }
  } catch (error) {
    const message = this.isEditMode() ? 'Failed to update product' : 'Failed to create product';
    this.notificationService.showSnackBar(message, 'failure');
    console.error(error);
  }
}

navigateToAddProduct() {
  this.router.navigate(['/product/add']);
}

  private showError(message: string) {
    this.notificationService.showSnackBar(message, 'failure');
  }

  goBack() {
    this.router.navigate(['/product/view']);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  getFieldError(fieldName: string): string {
    const field = this.productForm.get(fieldName);
    const formattedName = this.formatFieldName(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${formattedName} is required`;
      if (field.errors['email']) return 'Please enter a valid email';
      if (field.errors['minlength']) return `${formattedName} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['pattern']) return `${formattedName} format is invalid`;
      if (field.errors['whitespace']) return `${formattedName} cannot be empty or only spaces`;
    }
    return '';
  }

  resetProduct(): CreateProductRequestDto {
  return {
    productID: '',
    name: '',
    isLabCorp: false,
    labCode: null,
    parentId: null,
    typeId: null,
    category1Id: null,
    category2Id: null,
    category3Id: null,
    productDescription: null,
    protocol: null,
    isScheduled: null,
    webProductName: null,
    webProductDescription: null,
    isWebPopularMedicine: null,
    webFormId: null,
    webStrength: null,
    webCost: null,
    isEnabledCalculator: null,
    isNewEnabledCalculator: null,
    isPBPEnabled: null
  };
}

async publishProduct(): Promise<void> {
  if (!this.productId) return;
  const confirmed = await this.openConfirmation('Publish');
  if (!confirmed) return;
  try {
    await firstValueFrom(this.productService.publishProducts([this.productId]));
    this.notificationService.showSnackBar('Product published successfully', 'success');
    this.productForm.get('status')?.patchValue('Active');
    this.setButtonVisibility('Active');
  } catch (error) {
    this.notificationService.showSnackBar('Failed to publish product', 'failure');
    console.error(error);
  }
}

async activateProduct(): Promise<void> {
  if (!this.productId) return;

  const confirmed = await this.openConfirmation('Activate');
  if (!confirmed) return;
  try {
    await firstValueFrom(this.productService.publishProducts([this.productId]));
    this.notificationService.showSnackBar('Product activated successfully', 'success');
    this.productForm.get('status')?.patchValue('Active');
    this.setButtonVisibility('Active');
  } catch (error) {
    this.notificationService.showSnackBar('Failed to activate product', 'failure');
    console.error(error);
  }
}

async deActivateProduct(): Promise<void> {
  if (!this.productId) return;
   const confirmed = await this.openConfirmation('Deactivate');
   if (!confirmed) return;
  try {
    await firstValueFrom(this.productService.deactivateProducts([this.productId]));
    this.notificationService.showSnackBar('Product deactivated successfully', 'success');
    this.productForm.get('status')?.patchValue('Retired');
    this.setButtonVisibility('Retired');
  } catch (error) {
    this.notificationService.showSnackBar('Failed to deactivate product', 'failure');
    console.error(error);
  }
}

async deleteProduct(): Promise<void> {
  if (!this.productId) return;
  const confirmed = await this.openConfirmation('Delete');
  if (!confirmed) return;
  try {
    await firstValueFrom(this.productService.deleteProducts([this.productId]));
    this.notificationService.showSnackBar('Product deleted successfully', 'success');
    this.router.navigate(['/product/view']);
  } catch (error) {
    this.notificationService.showSnackBar('Failed to delete product', 'failure');
    console.error(error);
  }
}

setButtonVisibility(status: string) {
  this.showPublish.set(false);
  this.showActivate.set(false);
  this.showDeactivate.set(false);

  switch (status) {
    case 'Draft':
      this.showPublish.set(true);
      break;
    case 'Active':
      this.showDeactivate.set(true);
      break;
    case 'Retired':
      this.showActivate.set(true);
      break;
  }
}

async openConfirmation(action: string): Promise<boolean> {
  const data: ConfirmationDialogData = {
    title: `${action} Confirmation`,
    message: `<p>Are you sure you want to <strong>${action.toLowerCase()}</strong> this product?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const confirmed = await firstValueFrom(this.confirmationService.openConfirmation(data));
  return confirmed ?? false;
}

updateTab(event:MatButtonToggleChange):void {
  const tabName = event.value as 'product' | 'strengths' | 'priceList' | 'commissionRate';
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

  formatFieldName(fieldName: string): string {
    const formatted = fieldName.replace(/([A-Z])/g, ' $1').trim();
    return formatted.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
  }

}
