import { ChangeDetectorRef, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';

import { Router, ActivatedRoute } from '@angular/router';
import { finalize, firstValueFrom, forkJoin } from 'rxjs';

import { PatientService } from '../../patient/patient.services';
import { UserManagementService } from '../../user-management/user-management.service';
import { PharmacyService } from '../../pharmacy/pharmacy.services';
import { PriceListItemService } from '../../price-list-items/price-list-item.services';

import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { PriceListItemsByPharmacyIdResponseDto } from '../../price-list-items/model/pricelistitem-by-pharmacyid-response.model';
import { PharmacyShippingMethodResponseDto } from '../../pharmacy/model/pharmacy-shipping-method.model';
import { PatientCreditCardResponseDto } from '../../patient/model/patient-credit-card-response';
import { ShippingAddressResponseDto } from '../../patient/model/shipping-address-response.model';
import { ProductLineItemDto } from '../../price-list-items/model/product-transaction.model';

import { CardTypeEnum } from '../../../shared/enums/credit-card-type.enum';

import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { ShippingAddressAddComponent } from '../../patient/shipping-address-add/shipping-address-add';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { ReasonDialogComponent } from '../../../shared/components/reason-dialog/reason-dialog';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { BulkOperationResponseDto } from '../../../shared/models/bulk-operation-response.model';
import { CouponResponse } from '../../proposal/model/coupon-response.model';
import { OrderService } from '../order-service';
import { ProposalService } from '../../proposal/proposal.service';
import { OrderStatus } from '../../../shared/enums/order-status.enus';
import { OrderRequestDto } from '../model/order-request.model';
import { OrderReceipt } from '../order-receipt/order-receipt';
import { Prescription } from '../prescription/prescription';
import { PharmaciesDropdownResponseDto } from '../../pharmacy/model/pharmacies-dropdown-response.model';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatRadioButton, MatRadioGroup, MatRadioModule } from '@angular/material/radio';
import { OrderPaymentRequestDto } from '../model/order-payment-request.model';
import { RefundAmountDialogData, RefundAmountDialogComponent } from '../refund-amount-dialog/refund-amount-dialog';
import { SettleUpDialogComponent, SettleUpDialogData } from '../settle-up-dialog/settle-up-dialog';

interface PatientResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  outstandingRefundBalance: number;
}

@Component({
  selector: 'app-order-add',
  templateUrl: './order-add.html',
  styleUrls: ['./order-add.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatCheckboxModule,
    ShippingAddressAddComponent,
    FullPageLoaderComponent,
    FormsModule,
    MatRadioButton,
    MatRadioGroup,
    MatRadioModule
  ]
})
export class OrderAddComponent implements OnInit {
  private patientService = inject(PatientService);
  private userManagementService = inject(UserManagementService);
  private pharmacyService = inject(PharmacyService);
  private orderService = inject(OrderService);
  private priceListItemService = inject(PriceListItemService);
  private notificationService = inject(NotificationService);
  private confirmationDialogService = inject(ConfirmationDialogService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private userAccountService = inject(UserAccountService);
  private readonly dialog = inject(MatDialog);
  private readonly proposalService = inject(ProposalService);
  private readonly cdr = inject(ChangeDetectorRef);
  trackByAddressId = (_: number, a: ShippingAddressResponseDto) => a.id;
  trackByShippingMethodId = (_: number, m: PharmacyShippingMethodResponseDto) => m.id;
  isPrintReceipt = signal(false);
  orderForm!: FormGroup;
  productControl = new FormControl();

  orderId: string | null = null;
  isEditMode = signal(false);
  isDeleted = false;
  isFromSubmitOrder = false;
  isReadOnlyMode = signal(false);
  isLab = signal(false);

  pharmacies: PharmaciesDropdownResponseDto[] = [];
  filteredPharmacies: PharmaciesDropdownResponseDto[] = [];
  patients: DropDownResponseDto[] = [];
  filteredPatients: DropDownResponseDto[] = [];
  counselors: DropDownResponseDto[] = [];
  filteredCounselors: DropDownResponseDto[] = [];
  coupons: CouponResponse[] = [];
  filteredCoupons: CouponResponse[] = [];

  courierServicesList : DropDownResponseDto[] = []

  selectedCoupon: CouponResponse | null = null;

  pharmacyShippingMethods: PharmacyShippingMethodResponseDto[] = [];
  filteredShippingMethods: PharmacyShippingMethodResponseDto[] = [];
  selectedShippingMethod: PharmacyShippingMethodResponseDto | null = null;
  toggleModifyShippingValue = false;
  customShippingValue = 0;

  patientCreditCards: PatientCreditCardResponseDto[] = [];
  selectedCreditCard: PatientCreditCardResponseDto | null = null;
  enableCreditCardPayment = false;
  creditCardSurchargeRate = 0.0399;

  shippingAddresses: ShippingAddressResponseDto[] = [];
  selectedShippingAddressId: string | null = null;
  isAddressVerified = false;
  addressVerificationMap: Map<string, boolean> = new Map();

  showShippingAddressModal = false;
  editingShippingAddressId?: string;
  currentPatientId: string | null = null;
  isUrlHasPatientId = false;
  OrderStatus = OrderStatus;
  pharmacyProducts: PriceListItemsByPharmacyIdResponseDto[] = [];
  filteredProducts: PriceListItemsByPharmacyIdResponseDto[] = [];
  selectedProducts: ProductLineItemDto[] = [];
  displayedColumns: string[] = [];

  isLoadingProducts = signal(false);
  isLoadingAddresses = signal(false);
  isLoadingCreditCards = signal(false);
  isLoadingPage = signal(true);
  isSubmitting = signal(false);
  isPaymentModeLocked = signal(false);

  doctorsList: DropDownResponseDto[] = [];
  filteredDoctorsList: DropDownResponseDto[] = [];
  isLoadingDoctor: boolean = false;

  outstandingRefundBalance = signal<number>(0);
  outstandingBalanceType = signal<'positive' | 'negative' | 'none'>('none');
  currentPatient: PatientResponseDto | null = null;
  hasRefundAmount = signal<boolean>(false);
  settledAmount = signal<number>(0);
  lastSettlementDate = signal<Date | null>(null);


  constructor(private fb: FormBuilder) { }

  mathMax(a: number, b: number): number { return Math.max(a, b); }
  mathMin(a: number, b: number): number { return Math.min(a, b); }

  ngOnInit() {
    this.initForm();
    this.setupFormSubscriptions();
    this.activatedRoute.paramMap.subscribe(params => {
      this.orderId = params.get('id');
      this.currentPatientId = params.get('patientId');
      this.isEditMode.set(this.orderId !== null);
      this.loadMasterData();
      if (this.currentPatientId) {
        this.isUrlHasPatientId = true;
        this.orderForm.get('patientId')?.disable({ emitEvent: false });
      }
    });
    if (!this.isEditMode()) {
      this.displayedColumns = ['productName', 'productType', 'protocol', 'togglePerUnitPrice', 'perUnitPrice', 'quantity', 'finalAmount', 'actions'];
    } else {
      this.displayedColumns = ['productName', 'productType', 'protocol', 'perUnitPrice', 'quantity', 'finalAmount', 'actions'];
    }
  }
  initForm() {
    this.orderForm = this.fb.group({
      id: [''],
      number:[''],
      name: ['', [Validators.required, Validators.minLength(3)]],
      patientId: [null, Validators.required],
      pharmacyId: [null, Validators.required],
      counselorId: [null, Validators.required],
      physicianId: [null],
      couponId: [null],
      pharmacyOrderNumber: [null],
      signed: [null],
      trackingNumber: [null],
      shippingMethodId: [null],
      shippingAddressId: [null],
      status: [OrderStatus.New, Validators.required],
      courierServiceId: [null],
      therapyExpiration: [null],
      lastOfficeVisit: [null],
      description: [''],
      isReadyForLifeFile: [{ value: false, disabled: true }],
      isGenrateCommision: [null],
      paymentMode: [null],
      orderPaidDate: [null],
      orderFulFilled: [null],
      totalOnCommissionApplied: [null],
      commission: [null],
      refundAmount: [null],
      isActive: [null]
    });
    this.orderForm.get('status')?.disable();
    this.orderForm.get('number')?.disable();
  }

  private async loadMasterData() {
    forkJoin({
      pharmacies: this.pharmacyService.getAllActivePharmacies(),
      patients: this.patientService.getAllActivePatients(),
      counselors: this.userManagementService.getAllActiveSalesPerson(),
      coupons: this.proposalService.getCouponsForDropDown(),
      doctors: this.userManagementService.getAllActiveDoctors(),
      courierServices:this.orderService.getAllActiveCourierServices()
    })
      .pipe(finalize(() => this.isLoadingPage.set(false)))
      .subscribe({
        next: ({ pharmacies, patients, counselors, coupons, doctors,courierServices }) => {
          this.pharmacies = pharmacies;
          this.filteredPharmacies = [...pharmacies];
          this.patients = patients;
          this.filteredPatients = [...patients];
          this.counselors = counselors;
          this.filteredCounselors = [...counselors];
          this.coupons = coupons;
          this.filteredCoupons = [...coupons];
          this.doctorsList = doctors;
          this.filteredDoctorsList = [...doctors];
          this.courierServicesList = courierServices;

          if (this.orderId) {
            this.loadOrderData();
          } else if (this.currentPatientId) {
            this.handlePatientSelection(this.currentPatientId);
          } else {
            this.markFormPristineAndUntouched();
          }
        },
        error: (err) => {
          console.error('Failed to load reference data:', err);
          this.notificationService.showSnackBar(
            'Failed to load reference data',
            'failure'
          );
        },
      });
  }

  private async loadOrderData() {
    try {
      const order = await firstValueFrom(this.orderService.getOrderById(this.orderId!));
      if (order && order.isPharmacyActive) {
        await this.populateFormFromOrder(order);

        const st = order.status || OrderStatus.Completed;
        if (st === OrderStatus.New || st === OrderStatus.LifeFileError || st === OrderStatus.LifeFileSuccess) {
          this.isReadOnlyMode.set(false);
        } else {
          this.isReadOnlyMode.set(true);
        }
      } else {
        // Show confirmation dialog if pharmacy is inactive or order is not present
        const confirmed = await firstValueFrom(
          this.confirmationDialogService.openConfirmation({
            title: 'Pharmacy Inactive',
            message: 'The pharmacy associated with this order is inactive. Do you want to proceed?',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
          })
        );

        if (confirmed) {
          if (order) {
            await this.populateFormFromOrder(order);
            this.isReadOnlyMode.set(true);
          } else {
            this.notificationService.showSnackBar('Order data not available', 'normal');
          }
        } else {
          this.router.navigate(['/orders/view']);
        }
      }
    } catch (error) {
      console.error('Failed to load order data:', error);
      this.notificationService.showSnackBar('Failed to load order data', 'failure');
    }
  }

  private async populateFormFromOrder(order: any): Promise<void> {
    if (order.refundAmount && order.refundAmount > 0) {
      this.hasRefundAmount.set(true);
    }
    if (order.settledAmount) {
      this.settledAmount.set(order.settledAmount);
    }

    if (order.lastSettlementDate) {
      this.lastSettlementDate.set(new Date(order.lastSettlementDate));
    }
    this.orderForm.patchValue({
      id: order.id,
      number: order.orderNumber,
      name: order.name || '',
      status: order.status || OrderStatus.New,
      therapyExpiration: order.therapyExpiration ? new Date(order.therapyExpiration) : null,
      lastOfficeVisit: order.lastOfficeVisit ? new Date(order.lastOfficeVisit) : null,
      description: order.description || '',
      courierServiceId: order.courierServiceId || null,
      trackingNumber: order.trackingNumber || null,
      pharmacyOrderNumber: order.pharmacyOrderNumber || null,
      isReadyForLifeFile: order.isReadyForLifeFile || null,
      isGenrateCommision: order.isGenrateCommision || null,
      paymentMode: order.isOrderPaid ? 'orderPaid' : (order.isCashPayment ? 'cashPayment' : null),
      signed: order.signed || null,
      commission: order.commission,
      totalOnCommissionApplied: order.totalOnCommissionApplied,
      orderPaidDate: order.orderPaidDate ? new Date(order.orderPaidDate) : null,
      orderFulFilled: order.orderFulFilled ? new Date(order.orderFulFilled) : null,
      refundAmount: order.refundAmount || null,
      settledAmount: order.settledAmount || null,
      lastSettlementDate: order.lastSettlementDate ? new Date(order.lastSettlementDate) : null,
      isActive: order.isActive
    });

    if (order.patientId) {
      const patient = this.patients.find(p => p.id === order.patientId);
      if (patient) {
        this.orderForm.patchValue({ patientId: patient });
        this.currentPatientId = String(patient.id);
        await this.loadCreditCards(String(patient.id));
        await this.loadPatientDetails(String(patient.id));
      }
    }

    if (order.pharmacyId) {
      const pharmacy = this.pharmacies.find(p => p.id === order.pharmacyId);
      if (pharmacy) {
        this.orderForm.patchValue({ pharmacyId: pharmacy });
        await this.loadPharmacyProducts(String(pharmacy.id));
        if (!pharmacy.isLab)
          await this.loadPharmacyShippingMethods(String(pharmacy.id));
        else this.isLab.set(true);
      }
    }

    if (order.counselorId) {
      const counselor = this.counselors.find(c => c.id === order.counselorId);
      if (counselor) {
        this.orderForm.patchValue({ counselorId: counselor });
      }
    }

    if (order.physicianId) {
      const physician = this.filteredDoctorsList.find(c => c.id === order.physicianId);
      if (physician) {
        this.orderForm.patchValue({ physicianId: physician });
      }
    }

    if (order.couponId) {
      const coupon = this.coupons.find(c => c.id === order.couponId);
      if (coupon) {
        this.orderForm.patchValue({ couponId: coupon });
        this.selectedCoupon = coupon;
      }
    }

    if (order.pharmacyShippingMethodId) {
      this.selectedShippingMethod =
        this.pharmacyShippingMethods.find(m => m.id === order.pharmacyShippingMethodId) || null;

      if (this.selectedShippingMethod?.name !== 'Pick Up' && this.currentPatientId) {
        await this.loadAddresses(this.currentPatientId);
      }
    }

    this.selectedShippingAddressId = order.shippingAddressId || null;
    if (this.isEditMode() && this.selectedShippingAddressId) {
      this.shippingAddresses = this.shippingAddresses.filter(x => x.id == this.selectedShippingAddressId);
    }
    this.isAddressVerified = !!order.isAddressVerified;

    if (this.selectedShippingAddressId) {
      this.addressVerificationMap.set(this.selectedShippingAddressId, this.isAddressVerified);
      this.orderForm.patchValue({ shippingAddressId: this.selectedShippingAddressId });
    }

    this.selectedCreditCard = order.patientCreditCardId
      ? (this.patientCreditCards.find(card => card.id === order.patientCreditCardId) || null)
      : null;
    this.enableCreditCardPayment = !!order.patientCreditCardId;

    this.customShippingValue = order.deliveryCharge || 0;
    if (this.selectedShippingMethod && order.deliveryCharge) {
      const originalShippingCost = parseFloat(this.selectedShippingMethod.value) || 0;
      this.toggleModifyShippingValue = order.deliveryCharge !== originalShippingCost;
    }

    let allProducts = order.orderDetails || [];
    let activeProducts = allProducts.filter((detail: any) => detail.isActive !== false);

    if (activeProducts.length < allProducts.length) {
      const removedProducts = allProducts
        .filter((detail: any) => detail.isActive === false)
        .map((detail: any) => detail.productName)
        .join(', ');

      this.notificationService.showSnackBar(
        `${allProducts.length - activeProducts.length} inactive product(s) were removed: ${removedProducts}`,
        'normal'
      );
    }

    this.selectedProducts = activeProducts.map((detail: any) => ({
      id: detail.id,
      productId: detail.productId,
      productPharmacyPriceListItemId: detail.productPharmacyPriceListItemId,
      productName: detail.productName,
      amount: detail.amount || 0,
      status: 1,
      isColdStorageProduct: detail.isColdStorageProduct || false,
      quantity: detail.quantity || 1,
      finalAmount: detail.amount || 0,
      perUnitPrice: detail.perUnitAmount || detail.amount || 0,
      togglePerUnitPrice: false,
      productType: detail.productType || '',
      protocol: detail.protocol || ''
    }));

    this.updateShippingMethodsForColdStorage();
    this.markFormPristineAndUntouched();
    this.cdr.detectChanges();
  }

  private async loadPatientDetails(patientId: string): Promise<void> {
    try {
      // Assuming you have a method to get patient by ID
      const patient = await firstValueFrom(this.patientService.getPatientById(patientId));
      if (patient) {
        this.currentPatient = patient;
        const balance = patient.outstandingRefundBalance || 0;
        this.outstandingRefundBalance.set(balance);
        if (balance < 0) {
          this.outstandingBalanceType.set('negative');
        } else if (balance > 0) {
          this.outstandingBalanceType.set('positive');
        } else {
          this.outstandingBalanceType.set('none');
        }
      }
    } catch (error) {
      console.error('Failed to load patient details:', error);
    }
  }

  hasSettledAmount(): boolean {
    return this.settledAmount() !== 0;
  }

  getSettledAmount(): number {
    return this.settledAmount();
  }

  getLastSettlementDate(): Date | null {
    return this.lastSettlementDate();
  }

  onPhysicianSelected(selectedPhysician: DropDownResponseDto) {
    this.orderForm.patchValue({
      physicianId: selectedPhysician.id
    });
  }

  private reinitializeForm(): void {
    this.resetAllState();
    this.initForm();
    this.setupFormSubscriptions();
    this.loadMasterData();
  }

  private resetAllState(): void {
    this.pharmacies = [];
    this.filteredPharmacies = [];
    this.patients = [];
    this.filteredPatients = [];
    this.counselors = [];
    this.filteredCounselors = [];
    this.coupons = [];
    this.filteredCoupons = [];
    this.selectedCoupon = null;
    this.selectedShippingMethod = null;
    this.toggleModifyShippingValue = false;
    this.customShippingValue = 0;
    this.patientCreditCards = [];
    this.selectedCreditCard = null;
    this.enableCreditCardPayment = false;
    this.shippingAddresses = [];
    this.selectedShippingAddressId = null;
    this.isAddressVerified = false;
    this.addressVerificationMap.clear();
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;
    this.currentPatientId = null;
    this.pharmacyProducts = [];
    this.filteredProducts = [];
    this.selectedProducts = [];
    this.productControl.setValue('');
    this.isLoadingProducts.set(false);
    this.isLoadingAddresses.set(false);
    this.isLoadingCreditCards.set(false);
    this.isLoadingPage.set(true);
    this.isSubmitting.set(false);
    this.isReadOnlyMode.set(false);
    this.isDeleted = false;
    this.isPaymentModeLocked.set(false);
    this.outstandingBalanceType.set('none');
  }

  private async confirmAction(title: string, message: string): Promise<boolean> {
    return await firstValueFrom(
      this.confirmationDialogService.openConfirmation({
        title,
        message,
        confirmButtonText: 'Yes',
        cancelButtonText: 'Cancel'
      })
    );
  }
  private handlePatientSelection(patientValue: any) {
    let patientId: string | null = null;

    if (typeof patientValue === 'object' && patientValue?.id) {
      patientId = String(patientValue.id);
    } else if (typeof patientValue === 'string' && patientValue) {
      patientId = patientValue;

      const matchingPatient = this.patients.find(p => String(p.id) === patientId);
      if (matchingPatient) {
        this.orderForm.patchValue({ patientId: matchingPatient }, { emitEvent: false });
      }
    }

    if (patientId) {
      this.currentPatientId = patientId;
      this.loadCreditCards(patientId);
      this.loadAddresses(patientId);
      this.loadPatientDetails(patientId);
    } else {
      this.resetPatientData();
    }
  }
  setupFormSubscriptions() {
    this.orderForm.get('pharmacyId')?.valueChanges.subscribe(pharmacy => {
      if (pharmacy?.id) {

        this.selectedProducts = [];
        this.selectedShippingMethod = null;
        this.toggleModifyShippingValue = false;
        this.customShippingValue = 0;
        this.resetShippingAddress();
        this.loadPharmacyProducts(String(pharmacy.id));
        if (pharmacy.isLab)
          this.isLab.set(true);
        else {
          this.loadPharmacyShippingMethods(String(pharmacy.id));
          this.isLab.set(false);
        }
      } else {
        this.resetPharmacyData();
      }
    });
    this.orderForm.get('patientId')?.valueChanges.subscribe(patientValue => {
      this.handlePatientSelection(patientValue);
    });
    // this.orderForm.get('patientId')?.valueChanges.subscribe(patient => {
    //   if (patient?.id) {
    //     this.currentPatientId = String(patient.id);
    //     this.loadCreditCards(String(patient.id));
    //     this.loadAddresses(String(patient.id));
    //   } else {
    //     this.resetPatientData();
    //   }
    // });
    //   this.orderForm.get('isOrderPaid')?.valueChanges.subscribe(isPaid => {
    //   if (isPaid) {
    //     this.disablePaymentOptions();
    //   } else {
    //     this.enablePaymentOptions();
    //   }
    // });

    this.orderForm.get('paymentMode')?.valueChanges.subscribe(paymentMode => {
      this.handlePaymentModeChange(paymentMode);
    });

    // Handle Cash Payment changes
    this.orderForm.get('isCashPayment')?.valueChanges.subscribe(isCash => {
      if (isCash) {
        this.enableCreditCardPayment = false;
        this.selectedCreditCard = null;
        //this.orderForm.get('isOrderPaid')?.reset();
        this.orderForm.get('patientCreditCardId')?.reset();
        this.orderForm.get('patientCreditCardId')?.disable();

        this.notificationService.showSnackBar('Cash payment selected.', 'success');
      } else if (!this.orderForm.get('isOrderPaid')?.value) {
        this.orderForm.get('patientCreditCardId')?.enable();
      }
    });

    this.orderForm.get('couponId')?.valueChanges.subscribe(coupon => {
      this.selectedCoupon = coupon;
    });

    this.orderForm.get('shippingMethodId')?.valueChanges.subscribe(method => {
      if (!method || method.name === 'Pick Up') this.resetShippingAddress();
    });

    this.orderForm.get('shippingAddressId')?.valueChanges.subscribe(addressId => {
      if (addressId && addressId !== this.selectedShippingAddressId) {
        this.selectedShippingAddressId = addressId;
      }
    });
  }

  private handlePaymentModeChange(paymentMode: string | null): void {
    const isReadyForLifeFileControl = this.orderForm.get('isReadyForLifeFile');
    const isGenrateCommisionControl = this.orderForm.get('isGenrateCommision');
    if (paymentMode === 'orderPaid' || paymentMode === 'cashPayment') {
      isReadyForLifeFileControl?.enable();
      isGenrateCommisionControl?.enable();

    } else {
      isReadyForLifeFileControl?.disable();
      isReadyForLifeFileControl?.setValue(false);
      isGenrateCommisionControl?.disable();
      isGenrateCommisionControl?.setValue(false);
    }
  }
  // Reset helpers
  resetPharmacyData() {
    this.pharmacyProducts = [];
    this.filteredProducts = [];
    this.selectedProducts = [];
    this.pharmacyShippingMethods = [];
    this.filteredShippingMethods = [];
    this.selectedShippingMethod = null;
    this.resetShippingAddress();
    this.productControl.setValue('');
  }

  resetPatientData() {
    //this.currentPatientId = null;
    this.patientCreditCards = [];
    this.selectedCreditCard = null;
    this.enableCreditCardPayment = false;
    this.shippingAddresses = [];
    this.addressVerificationMap.clear();
    this.resetShippingAddress();
  }

  resetShippingAddress() {
    this.selectedShippingAddressId = null;
    this.isAddressVerified = false;
    this.orderForm.get('shippingAddressId')?.setValue(null);
  }

  // Utility
  preventDateInput(event: KeyboardEvent): void { event.preventDefault(); }

  private async loadAddresses(patientId: string): Promise<void> {
    this.isLoadingAddresses.set(true);
    try {
      const addresses = await firstValueFrom(this.patientService.getAllAddressBasedOnPatientId(patientId));
      this.shippingAddresses = addresses?.filter(addr => addr.isActive) || [];
      if (!this.selectedShippingAddressId && this.shippingAddresses.length > 0) {
        const def = this.shippingAddresses.find(addr => addr.isDefaultAddress === true);
        if (def) {
          this.selectedShippingAddressId = def.id;
          const addressControl = this.orderForm.get('shippingAddressId');
          if (addressControl) {
            addressControl.setValue(def.id);
            addressControl.markAsDirty();
            addressControl.markAsTouched();
            addressControl.updateValueAndValidity();
          }
          this.orderForm.patchValue({ shippingAddressId: def.id });
          this.isAddressVerified = false;
          this.addressVerificationMap.set(def.id, false);
          setTimeout(() => this.orderForm.updateValueAndValidity(), 100);
        }
      }
    } catch (err) {
      console.error('Failed to load addresses:', err);
      this.shippingAddresses = [];
      this.notificationService.showSnackBar('Failed to load addresses', 'failure');
    } finally {
      this.isLoadingAddresses.set(false);
    }
  }
  filterPhysicians(value: string): void {
    if (!value) {
      this.filteredDoctorsList = this.doctorsList || [];
      return;
    }
    const filterValue = value.toLowerCase();
    this.filteredDoctorsList = this.doctorsList.filter((doc: any) =>
      doc.value.toLowerCase().includes(filterValue)
    );
  }

  displayPhysician(physician: any): string {
    return physician && physician.value ? physician.value : '';
  }
  private async loadPharmacyProducts(pharmacyId: string): Promise<void> {
    this.isLoadingProducts.set(true);
    try {
      const data = await firstValueFrom(
        this.priceListItemService.getAllActivePriceListItemsByPharmacyId(pharmacyId)
      );
      this.pharmacyProducts = data;
      this.filteredProducts = [...data];
    } catch (err) {
      console.error('Failed to load products:', err);
      this.pharmacyProducts = [];
      this.filteredProducts = [];
      this.notificationService.showSnackBar('Failed to load products', 'failure');
    } finally {
      this.isLoadingProducts.set(false);
    }
  }

  private async loadCreditCards(patientId: string): Promise<void> {
    this.isLoadingCreditCards.set(true);
    try {
      const data = await firstValueFrom(this.patientService.getCreditCardByPatientId(patientId));
      this.patientCreditCards = data.filter(card => card.isActive);
    } catch (err) {
      console.error('Failed to load credit cards:', err);
      this.patientCreditCards = [];
      this.notificationService.showSnackBar('Failed to load credit cards', 'failure');
    } finally {
      this.isLoadingCreditCards.set(false);
    }
  }

  private async loadPharmacyShippingMethods(pharmacyId: string): Promise<void> {
    try {
      const data = await firstValueFrom(this.pharmacyService.getAllPharmacyShippingMethods(pharmacyId));
      if (data && data.length > 0) {
        this.pharmacyShippingMethods = data;
        this.updateShippingMethodsForColdStorage();
      } else {
        this.confirmationDialogService.openConfirmation({
          title: 'No Shipping Methods',
          message: 'No shipping methods found. Add at least one!',
          confirmButtonText: 'Ok',
          showCancelButton: false
        }).subscribe(() => { });
      }
    } catch (err) {
      console.error('Failed to load shipping methods:', err);
      this.pharmacyShippingMethods = [];
      this.filteredShippingMethods = [];
      this.notificationService.showSnackBar('Failed to load shipping methods', 'failure');
    }
  }

  // Validations
  hasColdStorageProducts(): boolean { return this.selectedProducts.some(p => p.isColdStorageProduct); }
  hasSelectedProducts(): boolean { return this.selectedProducts.length > 0; }
  hasSelectedShippingMethod(): boolean { return this.selectedShippingMethod !== null; }
  hasCreditCards(): boolean { return this.patientCreditCards.length > 0; }
  hasSelectedCoupon(): boolean { return this.selectedCoupon !== null; }
  shouldShowAddressSelection(): boolean {
    return this.selectedShippingMethod !== null && this.selectedShippingMethod.name !== 'Pick Up';
  }
  hasShippingAddresses(): boolean { return this.shippingAddresses.length > 0; }
  isAddressVerificationRequired(): boolean {
    return this.shouldShowAddressSelection() && this.selectedShippingAddressId !== null;
  }
  canSubmitForm(): boolean {
    const basicValidation = this.orderForm.valid && this.selectedProducts.length > 0;
    const addressValidation = !this.shouldShowAddressSelection() ||
      (!!this.selectedShippingAddressId && this.isAddressVerified);
    return basicValidation && addressValidation;
  }

  // Product Management
  updateShippingMethodsForColdStorage() {
    const hasCold = this.hasColdStorageProducts();
    if (hasCold) {
      this.filteredShippingMethods = this.pharmacyShippingMethods.filter(m =>
        m.name === 'OverNight Shipping' || m.name === 'Pick Up'
      );
      this.notificationService.showSnackBar('Shipping options limited due to cold storage products', 'normal');
    } else if (this.isEditMode() && this.selectedShippingMethod) {
      this.filteredShippingMethods = this.pharmacyShippingMethods.filter(x => x.id == this.selectedShippingMethod?.id);
    } else {
      this.filteredShippingMethods = [...this.pharmacyShippingMethods];
    }
    if (this.selectedShippingMethod &&
      !this.filteredShippingMethods.some(m => m.id === this.selectedShippingMethod?.id)) {
      this.selectedShippingMethod = null;
      this.customShippingValue = 0;
      this.resetShippingAddress();
    }
  }

  onProductSelected(event: any) {
    const product = event.option.value;
    if (!product) return;

    if (this.selectedProducts.some(p => p.productPharmacyPriceListItemId === product.id)) {
      this.notificationService.showSnackBar('This product is already added to the order', 'normal');
      this.productControl.setValue('');
      return;
    }

    const selectedProduct: ProductLineItemDto = {
      id: null as any,
      productPharmacyPriceListItemId: product.id,
      productId: product.productId,
      productName: product.productName,
      amount: product.amount,
      status: product.status,
      isColdStorageProduct: product.isColdStorageProduct,
      quantity: 1,
      finalAmount: product.amount,
      perUnitPrice: product.amount,
      togglePerUnitPrice: false,
      productType: product.productType,
      protocol: product.protocol
    };

    this.selectedProducts = [...this.selectedProducts, selectedProduct];
    this.productControl.setValue('');
    this.updateShippingMethodsForColdStorage();
    this.notificationService.showSnackBar(`${product.productName} added to order`, 'success');
  }
  onProtocolChanged(productId: string | number, event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    const product = this.selectedProducts.find(p => p.id === productId);
    if (product) {
      product.protocol = value;
    }
  }





  updateQuantity(productId: string, quantity: number) {
    const product = this.selectedProducts.find(p => p.id === productId);
    if (!product) return;
    const validQuantity = this.mathMax(1, quantity || 1);
    if (quantity < 1) {
      this.notificationService.showSnackBar('Quantity cannot be less than 1', 'normal');
    }
    product.quantity = validQuantity;
    product.finalAmount = product.perUnitPrice * validQuantity;
  }

  updatePerUnitPrice(productId: string, perUnitPrice: number) {
    const product = this.selectedProducts.find(p => p.id === productId);
    if (!product || !product.togglePerUnitPrice) return;
    const validPrice = this.mathMax(0, perUnitPrice || 0);
    if (perUnitPrice < 0) {
      this.notificationService.showSnackBar('Price cannot be negative', 'normal');
    }
    product.perUnitPrice = validPrice;
    product.finalAmount = validPrice * product.quantity;
  }

  togglePerUnitPriceModification(productId: string, isEnabled: boolean) {
    const product = this.selectedProducts.find(p => p.id === productId);
    if (!product) return;

    product.togglePerUnitPrice = isEnabled;

    if (!isEnabled) {
      product.finalAmount = product.perUnitPrice * product.quantity;
    }

    const message = isEnabled
      ? `Protocol editing enabled for ${product.productName}`
      : `Protocol editing disabled for ${product.productName}`;
    this.notificationService.showSnackBar(message, 'success');
  }


  removeProduct(productId: string): void {
    this.selectedProducts = this.selectedProducts.filter(p => p.id !== productId);
    this.updateShippingMethodsForColdStorage();
    this.notificationService.showSnackBar('Product removed (changes pending save)', 'normal');
  }

  // Calculations
  getProductSubTotal(): number {
    return this.selectedProducts.reduce((total, product) => total + product.finalAmount, 0);
  }

  getCreditCardSurcharge(): number {
    if (!this.enableCreditCardPayment || !this.selectedCreditCard) return 0;
    const subtotal = this.getProductSubTotal() + (this.customShippingValue || 0);
    return subtotal * this.creditCardSurchargeRate;
  }

  // getGrandTotal(): number {
  //   const productTotal = this.getProductSubTotal();
  //   const shippingCost = this.customShippingValue || 0;
  //   const creditCardSurcharge = this.getCreditCardSurcharge();
  //   const couponDiscount = this.getCouponDiscount();
  //   let grandTotal = productTotal + shippingCost + creditCardSurcharge - couponDiscount;

  //   // Apply settled amount logic
  //   if (this.hasSettledAmount()) {
  //     // If outstanding balance is 0 but we have settled amount, treat it as negative balance settlement
  //     if (this.outstandingRefundBalance() === 0 && this.settledAmount() > 0) {
  //       // This means we settled a negative balance to zero, so add the settled amount
  //       grandTotal += this.getSettledAmount();
  //     } else if (this.outstandingBalanceType() === 'negative') {
  //       // Negative balance: add settled amount to grand total
  //       grandTotal += this.getSettledAmount();
  //     } else {
  //       // Positive balance: subtract settled amount from grand total
  //       grandTotal -= this.getSettledAmount();
  //     }
  //   }

  //   return this.mathMax(0, grandTotal);
  // }

  getGrandTotal(): number {
    const productTotal = this.getProductSubTotal();
    const shippingCost = this.customShippingValue || 0;
    const creditCardSurcharge = this.getCreditCardSurcharge();
    const couponDiscount = this.getCouponDiscount();
    const settledAmount = this.getSettledAmount();
    console.log('settledAmount ', settledAmount);
    let grandTotal = productTotal + shippingCost + creditCardSurcharge + (settledAmount) - couponDiscount;

    // Apply settled amount logic for outstanding balance
    // if (this.hasSettledAmount()) {
    //   if (this.outstandingBalanceType() === 'negative') {
    //     // Negative balance (patient owes money): add settled amount to grand total
    //     grandTotal += this.getSettledAmount();
    //   } else {
    //     // Positive balance (we owe refund): subtract settled amount from grand total
    //     grandTotal -= this.getSettledAmount();
    //   }
    // }

    return this.mathMax(0, grandTotal);
  }

  getCouponDiscount(): number {
    if (!this.selectedCoupon) return 0;
    const subtotal = this.getProductSubTotal() + (this.customShippingValue || 0);
    let discount = 0;
    const fixedDiscount = this.selectedCoupon.amount || 0;
    const percentageDiscount = this.selectedCoupon.percentage
      ? (subtotal * this.selectedCoupon.percentage / 100)
      : 0;
    if (this.selectedCoupon.amount && this.selectedCoupon.percentage) {
      discount = this.mathMin(fixedDiscount, percentageDiscount);
    } else if (this.selectedCoupon.amount) {
      discount = fixedDiscount;
    } else if (this.selectedCoupon.percentage) {
      discount = percentageDiscount;
    }
    return this.mathMin(discount, subtotal);
  }

  getTotalPriceSavings(): number {
    return this.selectedProducts.reduce((savings, product) => {
      if (product.amount && product.perUnitPrice < product.amount) {
        return savings + ((product.amount - product.perUnitPrice) * product.quantity);
      }
      return savings;
    }, 0);
  }

  // Address Verification
  onAddressVerificationChange(isVerified: boolean) {
    this.isAddressVerified = isVerified;
    if (this.selectedShippingAddressId) {
      this.addressVerificationMap.set(this.selectedShippingAddressId, isVerified);
    }
    const message = isVerified ? 'Address marked as verified' : 'Address marked as unverified';
    this.notificationService.showSnackBar(message, 'success');
  }

  onShippingAddressSelected(addressId: string) {
    const previousAddressId = this.selectedShippingAddressId;
    this.selectedShippingAddressId = addressId;
    this.orderForm.get('shippingAddressId')?.setValue(addressId);

    const hasStoredVerification = this.addressVerificationMap.has(addressId);
    const storedVerificationStatus = this.addressVerificationMap.get(addressId) || false;

    if (previousAddressId && previousAddressId !== addressId) {
      if (hasStoredVerification) {
        this.isAddressVerified = storedVerificationStatus;
        this.notificationService.showSnackBar(
          storedVerificationStatus
            ? 'Address selected - verification status restored (verified)'
            : 'Address selected - verification status restored (unverified)',
          'success'
        );
      } else {
        this.isAddressVerified = false;
        this.notificationService.showSnackBar('Address changed - verification reset. Please verify the new address.', 'normal');
      }
    } else if (!previousAddressId) {
      if (hasStoredVerification) {
        this.isAddressVerified = storedVerificationStatus;
        this.notificationService.showSnackBar(
          storedVerificationStatus
            ? 'Address selected - previously verified'
            : 'Address selected - please verify this address',
          'success'
        );
      } else {
        this.isAddressVerified = false;
        this.notificationService.showSnackBar('Shipping address selected - please verify', 'success');
      }
    } else {
      this.notificationService.showSnackBar('Shipping address confirmed', 'success');
    }
  }

  // Address Modal
  openAddAddressModal() {
    if (!this.currentPatientId) {
      this.notificationService.showSnackBar('Please select a patient first', 'normal');
      return;
    }
    this.editingShippingAddressId = undefined;
    this.showShippingAddressModal = true;
  }

  openEditAddressModal(addressId: string) {
    if (!this.currentPatientId) {
      this.notificationService.showSnackBar('Please select a patient first', 'normal');
      return;
    }
    this.editingShippingAddressId = addressId;
    this.showShippingAddressModal = true;
  }

  onShippingAddressCreated(newAddress: any) {
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;
    if (this.currentPatientId) this.loadAddresses(this.currentPatientId);
    if (newAddress?.id) {
      this.selectedShippingAddressId = newAddress.id;
      this.orderForm.get('shippingAddressId')?.setValue(newAddress.id);
      this.isAddressVerified = false;
      this.addressVerificationMap.set(newAddress.id, false);
      this.notificationService.showSnackBar('New address added - please verify the address', 'success');
    }
  }

  onShippingAddressUpdated(updatedAddress: any) {
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;
    if (this.currentPatientId) this.loadAddresses(this.currentPatientId);

    if (updatedAddress?.id === this.selectedShippingAddressId) {
      this.isAddressVerified = false;
      this.addressVerificationMap.set(updatedAddress.id, false);
      this.notificationService.showSnackBar('Address updated - please re-verify the address', 'normal');
    } else if (updatedAddress?.id) {
      this.addressVerificationMap.set(updatedAddress.id, false);
      this.notificationService.showSnackBar('Address updated successfully', 'success');
    } else {
      this.notificationService.showSnackBar('Address updated successfully', 'success');
    }
  }

  onShippingAddressModalClosed() {
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;
  }

  onShippingMethodSelected(shippingMethod: PharmacyShippingMethodResponseDto) {
    this.selectedShippingMethod = shippingMethod;
    if (!this.toggleModifyShippingValue) {
      this.customShippingValue = parseFloat(shippingMethod.value) || 0;
    }
    if (shippingMethod.name === 'Pick Up') {
      this.resetShippingAddress();
    } else {
      if (this.currentPatientId) this.loadAddresses(this.currentPatientId);
    }
    this.notificationService.showSnackBar(`Shipping method selected: ${shippingMethod.name}`, 'success');
  }

  toggleShippingValueModification(isEnabled: boolean) {
    this.toggleModifyShippingValue = isEnabled;
    if (!isEnabled && this.selectedShippingMethod) {
      this.customShippingValue = parseFloat(this.selectedShippingMethod.value) || 0;
    }
    const message = isEnabled ? 'Custom shipping cost enabled' : 'Custom shipping cost disabled';
    this.notificationService.showSnackBar(message, 'success');
  }

  updateCustomShippingValue(value: number) {
    const validValue = this.mathMax(0, value || 0);
    if (value < 0) {
      this.notificationService.showSnackBar('Shipping cost cannot be negative', 'normal');
    }
    this.customShippingValue = validValue;
  }

  toggleCreditCardPayment(isEnabled: boolean) {
    this.enableCreditCardPayment = isEnabled;

    if (isEnabled) {
      this.orderForm.get('isCashPayment')?.setValue(false, { emitEvent: false });
      this.selectedCreditCard = null;
      this.orderForm.get('patientCreditCardId')?.enable({ emitEvent: false });
    } else {
      this.selectedCreditCard = null;
      this.orderForm.get('patientCreditCardId')?.reset(null, { emitEvent: false });
      this.orderForm.get('patientCreditCardId')?.disable({ emitEvent: false });
    }

    const message = isEnabled
      ? 'Credit card payment enabled.'
      : 'Credit card payment disabled.';
    this.notificationService.showSnackBar(message, 'success');
  }

  onCreditCardSelected(creditCard: PatientCreditCardResponseDto) {
    this.selectedCreditCard = creditCard;
    this.notificationService.showSnackBar('Credit card selected', 'success');
  }

  // Formatting
  getCardTypeName(cardType: number): string { return CardTypeEnum[cardType] || 'Unknown'; }
  getFormattedCardNumber(cardNumber: string): string {
    if (!cardNumber || cardNumber.length < 4) return cardNumber;
    return `**** **** **** ${cardNumber.slice(-4)}`;
  }
  getFormattedExpiryDate(month: number, year: string): string {
    const formattedMonth = month.toString().padStart(2, '0');
    return `${formattedMonth}/${year}`;
  }
  getFormattedAddress(address: ShippingAddressResponseDto): string {
    const parts = [
      address.addressLine1,
      address.city,
      address.stateName,
      address.countryName,
      address.postalCode
    ].filter(part => part && part.trim());
    let formattedAddress = parts.join(', ');
    if (address.isDefaultAddress) formattedAddress += ' (Default)';
    return formattedAddress;
  }

  // Filters
  filterProducts(value: string) {
    const filterValue = (value || '').toLowerCase();
    this.filteredProducts = this.pharmacyProducts.filter(product =>
      product.productName?.toLowerCase().includes(filterValue)
    );
  }
  filterPharmacies(value: string) {
    const filterValue = (value || '').toLowerCase();
    this.filteredPharmacies = this.pharmacies.filter(p =>
      p.name.toLowerCase().includes(filterValue)
    );
  }
  filterPatients(value: string) {
    const filterValue = (value || '').toLowerCase();
    this.filteredPatients = this.patients.filter(p =>
      p.value.toLowerCase().includes(filterValue)
    );
  }
  filterCounselors(value: string) {
    const filterValue = (value || '').toLowerCase();
    this.filteredCounselors = this.counselors.filter(c =>
      c.value.toLowerCase().includes(filterValue)
    );
  }
  filterCoupons(value: string) {
    const filterValue = (value || '').toLowerCase();
    this.filteredCoupons = this.coupons.filter(c =>
      (c.couponName || '').toLowerCase().includes(filterValue)
    );
  }

  // Display fns
  displayProductFn = (product: any): string => product ? product.productName : '';

  displayFn = (item: any): string => {
    if (!item) return '';
    if (item.value) return item.value;
    if (item.name) return item.name;
    if (item.couponName) return item.couponName;
    return '';
  }

  // Status text
  // getStatusDisplayText(statusValue: number): string {
  //   const status = this.statusList.find(s => s.value === statusValue);
  //   return status ? status.name : 'Unknown Status';
  // }

  // Validation helpers
  isFieldInvalid(fieldName: string): boolean {
    const field = this.orderForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }
  getFieldError(fieldName: string): string {
    const field = this.orderForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${this.pascalToSpace(fieldName)} is required`;
      if (field.errors['email']) return 'Please enter a valid email';
      if (field.errors['minlength']) return `${this.pascalToSpace(fieldName)} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['pattern']) return `${this.pascalToSpace(fieldName)} format is invalid`;
    }
    return '';
  }

  private markFormGroupTouched(): void {
    Object.values(this.orderForm.controls).forEach(control => {
      if (control instanceof FormGroup) {
        Object.values(control.controls).forEach(ctrl => ctrl.markAsTouched());
      } else {
        control.markAsTouched();
      }
    });
  }

  private markFormPristineAndUntouched(formGroup: FormGroup = this.orderForm) {
    formGroup.markAsPristine();
    formGroup.markAsUntouched();
    Object.values(formGroup.controls).forEach(control => {
      if (control instanceof FormGroup) {
        this.markFormPristineAndUntouched(control);
      } else {
        control.markAsPristine();
        control.markAsUntouched();
      }
    });
  }

  // Payload builder for OrderRequestDto
  private prepareApiPayload(): OrderRequestDto {
    const formValue = this.orderForm.getRawValue();
    const paymentMode = formValue.paymentMode;
    const isOrderPaid = paymentMode === 'orderPaid';
    const isCashPayment = paymentMode === 'cashPayment';
    const subtotal = this.getProductSubTotal();
    const deliveryCharge = this.customShippingValue || 0;
    const surcharge = this.getCreditCardSurcharge();
    const totalAmount = this.getGrandTotal();
    const couponDiscount = this.getCouponDiscount();

    const payload: OrderRequestDto = {
      name: formValue.name,
      patientId: formValue.patientId?.id || formValue.patientId,
      proposalId: formValue.proposalId || undefined,
      pharmacyId: formValue.pharmacyId?.id || formValue.pharmacyId,
      counselorId: parseInt(formValue.counselorId?.id || formValue.counselorId) || 0,
      physicianId: parseInt(formValue.physicianId?.id || formValue.physicianId) || undefined,
      couponId: formValue.couponId?.id || formValue.couponId || undefined,
      patientCreditCardId: this.selectedCreditCard?.id || undefined,
      pharmacyShippingMethodId: this.selectedShippingMethod?.id!,
      shippingAddressId: this.selectedShippingAddressId || undefined,
      therapyExpiration: formValue.therapyExpiration || undefined,
      lastOfficeVisit: formValue.lastOfficeVisit || undefined,
      status: formValue.status || OrderStatus.New,
      signed: formValue.signed || null,
      subtotal: subtotal,
      totalAmount: totalAmount,
      surcharge: surcharge && surcharge > 0 ? surcharge : undefined,
      couponDiscount: couponDiscount && couponDiscount > 0 ? couponDiscount : undefined,
      commission: formValue.commission || undefined,
      totalOnCommissionApplied: formValue.totalOnCommissionApplied || undefined,
      deliveryCharge: deliveryCharge > 0 ? deliveryCharge : undefined,
      isOrderPaid: isOrderPaid || undefined,
      isCashPayment: isCashPayment || undefined,
      isGenrateCommision: formValue.isGenrateCommision || undefined,
      isReadyForLifeFile: formValue.isReadyForLifeFile || undefined,
      description: formValue.description || undefined,
      orderPaidDate: formValue.orderPaidDate || undefined,
      orderFulFilled: formValue.orderFulFilled || undefined,
      pharmacyOrderNumber: formValue.pharmacyOrderNumber || undefined,
      courierServiceId: formValue.courierServiceId || undefined,

      orderDetails: this.selectedProducts.map(product => ({
        id: product.id,
        productPharmacyPriceListItemId: product.productPharmacyPriceListItemId,
        productId: product.productId,
        quantity: product.quantity,
        amount: product.finalAmount,
        protocol: product.protocol,
        perUnitAmount: product.perUnitPrice,
        totalAmount: product.finalAmount
      }))
    };

    return payload;
  }

  // Save / Submit
  onSaveInformation(): Promise<number> {
    return new Promise((resolve) => {
      if (this.isReadOnlyMode()) {
        this.notificationService.showSnackBar('Form is in readonly mode', 'normal');
        return resolve(0);
      }

      if (this.orderForm.invalid) {
        this.markFormGroupTouched();
        this.notificationService.showSnackBar('Please fill in all required fields correctly.', 'normal');
        return resolve(0);
      }
      if (this.enableCreditCardPayment && !this.selectedCreditCard) {
        this.notificationService.showSnackBar('Credit card is not selected', 'normal');
        return resolve(0);
      }

      this.isSubmitting.set(true);
      const payload = this.prepareApiPayload();
      if (this.isFromSubmitOrder) {
        payload.status = OrderStatus.New;
      }

      if (!this.orderId) {
        this.orderService.createOrder(payload).subscribe({
          next: (res: CommonOperationResponseDto) => {
            this.orderId = res?.id ? String(res.id) : null;
            this.router.navigate(['/order/edit', this.orderId]);
          },
          error: () => {
            this.notificationService.showSnackBar('Failed to create order. Please try again.', 'failure');
            this.isSubmitting.set(false);
            resolve(-1);
          }
        });
      } else {
        this.orderService.updateOrder(this.orderId, payload).subscribe({
          next: () => {
            this.notificationService.showSnackBar('Order updated successfully!', 'success');
            setTimeout(() => this.reinitializeForm(), 1000);
            this.isSubmitting.set(false);
            resolve(1);
          },
          error: () => {
            this.notificationService.showSnackBar('Failed to update order.', 'failure');
            this.isSubmitting.set(false);
            resolve(-1);
          }
        });
      }
    });
  }

  async onSaveAndClose(): Promise<void> {
    const status = await this.onSaveInformation();
    if (status > 0) this.onClose();
  }

  async onSubmit() { await this.onSaveInformation(); }
  //onSubmitOrderClick() { this.isFromSubmitOrder = true; this.onSaveInformation(); }

  // // Navigation
  // onOpenEditMode() {
  //   this.router.navigate(['/orders/edit', this.orderId]);
  // }

  onClickAdd(): void {
    const navigateToAdd = () => {
      this.resetForm();
      this.orderId = null;
      this.isEditMode.set(false);

      if (this.isUrlHasPatientId) {
        this.router.navigate(['/order/add', this.currentPatientId]);
      } else {
        this.currentPatientId = null;
        this.router.navigate(['/order/add']);
      }
    };

    if (this.orderForm.dirty) {
      this.confirmationDialogService.openConfirmation({
        title: 'Unsaved Changes',
        message: 'You have unsaved changes. Are you sure to continue without saving?',
      }).subscribe(confirmed => {
        if (confirmed) navigateToAdd();
      });
    } else {
      navigateToAdd();
    }
  }

  onClose(): void {
    if (this.isReadOnlyMode()) {
      this.goBackToOrderView();
    } else if (this.orderForm.dirty) {
      this.confirmationDialogService.openConfirmation({
        title: 'Unsaved Changes',
        message: 'You have unsaved changes. Are you sure you want to close without saving?',
      }).subscribe(confirmed => {
        if (confirmed) this.goBackToOrderView();
      });
    } else {
      this.goBackToOrderView();
    }
  }

  goBackToOrderView(): void {
    if (this.isUrlHasPatientId && this.currentPatientId) {
      this.router.navigate(['/orders/view', this.currentPatientId]);
    } else {
      this.router.navigateByUrl('/orders/view');
    }
  }


  resetForm(): void {
    this.orderForm.reset();
    this.selectedProducts = [];
    this.selectedCoupon = null;
    this.selectedShippingMethod = null;
    this.selectedCreditCard = null;
    this.selectedShippingAddressId = null;
    this.isAddressVerified = false;
    this.enableCreditCardPayment = false;
    this.customShippingValue = 0;
    this.toggleModifyShippingValue = false;
    this.addressVerificationMap.clear();
    this.resetPharmacyData();
    this.resetPatientData();
    this.orderForm.patchValue({ paymentMode: null });
  }

  // Helpers
  pascalToSpace(str: string): string {
    return str.replace(/([A-Z])/g, ' $1').trim();
  }

  isAddressPreviouslyVerified(addressId: string): boolean {
    return this.addressVerificationMap.get(addressId) || false;
  }

  getAddressVerificationDisplay(addressId: string): string {
    const isVerified = this.addressVerificationMap.get(addressId);
    if (isVerified === undefined) return '';
    return isVerified ? ' ✓ Verified' : ' ⚠ Unverified';
  }

  isDefaultAddress(address: ShippingAddressResponseDto): boolean {
    return address.isDefaultAddress === true;
  }

  getSelectedAddress(): ShippingAddressResponseDto | undefined {
    if (!this.selectedShippingAddressId) return undefined;
    return this.shippingAddresses.find(a => a.id === this.selectedShippingAddressId);
  }

  isSelectedAddressDefault(): boolean {
    const selectedAddress = this.getSelectedAddress();
    return selectedAddress ? this.isDefaultAddress(selectedAddress) : false;
  }

  getSelectedAddressType(): string {
    const selectedAddress = this.getSelectedAddress();
    return selectedAddress?.addressType || 'Address';
  }

  onDeleteClick(): void {
    if (this.isReadOnlyMode()) return;
    this.confirmationDialogService.openConfirmation({
      title: 'Confirm Delete',
      message: `Are you sure you want to delete this order?`,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
      showCancelButton: true,
    }).subscribe(confirmed => {
      if (confirmed) {
        if (!this.orderId) {
          this.notificationService.showSnackBar('Order ID is missing. Cannot delete.', 'failure');
          return;
        }
        this.orderService.deleteOrders([this.orderId]).subscribe({
          next: (res: BulkOperationResponseDto) => {
            if (res.successCount > 0) {
              this.notificationService.showSnackBar(res.message ?? 'Order deleted successfully.', 'success');
              this.onClose();
            } else {
              this.notificationService.showSnackBar(res.message ?? 'Failed to delete order.', 'failure');
            }
          },
          error: () => {
            this.notificationService.showSnackBar('Server error while deleting order.', 'failure');
          }
        });
      }
    });
  }
  onClickCloneOrder(): void {
    if (this.isReadOnlyMode()) return;

    this.confirmationDialogService.openConfirmation({
      title: 'Confirm Clone',
      message: `Are you sure you want to clone this order?`,
      confirmButtonText: 'Clone',
      cancelButtonText: 'Cancel',
      showCancelButton: true,
    }).subscribe(confirmed => {
      if (confirmed) {
        if (!this.orderId) {
          this.notificationService.showSnackBar('Order ID is missing. Cannot clone.', 'failure');
          return;
        }

        this.proposalService.cloneOrder(this.orderId).subscribe({
          next: (res: CommonOperationResponseDto) => {
            if (res.id) {
              this.notificationService.showSnackBar(res.message ?? 'Order cloned successfully.', 'success');
            } else {
              this.notificationService.showSnackBar(res.message ?? 'Failed to clone order.', 'failure');
            }
          },
          error: () => {
            this.notificationService.showSnackBar('Server error while cloning order.', 'failure');
          }
        });
      }
    });
  }
  async onOrderAction(status: OrderStatus): Promise<void> {
    if (!this.orderId) return;

    try {
      this.isSubmitting.set(true);

      if (status === OrderStatus.Cancel_rejected) {
        const reason = await this.promptForReason();
        if (!reason) {
          this.isSubmitting.set(false);
          return;
        }

        await firstValueFrom(
          this.orderService.updateOrderStatus(this.orderId, status, reason)
        );

        this.notificationService.showSnackBar('Order rejected successfully', 'success');
      } else {
        const actionLabel =
          status === OrderStatus.Completed ? 'accept' : 'cancel';

        const confirmed = await this.confirmAction(
          `${actionLabel.charAt(0).toUpperCase() + actionLabel.slice(1)} Order`,
          `Are you sure you want to ${actionLabel} this order?`
        );
        if (!confirmed) {
          this.isSubmitting.set(false);
          return;
        }

        await firstValueFrom(
          this.orderService.updateOrderStatus(this.orderId, status)
        );

        this.notificationService.showSnackBar(
          `Order ${actionLabel}ed successfully`,
          'success'
        );
      }

      this.onClose();
    } catch (error) {
      console.error('Failed to update order status:', error);
      this.notificationService.showSnackBar('Failed to update order', 'failure');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  private promptForReason(): Promise<string | null> {
    return new Promise((resolve) => {
      const dialogRef = this.dialog.open(ReasonDialogComponent, {
        width: '400px',
        data: { title: 'Reject Order', message: 'Please provide a reason for rejection' }
      });

      dialogRef.afterClosed().subscribe((result: string | null) => {
        resolve(result || null);
      });
    });
  }

  openReceiptDialog(isPaidLogo: boolean): void {
    if (this.orderId) {
      this.dialog.open(OrderReceipt, {
        width: '800px',
        data: { orderId: this.orderId, isPaidLogo: isPaidLogo },
        disableClose: false
      });
    } else {
      this.notificationService.showSnackBar(
        'No order to print receipt for.',
        'normal'
      );
    }
  }
  openPrescriptionDialog(isSigned?: boolean): void {
    if (this.orderId) {
      this.dialog.open(Prescription, {
        width: '800px',
        data: { orderId: this.orderId, isSigned: isSigned },
        disableClose: false
      });
    } else {
      this.notificationService.showSnackBar(
        'No order to print receipt for.',
        'normal'
      );
    }
  }

  makeProposalDataDisable() {
    if (this.isEditMode()) {
      this.orderForm.get('name')?.disable({ emitEvent: false });
      this.orderForm.get('patientId')?.disable({ emitEvent: false });
      this.orderForm.get('pharmacyId')?.disable({ emitEvent: false });
      this.orderForm.get('counselorId')?.disable({ emitEvent: false });
      this.orderForm.get('therapyExpiration')?.disable({ emitEvent: false });
      this.orderForm.get('couponId')?.disable({ emitEvent: false });
      this.orderForm.get('physicianId')?.disable({ emitEvent: false });
      this.orderForm.get('description')?.disable({ emitEvent: false });
    } else {
      this.orderForm.get('name')?.enable({ emitEvent: false });
      this.orderForm.get('patientId')?.enable({ emitEvent: false });
      this.orderForm.get('pharmacyId')?.enable({ emitEvent: false });
      this.orderForm.get('counselorId')?.enable({ emitEvent: false });
      this.orderForm.get('therapyExpiration')?.enable({ emitEvent: false });
      this.orderForm.get('couponId')?.enable({ emitEvent: false });
      this.orderForm.get('physicianId')?.enable({ emitEvent: false });
      this.orderForm.get('description')?.enable({ emitEvent: false });
    }
  }

  openRefundAmountDialog(): void {
    if (!this.orderId || !this.currentPatientId) {
      this.notificationService.showSnackBar('Order or patient information is missing', 'normal');
      return;
    }

    const currentRefundAmount = this.orderForm.get('refundAmount')?.value || 0;

    const dialogRef = this.dialog.open(RefundAmountDialogComponent, {
      width: '400px',
      data: {
        orderId: this.orderId,
        patientId: this.currentPatientId,
        currentRefundAmount: currentRefundAmount
      } as RefundAmountDialogData
    });

    dialogRef.afterClosed().subscribe((refundAmount: number | undefined) => {
      if (refundAmount !== undefined && refundAmount !== null) {
        this.refundAmount(refundAmount);
      }
    });
  }
  refundAmount(refundAmount: number): void {
    if (!this.orderId) {
      this.notificationService.showSnackBar('Order ID is missing', 'failure');
      return;
    }

    // if (refundAmount <= 0) {
    //   this.notificationService.showSnackBar('Refund amount must be greater than zero', 'normal');
    //   return;
    // }

    this.isSubmitting.set(true);

    this.orderService.refundOrder(this.orderId, refundAmount).subscribe({
      next: (res) => {
        this.notificationService.showSnackBar(res.message ?? 'Refund processed successfully', 'success');
        this.orderForm.patchValue({ refundAmount: refundAmount });
        this.hasRefundAmount.set(true);
        this.isSubmitting.set(false);
      },
      error: (err) => {
        console.error('Failed to process refund', err);
        this.notificationService.showSnackBar('Failed to process refund', 'failure');
        this.isSubmitting.set(false);
      }
    });
  }

  openSettleUpDialog(): void {
    if (!this.currentPatientId || !this.orderId) {
      this.notificationService.showSnackBar('Patient or order information is missing', 'normal');
      return;
    }

    const absoluteOutstandingBalance = Math.abs(this.outstandingRefundBalance());
    let maxSettleAmount = absoluteOutstandingBalance;
    if (this.outstandingRefundBalance() >= 0) {
      maxSettleAmount = Math.min(absoluteOutstandingBalance, this.getGrandTotal());
    }
    const dialogRef = this.dialog.open(SettleUpDialogComponent, {
      width: '400px',
      data: {
        orderId: this.orderId,
        patientId: this.currentPatientId,
        outstandingRefundBalance: this.outstandingRefundBalance(),
        maxSettleAmount: Math.abs(this.outstandingRefundBalance()),
        orderGrandTotal: this.getGrandTotal()
      } as SettleUpDialogData
    });

    dialogRef.afterClosed().subscribe((result: { settleAmount: number } | undefined) => {
      if (result && result.settleAmount > 0) {
        this.settleOutstandingRefund(result.settleAmount);
      }
    });
  }

  settleOutstandingRefund(settleAmount: number): void {
    if (!this.orderId || !this.currentPatientId) {
      this.notificationService.showSnackBar('Order or patient information is missing', 'failure');
      return;
    }

    if (settleAmount <= 0) {
      this.notificationService.showSnackBar('Settle amount must be greater than zero', 'normal');
      return;
    }

    const maxAllowedSettleAmount = Math.abs(this.outstandingRefundBalance());
    if (settleAmount > maxAllowedSettleAmount) {
      this.notificationService.showSnackBar(`Settle amount cannot exceed $${maxAllowedSettleAmount.toFixed(2)}`, 'normal');
      return;
    }

    this.isSubmitting.set(true);

    this.orderService.settleOutstandingRefund(this.orderId, settleAmount).subscribe({
      next: (res) => {
        this.notificationService.showSnackBar(res.message ?? 'Settle up processed successfully', 'success');

        // Store the original balance type before updating
        const originalWasNegative = this.outstandingBalanceType() === 'negative';
        console.log('this.outstandingRefundBalance() ', this.outstandingRefundBalance());
        // if (this.outstandingRefundBalance() < 0) {
        //   console.log('inside if');
        //   this.outstandingRefundBalance.set(this.outstandingRefundBalance() + settleAmount);
        //   console.log('inside if outstandingRefundBalance ', this.outstandingRefundBalance);
        //   if (this.settledAmount() < 0) {
        //     settleAmount = -1 * settleAmount;
        //   }
        //   this.settledAmount.set(this.settledAmount() + settleAmount);
        // } else {
        //   console.log('inside else');
        //   this.outstandingRefundBalance.set(this.outstandingRefundBalance() - settleAmount);
        //   console.log('inside else outstanding refund ', this.outstandingRefundBalance);
        //   if (this.settledAmount() < 0) {
        //     settleAmount = -1 * settleAmount;
        //   }
        //   this.settledAmount.set(this.settledAmount() + settleAmount);
        // }

        if (this.outstandingRefundBalance() < 0) {
          // Negative balance case
          this.outstandingRefundBalance.set(this.outstandingRefundBalance() + settleAmount);

          if (this.settledAmount() < 0) {
            settleAmount = -1 * settleAmount;
            this.settledAmount.set(this.settledAmount() + settleAmount);
          } else {
            this.settledAmount.set(this.settledAmount() + settleAmount);
          }

        } else {
          // Positive balance case
          this.outstandingRefundBalance.set(this.outstandingRefundBalance() - settleAmount);

          if (this.settledAmount() < 0) {
            this.settledAmount.set(this.settledAmount() + settleAmount);
          } else {
            this.settledAmount.set(this.settledAmount() - settleAmount);
          }
        }

        this.lastSettlementDate.set(new Date());

        // Update balance type based on new value
        if (this.outstandingRefundBalance() < 0) {
          this.outstandingBalanceType.set('negative');
        } else if (this.outstandingRefundBalance() > 0) {
          this.outstandingBalanceType.set('positive');
        } else {
          // When balance becomes 0, preserve the original type for display purposes
          this.outstandingBalanceType.set(originalWasNegative ? 'negative' : 'positive');
        }

        if (this.currentPatient) {
          this.currentPatient.outstandingRefundBalance = this.outstandingRefundBalance();
        }
        this.isSubmitting.set(false);
      },
      error: (err) => {
        console.error('Failed to process settle up', err);
        this.notificationService.showSnackBar('Failed to process settle up', 'failure');
        this.isSubmitting.set(false);
      }
    });
  }

  async onReadyForLifeFileChange(event: any): Promise<void> {
    if (event.checked) {
      // Only open dialog if checkbox is being checked
      const confirmed = await firstValueFrom(
        this.confirmationDialogService.openConfirmation({
          title: 'Confirm Life File',
          message: 'Are you sure want to start lifefile?',
          confirmButtonText: 'Yes',
          cancelButtonText: 'No'
        })
      );

      if (!confirmed) {
        this.orderForm.get('isReadyForLifeFile')?.setValue(false);
      } else {
        await this.onStartLifeFile();
      }
    }
  }
  async onGenerateCommissionChange(event: any): Promise<void> {
    if (event.checked) {
      const confirmed = await firstValueFrom(
        this.confirmationDialogService.openConfirmation({
          title: 'Confirm Commission Generation',
          message: 'Are you sure want to generate commission?',
          confirmButtonText: 'Yes',
          cancelButtonText: 'No'
        })
      );

      if (!confirmed) {
        this.orderForm.get('isGenrateCommision')?.setValue(false, { emitEvent: false });
      } else {
        await this.onGenerateCommission();
      }
    }
  }

  async onGenerateCommission(): Promise<void> {
    try {
      const orderId = this.orderForm.get('id')?.value;
      const res = await firstValueFrom(
        this.orderService.generateCommission(orderId)
      );
      if (res.id) {
        this.notificationService.showSnackBar(res.message, 'success');
        this.reinitializeForm();
      } else {
        this.notificationService.showSnackBar(res.message || 'Failed to generate commission.', 'failure');
        this.orderForm.get('isGenrateCommision')?.setValue(false, { emitEvent: false });
      }
    } catch (error) {
      this.notificationService.showSnackBar(
        'Failed to generate commission.',
        'failure'
      );
      this.orderForm.get('isGenrateCommision')?.setValue(false, { emitEvent: false });
    }
  }


  async onStartLifeFile(): Promise<void> {
    try {
      const orderId = this.orderForm.get('id')?.value;
      const res = await firstValueFrom(
        this.orderService.markReadyToLifeFile(orderId)
      );
      if (res.isSuccess) {
        this.notificationService.showSnackBar(res.message, 'success');
        this.reinitializeForm();
      }
      else {
        this.notificationService.showSnackBar(res.message, 'failure');
        this.orderForm.get('isReadyForLifeFile')?.setValue(false);
      }

    } catch (error) {
      this.notificationService.showSnackBar(
        'Failed to mark ready for LifeFile.',
        'failure'
      );
      this.orderForm.get('isReadyForLifeFile')?.setValue(false);
    }
  }

  isStatusLifeFileProcessing(): boolean {
    return this.orderForm.get('status')?.value === OrderStatus.LifeFileProcessing;
  }

  getStatusDisplayText(statusValue: number): string {
    switch (statusValue) {
      case OrderStatus.New:
        return 'New';
      case OrderStatus.Completed:
        return 'Accepted';
      case OrderStatus.Cancel_noMoney:
        return 'No Money';
      case OrderStatus.Cancel_rejected:
        return 'Rejected';
      case OrderStatus.LifeFileProcessing:
        return 'LifeFile Processing';
      case OrderStatus.LifeFileSuccess:
        return 'LifeFile Success';
      case OrderStatus.LifeFileError:
        return 'LifeFile Error';
      default:
        return 'Unknown';
    }
  }

  getOrderStatusClass(status: number): string {
    switch (status) {
      case OrderStatus.New:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case OrderStatus.Cancel_noMoney:
        return 'bg-yellow-100 text-yellow-800 border border-yellow-300';
      case OrderStatus.Cancel_rejected:
        return 'bg-red-100 text-red-800 border border-red-300';
      case OrderStatus.Completed:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileProcessing:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileSuccess:
        return 'bg-green-100 text-green-800 border border-green-300';
      case OrderStatus.LifeFileError:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
    }
  }
  async onPaymentModeChange(newPaymentMode: string, event: Event): Promise<void> {
    event.preventDefault();
    const currentPaymentMode = this.orderForm.get('paymentMode')?.value;

    if (this.isPaymentModeLocked() || this.isReadOnlyMode() || currentPaymentMode === newPaymentMode) {
      return;
    }

    try {
      const confirmed = await firstValueFrom(
        this.confirmationDialogService.openConfirmation({
          title: 'Confirm Payment Mode Change',
          message: 'Are you sure the payment was paid? This action cannot be undone.',
          confirmButtonText: 'Yes',
          cancelButtonText: 'No'
        })
      );

      if (confirmed && this.orderId) {
        const paymentRequest: OrderPaymentRequestDto = {
          orderId: this.orderId,
          isOrderPaid: newPaymentMode === 'orderPaid' ? true : null,
          isCashPayment: newPaymentMode === 'cashPayment' ? true : null
        };

        this.isSubmitting.set(true);
        const response = await firstValueFrom(
          this.orderService.updateOrderPayment(paymentRequest)
        );

        if (response.isSuccess) {
          this.isPaymentModeLocked.set(true);
          this.orderForm.patchValue({
            paymentMode: newPaymentMode,
            isOrderPaid: response.isOrderPaid,
            isCashPayment: response.isCashPayment,
            orderPaidDate: response.orderPaidDate ? new Date(response.orderPaidDate) : null
          });

          this.notificationService.showSnackBar(
            `Payment status updated to ${newPaymentMode === 'orderPaid' ? 'Paid Online' : 'Paid in Cash'}`,
            'success'
          );
          this.reinitializeForm();
        } else {
          this.notificationService.showSnackBar(response.message || 'Failed to update payment status', 'failure');
          this.orderForm.get('paymentMode')?.setValue(currentPaymentMode);
        }
      } else {
        this.orderForm.get('paymentMode')?.setValue(currentPaymentMode);
      }
    } catch (error) {
      console.error('Error updating payment status:', error);
      this.notificationService.showSnackBar('Failed to update payment status', 'failure');
      this.orderForm.get('paymentMode')?.setValue(currentPaymentMode);
    } finally {
      this.isSubmitting.set(false);
    }
  }
  
  async onCancelCommission(): Promise<void> {
    try {
      const confirmed = await firstValueFrom(
        this.confirmationDialogService.openConfirmation({
          title: 'Confirm Commission Cancellation',
          message: 'Are you sure you want to cancel commission generation?',
          confirmButtonText: 'Yes',
          cancelButtonText: 'No'
        })
      );

      if (!confirmed) {
        return;
      }

      await this.onCancelGenerateCommission();
    } catch (error) {
      this.notificationService.showSnackBar('Confirmation dialog failed.', 'failure');
    }
  }

  async onCancelGenerateCommission(): Promise<void> {
    const orderId = this.orderForm.get('id')?.value;
    if (!orderId) {
      this.notificationService.showSnackBar('Order ID is missing.', 'failure');
      return;
    }

    const res = await firstValueFrom(
      this.orderService.cancelGenerateCommission(orderId)
    );

    if (res?.id) {
      this.notificationService.showSnackBar(res.message || 'Commission cancelled successfully.', 'success');
      this.reinitializeForm();
    } else {
      this.notificationService.showSnackBar(res?.message || 'Failed to cancel commission.', 'failure');
    }

  }

  getAbsoluteOutstandingBalance(): number {
    return Math.abs(this.outstandingRefundBalance());
  }

  // Method to determine display string for outstanding balance
  getOutstandingBalanceDisplayString(): string {
    return this.outstandingBalanceType() === 'negative' ? 'Payment Due from Customer' : 'Refund Due to Customer';
  }

  getSubText(): string {
    return this.outstandingBalanceType() === 'negative' ? 'This amount is pending collection from the customer.' : 'This amount is pending refund to the customer.';
  }

  // Method to determine display string for settled amount
  getSettledAmountDisplayString(): string {
    // If we have any settled amount and the original outstanding balance was negative
    // or if outstanding balance is now 0 but we settled from negative
    if (this.settledAmount() > 0) {
      // Check if the settled amount equals the absolute value of what would have been a negative balance
      // This indicates we settled from negative to zero
      if (this.outstandingRefundBalance() === 0 && this.settledAmount() > 0) {
        return 'Refund Issued';
      }
      return this.outstandingBalanceType() === 'negative' ? 'Payment Received' : 'Refund Issued';
    }
    return 'Payment Received';
  }

  shouldShowOutstandingBalanceWarning(): boolean {
    const currentStatus = this.orderForm.get('status')?.value;
    return currentStatus !== OrderStatus.Completed &&
      currentStatus !== OrderStatus.Cancel_noMoney &&
      currentStatus !== OrderStatus.Cancel_rejected;
  }

  getFormattedSettledAmount(): string {
    const amount = this.getSettledAmount();
    const absValue = Math.abs(amount).toFixed(2);
    return amount < 0 ? `-$${absValue}` : `$${absValue}`;
  }



}
