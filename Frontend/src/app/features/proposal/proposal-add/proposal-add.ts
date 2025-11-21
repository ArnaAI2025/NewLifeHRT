import { ChangeDetectorRef, Component, OnInit, inject, signal, computed, ViewChildren, ElementRef, QueryList, ApplicationRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl, FormControlName } from '@angular/forms';
import { PatientService } from '../../patient/patient.services';
import { UserManagementService } from '../../user-management/user-management.service';
import { PharmacyService } from '../../pharmacy/pharmacy.services';
import { ProposalService } from '../proposal.service';
import { finalize, firstValueFrom, forkJoin } from 'rxjs';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { CouponResponse } from '../model/coupon-response.model';
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
import { DateAdapter, MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { PriceListItemService } from '../../price-list-items/price-list-item.services';
import { PriceListItemsByPharmacyIdResponseDto } from '../../price-list-items/model/pricelistitem-by-pharmacyid-response.model';
import { PharmacyShippingMethodResponseDto } from '../../pharmacy/model/pharmacy-shipping-method.model';
import { PatientCreditCardResponseDto } from '../../patient/model/patient-credit-card-response';
import { ShippingAddressResponseDto } from '../../patient/model/shipping-address-response.model';
import { CardTypeEnum } from '../../../shared/enums/credit-card-type.enum';
import { Status } from '../../../shared/enums/status.enum';
import { ProductLineItemDto } from '../../price-list-items/model/product-transaction.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ProposalRequestDto } from '../model/proposal-request.model';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { ShippingAddressAddComponent } from '../../patient/shipping-address-add/shipping-address-add';
import { BulkOperationResponseDto } from '../../../shared/models/bulk-operation-response.model';
import { ProposalDetailRequestDto } from '../model/proposal-detail-request.model';
import { MatDialog } from '@angular/material/dialog';
import { ReasonDialogComponent } from '../../../shared/components/reason-dialog/reason-dialog';
import { PharmaciesDropdownResponseDto } from '../../pharmacy/model/pharmacies-dropdown-response.model';
import { PatientCounselorInfoDto } from '../../patient/model/patient-counselor-info';
import { WholeNumberDirective } from '../../../shared/directives/whole-number.directive';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  selector: 'app-proposal-add',
  templateUrl: './proposal-add.html',
  styleUrls: ['./proposal-add.scss'],
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
    WholeNumberDirective,
  ],
})
export class ProposalAdd implements OnInit {
  private patientService = inject(PatientService);
  private userManagementService = inject(UserManagementService);
  private pharmacyService = inject(PharmacyService);
  private proposalService = inject(ProposalService);
  private priceListItemService = inject(PriceListItemService);
  private notificationService = inject(NotificationService);
  private confirmationDialogService = inject(ConfirmationDialogService);
  private router = inject(Router);
  private appRef = inject(ApplicationRef);
  private confirmationService = inject(ConfirmationDialogService);

  private activatedRoute = inject(ActivatedRoute);
  private readonly dialog = inject(MatDialog);
  private readonly cdr = inject(ChangeDetectorRef);
  trackByAddressId = (_: number, a: ShippingAddressResponseDto) => a.id;
  trackByShippingMethodId = (_: number, m: PharmacyShippingMethodResponseDto) =>
    m.id;

  proposalForm!: FormGroup;
  productControl = new FormControl();

  proposalId: string | null = null;
  isEditMode: boolean = false;
  isDeleted: boolean = false;
  isReadOnlyMode = signal(false);
  isLab = signal(false);
  pharmacies: PharmaciesDropdownResponseDto[] = [];
  filteredPharmacies = signal<PharmaciesDropdownResponseDto[]>([]);
  patients: PatientCounselorInfoDto[] = [];
  filteredPatients: PatientCounselorInfoDto[] = [];
  counselors: DropDownResponseDto[] = [];
  filteredCounselors: DropDownResponseDto[] = [];
  coupons: CouponResponse[] = [];
  filteredCoupons: CouponResponse[] = [];
  phyisianInfo: DropDownResponseDto | null = null;
  Status = Status;
  statusList = Object.keys(Status)
    .filter((key) => !isNaN(Number(Status[key as keyof typeof Status])))
    .map((key) => ({
      name: key,
      value: Status[key as keyof typeof Status],
    }));

  selectedCoupon: CouponResponse | null = null;

  pharmacyShippingMethods: PharmacyShippingMethodResponseDto[] = [];
  filteredShippingMethods = signal<PharmacyShippingMethodResponseDto[]>([]);
  selectedShippingMethod: PharmacyShippingMethodResponseDto | null = null;
  toggleModifyShippingValue = false;
  customShippingValue = 0;

  patientCreditCards = signal<PatientCreditCardResponseDto[]>([]);
  selectedCreditCard: PatientCreditCardResponseDto | null = null;
  enableCreditCardPayment = false;
  creditCardSurchargeRate = 0.0399;

  shippingAddresses = signal<ShippingAddressResponseDto[]>([]);
  selectedShippingAddressId: string | null = null;
  isAddressVerified: boolean = false;
  addressVerificationMap: Map<string, boolean> = new Map();

  showShippingAddressModal = false;
  editingShippingAddressId?: string;
  currentPatientId: string | null = null;
  isUrlHasPatientId = false;
  pharmacyProducts: PriceListItemsByPharmacyIdResponseDto[] = [];
  filteredProducts: PriceListItemsByPharmacyIdResponseDto[] = [];
  selectedProducts: ProductLineItemDto[] = [];
  displayedColumns: string[] = [
    'productName',
    'protocol',
    'perUnitPrice',
    'quantity',
    'finalAmount',
    'actions',
  ];
  fromDashboard = false;

  isLoadingProducts = signal(false);
  isLoadingAddresses = signal(false);
  isLoadingCreditCards = signal(false);
  isLoadingPage = signal(true);
  isSubmitting = signal(false);
  isSavingProposalDetails = signal(false);
  isComingFromPatient = signal(false);
  constructor(private fb: FormBuilder) {}

  mathMax(a: number, b: number): number {
    return Math.max(a, b);
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }
  @ViewChildren(FormControlName, { read: ElementRef })
  formControls!: QueryList<ElementRef>;

  ngOnInit() {
    this.initForm();
    this.setupFormSubscriptions();
    this.activatedRoute.queryParams.subscribe((params) => {
      this.fromDashboard = params['fromDashboard'] || false;
    });

    this.activatedRoute.paramMap.subscribe((params) => {
      this.proposalId = params.get('proposalId');
      this.currentPatientId = params.get('patientId');
      this.isEditMode = this.proposalId !== null;
      this.loadMasterData();
      if (this.currentPatientId) {
        this.isUrlHasPatientId = true;
        this.proposalForm.get('patientId')?.disable({ emitEvent: false });
        this.isComingFromPatient.set(true);
      }
    });
  }

  initForm() {
    this.proposalForm = this.fb.group({
      name: [
        '',
        [Validators.required, Validators.minLength(3), NoWhitespaceValidator],
      ],
      patientId: [null, Validators.required],
      pharmacyId: [null, Validators.required],
      counselorId: [null, Validators.required],
      physicianId: [null],
      couponId: [null],
      shippingMethodId: [null],
      shippingAddressId: [null],
      status: [Status.Draft, Validators.required],
      therapyExpiration: [null],
      description: [''],
    });
    this.proposalForm.get('status')?.disable();
  }

  private setFormReadOnly(readonly: boolean): void {
    if (readonly) {
      Object.keys(this.proposalForm.controls).forEach((controlName) => {
        const control = this.proposalForm.get(controlName);
        if (control && control.enabled) {
          control.disable({ emitEvent: false });
        }
      });
    } else {
      Object.keys(this.proposalForm.controls).forEach((controlName) => {
        const control = this.proposalForm.get(controlName);
        if (control && control.disabled && controlName !== 'status') {
          control.enable({ emitEvent: false });
        }
      });
      this.proposalForm.get('status')?.disable();
    }
  }

  private async loadMasterData() {
    forkJoin({
      pharmacies: this.pharmacyService.getAllActivePharmacies(),
      patients: this.patientService.getAllPatientsCounselorInfo(),
      counselors: this.userManagementService.getAllActiveSalesPerson(),
      coupons: this.proposalService.getActiveCouponsForDropDown(),
    })
      .pipe(finalize(() => this.isLoadingPage.set(false)))
      .subscribe({
        next: ({ pharmacies, patients, counselors, coupons }) => {
          this.pharmacies = pharmacies;
          this.filteredPharmacies.set([...pharmacies]);
          this.patients = patients;
          this.filteredPatients = [...patients];
          this.counselors = counselors;
          this.filteredCounselors = [...counselors];
          this.coupons = coupons;
          this.filteredCoupons = [...coupons];

          if (this.proposalId) {
            this.loadProposalData();
          } else if (this.currentPatientId) {
            let matchingPatient = this.patients.find(
              (p) => String(p.id) === this.currentPatientId
            );
            this.handlePatientSelection(matchingPatient);
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

  private async loadProposalData() {
    try {
      const proposal = await firstValueFrom(
        this.proposalService.getProposalById(this.proposalId!)
      );
      if (proposal) {
        if ( proposal.status !== Status.Draft && proposal.status !== Status.Rejected ) {
          {
            this.loadAllCoupons();
          }
        }
        await this.populateFormFromProposal(proposal);
        this.isDeleted = proposal.isDeleted || false;

        const proposalStatus = proposal.status || Status.InReview;

        if (
          proposalStatus != Status.Draft &&
          proposalStatus != Status.Rejected
        ) {
          this.isReadOnlyMode.set(true);
          this.setFormReadOnly(true);
        } else {
          this.isReadOnlyMode.set(false);
          this.setFormReadOnly(false);
        }
      }
    } catch (error) {
      console.error('Failed to load proposal data:', error);
      this.notificationService.showSnackBar(
        'Failed to load proposal data',
        'failure'
      );
    }
  }
  async loadAllCoupons(): Promise<void> {
    const data = await firstValueFrom(this.proposalService.getAllCoupon());
    this.coupons = data;
    this.filteredCoupons = [...data];
    this.cdr.markForCheck();
  }

  private async populateFormFromProposal(proposal: any): Promise<void> {
    this.proposalForm.patchValue({
      name: proposal.name || '',
      status: proposal.status || Status.InReview,
      physicianId: proposal.physician || null,
      therapyExpiration: proposal.therapyExpiration
        ? new Date(proposal.therapyExpiration)
        : null,
      description: proposal.description || '',
    });

    if (proposal.patientId) {
      const patient = this.patients.find((p) => p.id === proposal.patientId);
      if (patient) {
        this.proposalForm.patchValue({ patientId: patient });
        this.currentPatientId = String(patient.id);
        await this.loadCreditCards(String(patient.id));
      }
    }

    if (proposal.pharmacyId) {
      const pharmacy = this.pharmacies.find(
        (p) => p.id === proposal.pharmacyId
      );
      if (pharmacy) {
        this.proposalForm.patchValue({ pharmacyId: pharmacy });
        await this.loadPharmacyProducts(String(pharmacy.id));
        if (!pharmacy.isLab) {
          await this.loadPharmacyShippingMethods(String(pharmacy.id));
        } else this.isLab.set(true);
      }
    }

    if (proposal.counselorId) {
      const counselor = this.counselors.find(
        (c) => c.id === proposal.counselorId
      );
      if (counselor) {
        this.proposalForm.patchValue({ counselorId: counselor });
      }
    }

    if (proposal.couponId) {
      const coupon = this.coupons.find((c) => c.id === proposal.couponId);
      if (coupon) {
        this.proposalForm.patchValue({ couponId: coupon });
        this.selectedCoupon = coupon;
      }
    }

    if (proposal.pharmacyShippingMethodId) {
      this.selectedShippingMethod =
        this.pharmacyShippingMethods.find(
          (method) => method.id === proposal.pharmacyShippingMethodId
        ) || null;

      if (
        this.selectedShippingMethod?.name !== 'Pick Up' &&
        this.currentPatientId
      ) {
        await this.loadAddresses(this.currentPatientId);
      }
    }

    this.selectedShippingAddressId = proposal.shippingAddressId || null;
    this.isAddressVerified =
      proposal.status == Status.Rejected
        ? false
        : proposal.isAddressVerified || false;

    if (this.selectedShippingAddressId) {
      this.addressVerificationMap.set(
        this.selectedShippingAddressId,
        this.isAddressVerified
      );
      this.proposalForm.patchValue({
        shippingAddressId: this.selectedShippingAddressId,
      });
    }

    this.selectedCreditCard = proposal.patientCreditCardId
      ? this.patientCreditCards().find(
          (card) => card.id === proposal.patientCreditCardId
        ) || null
      : null;
    this.enableCreditCardPayment = !!proposal.patientCreditCardId;

    this.customShippingValue = proposal.deliveryCharge || 0;
    if (this.selectedShippingMethod && proposal.deliveryCharge) {
      const originalShippingCost =
        parseFloat(this.selectedShippingMethod.value) || 0;
      this.toggleModifyShippingValue =
        proposal.deliveryCharge !== originalShippingCost;
    }

    this.selectedProducts = (proposal.proposalDetails || []).map(
      (detail: any) => ({
        id: detail.id,
        productPharmacyPriceListItemId: detail.productPharmacyPriceListItemId,
        productId: detail.productId,
        productName: detail.productName,
        protocol: detail.protocol || '',
        amount: detail.amount || 0,
        status: 1,
        isColdStorageProduct: detail.isColdStorageProduct || false,
        quantity: detail.quantity || 1,
        finalAmount: detail.amount || 0,
        perUnitPrice: detail.perUnitAmount || detail.amount || 0,
        togglePerUnitPrice: false,
      })
    );

    this.updateShippingMethodsForColdStorage();
    this.markFormPristineAndUntouched();
    this.cdr.detectChanges();
  }

  private reinitializeForm(): void {
    console.log('Starting form reinitialization...');

    this.resetAllState();

    this.initForm();

    this.setupFormSubscriptions();

    this.loadMasterData();

    console.log('Form reinitialization completed');
  }

  private resetAllState(): void {
    this.pharmacies = [];
    this.filteredPharmacies.set([]);
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
    this.patientCreditCards.set([]);
    this.selectedCreditCard = null;
    this.enableCreditCardPayment = false;
    this.shippingAddresses.set([]);
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
  }

  async onAcceptProposal(): Promise<void> {
    if (!this.proposalId) return;

    const confirmed = await this.confirmAction(
      'Accept Proposal',
      'Are you sure you want to accept this proposal?'
    );

    if (!confirmed) return;

    try {
      this.isSubmitting.set(true);

      const response = await firstValueFrom(
        this.proposalService.updateProposalStatus(
          this.proposalId,
          Status.Approved
        )
      );

      this.notificationService.showSnackBar(
        'Proposal accepted successfully',
        'success'
      );

      if (response) {
        if (!this.isUrlHasPatientId) {
          this.router.navigate(['/order/edit', response.id]);
        } else if (this.currentPatientId) {
          this.router.navigate([
            '/order/edit',
            response.id,
            this.currentPatientId,
          ]);
        }
      }
    } catch (error) {
      console.error('Failed to accept proposal:', error);
      this.notificationService.showSnackBar(
        'Failed to accept proposal',
        'failure'
      );
    } finally {
      this.isSubmitting.set(false);
    }
  }

  async onAcceptByPatientProposal(): Promise<void> {
    if (!this.proposalId) return;

    const confirmed = await this.confirmAction(
      'Accept Proposal',
      'Are you sure you want to accept this proposal?'
    );

    if (!confirmed) return;

    try {
      this.isSubmitting.set(true);

      const response = await firstValueFrom(
        this.proposalService.updateProposalStatus(
          this.proposalId,
          Status.ApprovedByPatient
        )
      );

      this.notificationService.showSnackBar(
        'Proposal accepted successfully',
        'success'
      );
      this.reinitializeForm();
    } catch (error) {
      console.error('Failed to accept proposal:', error);
      this.notificationService.showSnackBar(
        'Failed to accept proposal',
        'failure'
      );
    } finally {
      this.isSubmitting.set(false);
    }
  }

  onRejectByPatientProposal(): void {
    let dialogTitle = 'Enter Rejection Reason';

    const dialogRef = this.dialog.open(ReasonDialogComponent, {
      width: '400px',
      data: { title: dialogTitle },
    });

    dialogRef.afterClosed().subscribe((reason: string | null) => {
      if (reason) {
        console.log(`${dialogTitle}:`, reason);
        this.onReasonSubmittedByPatient(Status.RejectedByPatient, reason);
      } else {
        console.log('Dialog closed without providing a reason');
      }
    });
  }

  onReasonSubmittedByPatient(
    statusId: number,
    reason: string | null = null
  ): void {
    if (!this.proposalId) return;
    this.isSubmitting.set(true);
    this.proposalService
      .updateProposalStatus(this.proposalId, statusId, reason)
      .subscribe({
        next: (response) => {
          this.notificationService.showSnackBar(
            `Proposal rejected successfully`,
            'success'
          );
          this.reinitializeForm();
        },
        error: (err) => {
          this.notificationService.showSnackBar(
            `Failed to reject proposal.`,
            'failure'
          );
        },
        complete: () => {
          this.isSubmitting.set(false);
        },
      });
  }

  // Reject proposal method
  onRejectProposal(): void {
    let dialogTitle = 'Enter Rejection Reason';

    const dialogRef = this.dialog.open(ReasonDialogComponent, {
      width: '400px',
      data: { title: dialogTitle },
    });

    dialogRef.afterClosed().subscribe((reason: string | null) => {
      if (reason) {
        console.log(`${dialogTitle}:`, reason);
        this.onReasonSubmitted(Status.Rejected, reason);
      } else {
        console.log('Dialog closed without providing a reason');
      }
    });
  }

  onReasonSubmitted(statusId: number, reason: string | null = null): void {
    if (!this.proposalId) return;
    this.isSubmitting.set(true);
    this.proposalService
      .updateProposalStatus(this.proposalId, statusId, reason)
      .subscribe({
        next: (response) => {
          this.notificationService.showSnackBar(
            `Proposal ${Status[statusId].toLowerCase()} successfully`,
            'success'
          );
          this.reinitializeForm();
        },
        error: (err) => {
          const errorTerm = statusId === Status.Rejected ? 'reject' : 'cancel';
          console.error(`Failed to ${errorTerm} proposal with reason:`, err);
          this.notificationService.showSnackBar(
            `Failed to ${errorTerm} proposal.`,
            'failure'
          );
        },
        complete: () => {
          this.isSubmitting.set(false);
        },
      });
  }

  async onCancelProposal(): Promise<void> {
    if (!this.proposalId) return;

    const confirmed = await this.confirmAction(
      'Cancel Proposal',
      'Are you sure you want to cancel this proposal?'
    );

    if (!confirmed) return;

    this.onReasonSubmitted(Status.Canceled);
  }

  // Helper method for confirmation dialogs
  private async confirmAction(
    title: string,
    message: string
  ): Promise<boolean> {
    return await firstValueFrom(
      this.confirmationDialogService.openConfirmation({
        title,
        message,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
      })
    );
  }
  private handlePatientSelection(patientValue: any) {
    let patientId: string | null = null;

    if (typeof patientValue === 'object' && patientValue?.id) {
      patientId = String(patientValue.id);
      const matchingPatient = this.patients.find(
        (p) => String(p.id) === patientId
      );
      if (matchingPatient) {
        this.proposalForm.patchValue(
          { patientId: matchingPatient },
          { emitEvent: false }
        );
        if (matchingPatient.counselorId) {
          const matchingCounselor = this.counselors.find(
            (c) => c.id === matchingPatient.counselorId
          );
          if (matchingCounselor) {
            this.proposalForm.patchValue({ counselorId: matchingCounselor });
          }
        }
      }
    }

    if (patientId) {
      this.currentPatientId = patientId;
      this.loadCreditCards(patientId);
      this.loadAddresses(patientId);
      if (!this.proposalId) this.loadPhyisianInfo(patientId);
    } else {
      this.resetPatientData();
    }
  }

  setupFormSubscriptions() {
    this.proposalForm
      .get('pharmacyId')
      ?.valueChanges.subscribe(async (pharmacy) => {
        if (pharmacy?.id) {
          this.resetPharmacyData();
          this.toggleModifyShippingValue = false;
          this.customShippingValue = 0;
          this.loadPharmacyProducts(String(pharmacy.id));
          if (pharmacy.isLab) this.isLab.set(true);
          else {
            await this.loadPharmacyShippingMethods(String(pharmacy.id));
            this.isLab.set(false);
          }
        } else {
          this.resetPharmacyData();
        }
      });

    this.proposalForm
      .get('patientId')
      ?.valueChanges.subscribe((patientValue) => {
        this.handlePatientSelection(patientValue);
      });

    // this.proposalForm.get('patientId')?.valueChanges.subscribe(patient => {
    //   if (patient?.id) {
    //     this.currentPatientId = String(patient.id);
    //     this.loadCreditCards(String(patient.id));
    //   } else {
    //     this.resetPatientData();
    //   }
    // });

    this.proposalForm.get('couponId')?.valueChanges.subscribe((coupon) => {
      this.selectedCoupon = coupon;
    });

    this.proposalForm
      .get('shippingMethodId')
      ?.valueChanges.subscribe((method) => {
        if (!method || method.name === 'Pick Up') {
          this.resetShippingAddress();
        }
      });

    this.proposalForm
      .get('shippingAddressId')
      ?.valueChanges.subscribe((addressId) => {
        if (addressId && addressId !== this.selectedShippingAddressId) {
          this.selectedShippingAddressId = addressId;
        }
      });
  }

  // Reset Methods
  resetPharmacyData() {
    this.pharmacyProducts = [];
    this.filteredProducts = [];
    this.selectedProducts = [];
    this.pharmacyShippingMethods = [];
    this.filteredShippingMethods.set([]);
    this.selectedShippingMethod = null;
    this.resetShippingAddress();
    this.productControl.setValue('');
  }

  resetPatientData() {
    //this.currentPatientId = null;
    this.patientCreditCards.set([]);
    this.selectedCreditCard = null;
    this.enableCreditCardPayment = false;
    this.shippingAddresses.set([]);
    this.addressVerificationMap.clear();
    this.resetShippingAddress();
  }

  resetShippingAddress() {
    this.selectedShippingAddressId = null;
    this.isAddressVerified = false;
    this.proposalForm.get('shippingAddressId')?.setValue(null);
  }

  // Utility Methods
  preventDateInput(event: KeyboardEvent): void {
    event.preventDefault();
  }

  private async loadAddresses(patientId: string): Promise<void> {
    this.isLoadingAddresses.set(true);
    try {
      let addresses = await firstValueFrom(
        this.patientService.getAllAddressBasedOnPatientId(patientId)
      );
      addresses = addresses?.filter((addr) => addr.isActive) || [];
      this.shippingAddresses.set(addresses);

      if (
        !this.selectedShippingAddressId &&
        this.shippingAddresses().length > 0
      ) {
        const defaultAddress = this.shippingAddresses().find(
          (addr) => addr.isDefaultAddress === true
        );

        if (defaultAddress) {
          this.selectedShippingAddressId = defaultAddress.id;
          const addressControl = this.proposalForm.get('shippingAddressId');
          if (addressControl) {
            addressControl.setValue(defaultAddress.id);
            addressControl.markAsDirty();
            addressControl.markAsTouched();
            addressControl.updateValueAndValidity();
          }

          this.proposalForm.patchValue({
            shippingAddressId: defaultAddress.id,
          });

          this.isAddressVerified = false;
          this.addressVerificationMap.set(defaultAddress.id, false);

          setTimeout(() => {
            this.proposalForm.updateValueAndValidity();
          }, 100);
        }
      }
    } catch (err) {
      console.error('Failed to load addresses:', err);
      this.shippingAddresses.set([]);
      this.notificationService.showSnackBar(
        'Failed to load addresses',
        'failure'
      );
    } finally {
      this.isLoadingAddresses.set(false);
    }
  }
  private async loadPhyisianInfo(patientId: string) {
    const data = await firstValueFrom(
      this.patientService.getPhysianInfoBasedOnPatientId(patientId)
    );
    if (data) {
      this.proposalForm.patchValue({ physicianId: data });
      this.cdr.detectChanges();
    } else {
      if (!data) {
        this.confirmationService
          .openConfirmation({
            title: 'Doctor Not Assigned',
            message: 'Please assign a doctor before proceeding.',
            confirmButtonText: 'OK',
            showCancelButton: false,
          })
          .subscribe(() => {
            this.proposalForm.patchValue({ physicianId: null });
            this.cdr.detectChanges();
            return;
          });
      }
    }
  }
  private async loadPharmacyProducts(pharmacyId: string): Promise<void> {
    this.isLoadingProducts.set(true);

    try {
      const data = await firstValueFrom(
        this.priceListItemService.getAllActivePriceListItemsByPharmacyId(
          pharmacyId
        )
      );
      this.pharmacyProducts = data;
      this.filteredProducts = [...data];
    } catch (err) {
      console.error('Failed to load pharmacy products:', err);
      this.pharmacyProducts = [];
      this.filteredProducts = [];
      this.notificationService.showSnackBar(
        'Failed to load products',
        'failure'
      );
    } finally {
      this.isLoadingProducts.set(false);
    }
  }
  onProtocolChanged(proposalDetailId: string | number, event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    const product = this.selectedProducts.find(
      (p) => p.id === proposalDetailId
    );
    if (product) {
      product.protocol = value;
    }
  }

  private async loadCreditCards(patientId: string): Promise<void> {
    this.isLoadingCreditCards.set(true);

    try {
      const data = await firstValueFrom(
        this.patientService.getCreditCardByPatientId(patientId)
      );
      this.patientCreditCards.set(data.filter((card) => card.isActive));
      if (!this.selectedCreditCard && this.patientCreditCards().length > 0) {
        const defaultCard = this.patientCreditCards().find(
          (c) => c.isDefaultCreditCard
        );
        if (defaultCard) {
          this.selectedCreditCard = defaultCard;
          this.proposalForm.patchValue({ cardId: defaultCard.id });
        }
      }
    } catch (err) {
      console.error('Failed to load credit cards:', err);
      this.patientCreditCards.set([]);
      this.notificationService.showSnackBar(
        'Failed to load credit cards',
        'failure'
      );
    } finally {
      this.isLoadingCreditCards.set(false);
    }
  }

  private async loadPharmacyShippingMethods(pharmacyId: string): Promise<void> {
    try {
      const data = await firstValueFrom(
        this.pharmacyService.getAllPharmacyShippingMethods(pharmacyId)
      );
      if (data && data.length > 0) {
        this.pharmacyShippingMethods = data;
        this.updateShippingMethodsForColdStorage();
      } else {
        this.confirmationDialogService
          .openConfirmation({
            title: 'No Shipping Methods',
            message: 'No shipping methods found. Add at least one!',
            confirmButtonText: 'Ok',
            showCancelButton: false,
          })
          .subscribe(() => {});
      }
    } catch (err) {
      console.error('Failed to load shipping methods:', err);
      this.pharmacyShippingMethods = [];
      this.filteredShippingMethods.set([]);
      this.notificationService.showSnackBar(
        'Failed to load shipping methods',
        'failure'
      );
    }
  }

  // Validation Methods
  hasColdStorageProducts(): boolean {
    return this.selectedProducts.some((p) => p.isColdStorageProduct);
  }

  hasSelectedProducts(): boolean {
    return this.selectedProducts.length > 0;
  }

  hasSelectedShippingMethod(): boolean {
    return this.selectedShippingMethod !== null;
  }

  hasCreditCards = computed(() => this.patientCreditCards().length > 0);

  hasSelectedCoupon(): boolean {
    return this.selectedCoupon !== null;
  }

  shouldShowAddressSelection(): boolean {
    return (
      this.selectedShippingMethod !== null &&
      this.selectedShippingMethod.name !== 'Pick Up'
    );
  }

  hasShippingAddresses(): boolean {
    return this.shippingAddresses().length > 0;
  }

  isAddressVerificationRequired(): boolean {
    return (
      this.shouldShowAddressSelection() &&
      this.selectedShippingAddressId !== null
    );
  }

  canSubmitForm(): boolean {
    const basicValidation =
      this.proposalForm.valid && this.selectedProducts.length > 0;
    const addressValidation =
      !this.shouldShowAddressSelection() ||
      (!!this.selectedShippingAddressId && this.isAddressVerified);
    return basicValidation && addressValidation;
  }

  // Product Management
  updateShippingMethodsForColdStorage() {
    const hasColdStorageProducts = this.hasColdStorageProducts();
    let shippingMethods = [];
    if (hasColdStorageProducts) {
      shippingMethods = this.pharmacyShippingMethods.filter(
        (method) =>
          method.name === 'OverNight Shipping' || method.name === 'Pick Up'
      );
      this.notificationService.showSnackBar(
        'Shipping options limited due to cold storage products',
        'normal'
      );
    } else {
      shippingMethods = [...this.pharmacyShippingMethods];
    }

    this.filteredShippingMethods.set(shippingMethods);

    if (
      this.selectedShippingMethod &&
      !this.filteredShippingMethods().some(
        (m) => m.id === this.selectedShippingMethod?.id
      )
    ) {
      this.selectedShippingMethod = null;
      this.customShippingValue = 0;
      this.resetShippingAddress();
    }
  }

  onProductSelected(event: any) {
    const product = event.option.value;
    if (!product) return;

    if (
      this.selectedProducts.some(
        (p) => p.productPharmacyPriceListItemId === product.id
      )
    ) {
      this.notificationService.showSnackBar(
        'This product is already added to the proposal',
        'normal'
      );
      this.productControl.setValue('');
      return;
    }

    const selectedProduct: ProductLineItemDto = {
      id: undefined,
      productPharmacyPriceListItemId: product.id,
      productId: product.productId,
      productName: product.productName,
      protocol: product.protocol,
      amount: product.amount,
      status: product.status,
      isColdStorageProduct: product.isColdStorageProduct,
      quantity: 1,
      finalAmount: product.amount,
      perUnitPrice: product.amount,
      togglePerUnitPrice: false,
      productType: '',
    };

    this.selectedProducts = [...this.selectedProducts, selectedProduct];
    this.productControl.setValue('');
    this.updateShippingMethodsForColdStorage();
    this.notificationService.showSnackBar(
      `${product.productName} added to proposal`,
      'success'
    );
  }

  updateQuantity(priceListItemId: string, quantity: number) {
    const product = this.selectedProducts.find(
      (p) => p.productPharmacyPriceListItemId === priceListItemId
    );
    if (!product) return;
    const validQuantity = this.mathMax(1, quantity || 1);
    product.quantity = validQuantity;
    product.finalAmount = product.perUnitPrice * validQuantity;
  }

  updatePerUnitPrice(priceListItemId: string, perUnitPrice: number) {
    const product = this.selectedProducts.find(
      (p) => p.productPharmacyPriceListItemId === priceListItemId
    );
    if (!product || !product.togglePerUnitPrice) return;

    const validPrice = this.mathMax(0, perUnitPrice || 0);

    if (perUnitPrice < 0) {
      this.notificationService.showSnackBar(
        'Price cannot be negative',
        'normal'
      );
    }

    product.perUnitPrice = validPrice;
    product.finalAmount = validPrice * product.quantity;
  }

  togglePerUnitPriceModification(priceListItemId: string, isEnabled: boolean) {
    const product = this.selectedProducts.find(
      (p) => p.productPharmacyPriceListItemId === priceListItemId
    );
    if (!product) return;

    product.togglePerUnitPrice = isEnabled;

    if (!isEnabled) {
      product.finalAmount = product.perUnitPrice * product.quantity;
    }
  }

  removeProduct(productId: string): void {
    this.selectedProducts = this.selectedProducts.filter(
      (p) => p.productPharmacyPriceListItemId !== productId
    );
    this.updateShippingMethodsForColdStorage();
    this.notificationService.showSnackBar(
      'Product removed (changes pending save)',
      'normal'
    );
  }

  // Calculation Methods
  getProductSubTotal(): number {
    return this.selectedProducts.reduce(
      (total, product) => total + product.finalAmount,
      0
    );
  }

  getCreditCardSurcharge(): number {
    if (!this.enableCreditCardPayment || !this.selectedCreditCard) return 0;
    const subtotal =
      this.getProductSubTotal() + (this.customShippingValue || 0);
    return subtotal * this.creditCardSurchargeRate;
  }

  getGrandTotal(): number {
    const productTotal = this.getProductSubTotal();
    const shippingCost = this.customShippingValue || 0;
    const creditCardSurcharge = this.getCreditCardSurcharge();
    const couponDiscount = this.getCouponDiscount();

    return this.mathMax(
      0,
      productTotal + shippingCost + creditCardSurcharge - couponDiscount
    );
  }

  getCouponDiscount(): number {
    if (!this.selectedCoupon) return 0;

    const subtotal =
      this.getProductSubTotal() + (this.customShippingValue || 0);
    let discount = 0;

    const fixedDiscount = this.selectedCoupon.amount || 0;
    const percentageDiscount = this.selectedCoupon.percentage
      ? (subtotal * this.selectedCoupon.percentage) / 100
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
        return (
          savings + (product.amount - product.perUnitPrice) * product.quantity
        );
      }
      return savings;
    }, 0);
  }

  // Address Management
  onAddressVerificationChange(isVerified: boolean) {
    this.isAddressVerified = isVerified;

    if (this.selectedShippingAddressId) {
      this.addressVerificationMap.set(
        this.selectedShippingAddressId,
        isVerified
      );
    }

    const message = isVerified
      ? 'Address marked as verified'
      : 'Address marked as unverified';
    this.notificationService.showSnackBar(message, 'success');
  }

  onShippingAddressSelected(addressId: string) {
    const previousAddressId = this.selectedShippingAddressId;

    this.selectedShippingAddressId = addressId;
    this.proposalForm.get('shippingAddressId')?.setValue(addressId);

    const hasStoredVerification = this.addressVerificationMap.has(addressId);
    const storedVerificationStatus =
      this.addressVerificationMap.get(addressId) || false;

    if (previousAddressId && previousAddressId !== addressId) {
      if (hasStoredVerification) {
        this.isAddressVerified = storedVerificationStatus;
        const statusMessage = storedVerificationStatus
          ? 'Address selected - verification status restored (verified)'
          : 'Address selected - verification status restored (unverified)';
        this.notificationService.showSnackBar(statusMessage, 'success');
      } else {
        this.isAddressVerified = false;
        this.notificationService.showSnackBar(
          'Address changed - verification reset. Please verify the new address.',
          'normal'
        );
      }
    } else if (!previousAddressId) {
      if (hasStoredVerification) {
        this.isAddressVerified = storedVerificationStatus;
        const statusMessage = storedVerificationStatus
          ? 'Address selected - previously verified'
          : 'Address selected - please verify this address';
        this.notificationService.showSnackBar(statusMessage, 'success');
      } else {
        this.isAddressVerified = false;
        this.notificationService.showSnackBar(
          'Shipping address selected - please verify',
          'success'
        );
      }
    } else {
      this.notificationService.showSnackBar(
        'Shipping address confirmed',
        'success'
      );
    }
  }

  // Address Modal Management
  openAddAddressModal() {
    if (!this.currentPatientId) {
      this.notificationService.showSnackBar(
        'Please select a patient first',
        'normal'
      );
      return;
    }

    this.editingShippingAddressId = undefined;
    this.showShippingAddressModal = true;
  }

  openEditAddressModal(addressId: string) {
    if (!this.currentPatientId) {
      this.notificationService.showSnackBar(
        'Please select a patient first',
        'normal'
      );
      return;
    }

    this.editingShippingAddressId = addressId;
    this.showShippingAddressModal = true;
  }

  onShippingAddressCreated(newAddress: any) {
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;

    if (this.currentPatientId) {
      this.loadAddresses(this.currentPatientId);
    }

    if (newAddress?.id) {
      this.selectedShippingAddressId = newAddress.id;
      this.proposalForm.get('shippingAddressId')?.setValue(newAddress.id);
      this.isAddressVerified = false;
      this.addressVerificationMap.set(newAddress.id, false);
      this.notificationService.showSnackBar(
        'New address added - please verify the address',
        'success'
      );
    }
  }

  onShippingAddressUpdated(updatedAddress: any) {
    this.showShippingAddressModal = false;
    this.editingShippingAddressId = undefined;

    if (this.currentPatientId) {
      this.loadAddresses(this.currentPatientId);
    }

    if (updatedAddress?.id === this.selectedShippingAddressId) {
      this.isAddressVerified = false;
      this.addressVerificationMap.set(updatedAddress.id, false);
      this.notificationService.showSnackBar(
        'Address updated - please re-verify the address',
        'normal'
      );
    } else if (updatedAddress?.id) {
      this.addressVerificationMap.set(updatedAddress.id, false);
      this.notificationService.showSnackBar(
        'Address updated successfully',
        'success'
      );
    } else {
      this.notificationService.showSnackBar(
        'Address updated successfully',
        'success'
      );
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
      if (this.currentPatientId) {
        this.loadAddresses(this.currentPatientId);
      }
    }

    this.notificationService.showSnackBar(
      `Shipping method selected: ${shippingMethod.name}`,
      'success'
    );
  }

  toggleShippingValueModification(isEnabled: boolean) {
    this.toggleModifyShippingValue = isEnabled;

    if (!isEnabled && this.selectedShippingMethod) {
      this.customShippingValue =
        parseFloat(this.selectedShippingMethod.value) || 0;
    }

    const message = isEnabled
      ? 'Custom shipping cost enabled'
      : 'Custom shipping cost disabled';
    this.notificationService.showSnackBar(message, 'success');
  }

  updateCustomShippingValue(value: number) {
    const validValue = this.mathMax(0, value || 0);

    if (value < 0) {
      this.notificationService.showSnackBar(
        'Shipping cost cannot be negative',
        'normal'
      );
    }

    this.customShippingValue = validValue;
  }

  toggleCreditCardPayment(isEnabled: boolean) {
    this.enableCreditCardPayment = isEnabled;
    if (!isEnabled) {
      this.selectedCreditCard = null;
    }

    const message = isEnabled
      ? 'Credit card payment enabled'
      : 'Credit card payment disabled';
    this.notificationService.showSnackBar(message, 'success');
  }

  onCreditCardSelected(creditCard: PatientCreditCardResponseDto) {
    this.selectedCreditCard = creditCard;
    this.notificationService.showSnackBar('Credit card selected', 'success');
  }

  // Formatting Methods
  getCardTypeName(cardType: number): string {
    return CardTypeEnum[cardType] || 'Unknown';
  }

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
      address.postalCode,
    ].filter((part) => part && part.trim());

    let formattedAddress = parts.join(', ');

    if (address.isDefaultAddress) {
      formattedAddress += ' (Default)';
    }

    return formattedAddress;
  }

  // Filter Methods
  filterProducts(value: string) {
    const filterValue = value.toLowerCase();
    this.filteredProducts = this.pharmacyProducts.filter((product) =>
      product.productName?.toLowerCase().includes(filterValue)
    );
  }

  filterPharmacies(value: string) {
    const filterValue = value.toLowerCase();
    const filteredPharmacies = this.pharmacies.filter((p) =>
      p.name.toLowerCase().includes(filterValue)
    );
    this.filteredPharmacies.set(filteredPharmacies);
  }

  filterPatients(value: string) {
    const filterValue = value.toLowerCase();
    this.filteredPatients = this.patients.filter((p) =>
      p.name.toLowerCase().includes(filterValue)
    );
  }

  filterCounselors(value: string) {
    const filterValue = value.toLowerCase();
    this.filteredCounselors = this.counselors.filter((c) =>
      c.value.toLowerCase().includes(filterValue)
    );
  }

  filterCoupons(value: string) {
    const filterValue = value.toLowerCase();
    this.filteredCoupons = this.coupons.filter((c) =>
      (c.couponName || '').toLowerCase().includes(filterValue)
    );
  }

  // Display Functions
  displayProductFn = (product: any): string => {
    return product ? product.productName : '';
  };

  displayFn = (item: any): string => {
    if (!item) return '';
    if (item.value) return item.value;
    if (item.name) return item.name;
    if (item.couponName) return item.couponName;
    return '';
  };

  // Status Helper Methods
  getStatusDisplayText(statusValue: number): string {
    const status = this.statusList.find((s) => s.value === statusValue);
    return status ? status.name : 'Unknown';
  }

  getStatusClassByEnum(status: number): string {
    switch (status) {
      case Status.Draft:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
      case Status.InReview:
        return 'bg-blue-100 text-blue-800 border border-blue-300';
      case Status.Approved:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.Canceled:
        return 'bg-yellow-100 text-yellow-800 border border-yellow-300';
      case Status.Rejected:
        return 'bg-red-100 text-red-800 border border-red-300';
      case Status.ApprovedByPatient:
        return 'bg-green-100 text-green-800 border border-green-300';
      case Status.RejectedByPatient:
        return 'bg-red-100 text-red-800 border border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-300';
    }
  }

  // Validation Helper Methods
  isFieldInvalid(fieldName: string): boolean {
    const field = this.proposalForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  getFieldError(fieldName: string): string {
    const field = this.proposalForm.get(fieldName);
    let label = fieldName;
    if (label.endsWith('Id')) {
      label = label.slice(0, -2);
    }
    label = label.replace(/([A-Z])/g, ' $1').trim();
    label = label.charAt(0).toUpperCase() + label.slice(1);
    if (field?.errors) {
      if (field.errors['required']) return `${label} is required`;
      if (field.errors['email']) return 'Please enter a valid email';
      if (field.errors['minlength'])
        return `${label} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['pattern']) return `${label} format is invalid`;
      if (field.errors['whitespace'])
        return `${label} cannot be empty or only spaces`;
    }
    return '';
  }

  // Form State Management
  private markFormGroupTouched(): void {
    Object.values(this.proposalForm.controls).forEach((control) => {
      if (control instanceof FormGroup) {
        Object.values(control.controls).forEach((ctrl) => ctrl.markAsTouched());
      } else {
        control.markAsTouched();
      }
    });
  }

  private markFormPristineAndUntouched(
    formGroup: FormGroup = this.proposalForm
  ) {
    formGroup.markAsPristine();
    formGroup.markAsUntouched();

    Object.values(formGroup.controls).forEach((control) => {
      if (control instanceof FormGroup) {
        this.markFormPristineAndUntouched(control);
      } else {
        control.markAsPristine();
        control.markAsUntouched();
      }
    });
  }

  // API Payload Preparation
  private prepareApiPayload(): ProposalRequestDto {
    const formValue = this.proposalForm.getRawValue();

    const subtotal = this.getProductSubTotal();
    const deliveryCharge = this.customShippingValue || 0;
    const surcharge = this.getCreditCardSurcharge();
    const totalAmount = this.getGrandTotal();
    const couponDiscount = this.getCouponDiscount();

    const payload: ProposalRequestDto = {
      id: this.proposalId || undefined,
      name: formValue.name.trim(),
      patientId: formValue.patientId?.id || formValue.patientId,
      pharmacyId: formValue.pharmacyId?.id || formValue.pharmacyId,
      counselorId:
        parseInt(formValue.counselorId?.id || formValue.counselorId) || 0,
      phyisianId:
        parseInt(formValue.physicianId?.id || formValue.phyisianId) || 0,
      couponId: formValue.couponId?.id || formValue.couponId || undefined,
      patientCreditCardId: this.selectedCreditCard?.id || undefined,
      pharmacyShippingMethodId: this.selectedShippingMethod?.id || null,
      shippingAddressId: this.selectedShippingAddressId || undefined,
      isAddressVerified: this.isAddressVerified,
      therapyExpiration: formValue.therapyExpiration || undefined,

      subtotal: subtotal,
      totalAmount: totalAmount,
      surcharge: surcharge && surcharge > 0 ? surcharge : undefined,
      deliveryCharge: deliveryCharge > 0 ? deliveryCharge : undefined,
      couponDiscount: couponDiscount > 0 ? couponDiscount : undefined,
      status: Status.Draft,
      description: formValue.description || undefined,

      proposalDetails: this.getProductDetialsPayload(),
    };
    return payload;
  }

  getProductDetialsPayload(): ProposalDetailRequestDto[] {
    return this.selectedProducts.map(
      (product) =>
        ({
          id: product.id,
          productPharmacyPriceListItemId:
            product.productPharmacyPriceListItemId,
          productId: product.productId,
          quantity: product.quantity,
          amount: product.finalAmount,
          protocol: product.protocol,
          perUnitAmount: product.perUnitPrice,
          totalAmount: product.finalAmount,
        } as ProposalDetailRequestDto)
    );
  }

  updateProposalDetails(): Promise<boolean> {
    return new Promise((resolve) => {
      if (!this.proposalId) {
        return resolve(false);
      }
      this.isSavingProposalDetails.set(true);
      const detailsPayload = this.getProductDetialsPayload();
      this.proposalService
        .updateProposalDetails(this.proposalId, detailsPayload)
        .subscribe({
          next: (res: BulkOperationResponseDto) => {
            this.selectedProducts = this.selectedProducts.map((p) => ({
              ...p,
              togglePerUnitPrice: false,
            }));
            this.notificationService.showSnackBar(
              'Protocols updated successfully',
              'success'
            );
            resolve(true);
          },
          error: (err) => {
            console.error('Failed to update protocols:', err);
            this.notificationService.showSnackBar(
              'Failed to update protocols',
              'failure'
            );
            resolve(false);
          },
          complete: () => {
            this.isSavingProposalDetails.set(false);
          },
        });
    });
  }

  onSaveInformation(
    shouldClose: boolean = false,
    shouldSubmit: boolean = false
  ): Promise<number> {
    this.appRef.whenStable().then(() => {
      this.scrollToFirstInvalidControl();
    });
    return new Promise((resolve) => {
      if (this.isReadOnlyMode()) {
        this.notificationService.showSnackBar(
          'Form is in readonly mode',
          'normal'
        );
        return resolve(0);
      }

      if (this.proposalForm.invalid) {
        this.markFormGroupTouched();
        this.notificationService.showSnackBar(
          'Please fill in all required fields correctly.',
          'normal'
        );
        return resolve(0);
      }
      if (!this.proposalForm.value.physicianId) {
        this.notificationService.showSnackBar(
          'Assign physician to the patient',
          'normal'
        );
        return resolve(0);
      }
      if (shouldSubmit) {
        if (this.selectedProducts.length === 0) {
          this.notificationService.showSnackBar(
            'Please add at least one product to the proposal',
            'normal'
          );
          return resolve(0);
        }

        if (!this.isLab() && !this.hasSelectedShippingMethod()) {
          this.notificationService.showSnackBar(
            'Please select a shipping method',
            'normal'
          );
          return resolve(0);
        }
        if (
          this.shouldShowAddressSelection() &&
          !this.selectedShippingAddressId
        ) {
          this.notificationService.showSnackBar(
            'Please select a shipping address',
            'normal'
          );
          return resolve(0);
        }

        if (
          this.shouldShowAddressSelection() &&
          this.selectedShippingAddressId &&
          !this.isAddressVerified
        ) {
          this.notificationService.showSnackBar(
            'Please verify the shipping address before submitting',
            'normal'
          );
          return resolve(0);
        }
      }

      this.isSubmitting.set(true);
      const payload = this.prepareApiPayload();
      if (shouldSubmit) {
        payload.status = Status.InReview;
      }
      if (!this.proposalId) {
        this.proposalService.createProposal(payload).subscribe({
          next: (res: CommonOperationResponseDto) => {
            this.proposalId = res?.id ? String(res.id) : null;
            this.notificationService.showSnackBar(
              'Proposal successfully created!',
              'success'
            );

            if (shouldClose) {
              this.goBackToProposalView();
              return;
            }

            if (this.isUrlHasPatientId && this.currentPatientId) {
              this.router.navigate([
                '/patient',
                this.currentPatientId,
                'proposal',
                'edit',
                this.proposalId,
              ]);
            } else {
              this.router.navigate(['/proposal/edit', this.proposalId]);
            }

            this.isSubmitting.set(false);
            resolve(1);
          },
          error: () => {
            this.notificationService.showSnackBar(
              'Failed to create proposal. Please try again.',
              'failure'
            );
            this.isSubmitting.set(false);
            resolve(-1);
          },
        });
      } else {
        this.proposalService
          .updateProposal(this.proposalId, payload)
          .subscribe({
            next: () => {
              this.notificationService.showSnackBar(
                'Proposal updated successfully!',
                'success'
              );

              if (shouldClose) {
                this.goBackToProposalView();
                return;
              }

              setTimeout(() => {
                this.reinitializeForm();
              }, 1000);

              this.isSubmitting.set(false);
              resolve(1);
            },
            error: () => {
              this.notificationService.showSnackBar(
                'Failed to update proposal.',
                'failure'
              );
              this.isSubmitting.set(false);
              resolve(-1);
            },
          });
      }
    });
  }

  async onSubmit() {
    await this.onSaveInformation();
  }

  // // Navigation Methods
  // onOpenEditMode() {
  //   this.router.navigate(['/proposal/edit', this.proposalId]);
  // }

  onClickAdd(): void {
    const navigateToAdd = () => {
      this.resetForm();
      this.proposalId = null;
      this.isEditMode = false;

      if (this.isUrlHasPatientId && this.currentPatientId) {
        this.router.navigate(['/proposal/add', this.currentPatientId]);
        let matchingPatient = this.patients.find(
          (p) => String(p.id) === this.currentPatientId
        );
        this.handlePatientSelection(matchingPatient);
      } else {
        this.currentPatientId = null;
        this.router.navigate(['/proposal/add']);
      }
    };

    if (this.proposalForm.dirty) {
      this.confirmationDialogService
        .openConfirmation({
          title: 'Unsaved Changes',
          message:
            'You have unsaved changes. Are you sure you want to continue without saving?',
        })
        .subscribe((confirmed) => {
          if (confirmed) {
            navigateToAdd();
          }
        });
    } else {
      navigateToAdd();
    }
  }

  onClose(): void {
    if (this.isReadOnlyMode()) {
      this.goBackToProposalView();
    } else if (this.proposalForm.dirty) {
      this.confirmationDialogService
        .openConfirmation({
          title: 'Unsaved Changes',
          message:
            'You have unsaved changes. Are you sure you want to close without saving?',
        })
        .subscribe((confirmed) => {
          if (confirmed) {
            this.goBackToProposalView();
          }
        });
    } else {
      this.goBackToProposalView();
    }
  }

  goBackToProposalView() {
    if (this.isUrlHasPatientId && this.currentPatientId) {
      this.router.navigate(['/proposals/view', this.currentPatientId]);
    } else if (
      this.proposalForm.get('status')?.value === Status.InReview &&
      this.fromDashboard
    ) {
      this.router.navigate(['/dashboard']);
    } else {
      this.router.navigate(['/proposals/view']);
    }
  }

  resetForm(): void {
    this.proposalForm.reset();
    this.proposalForm.get('status')?.setValue(Status.Draft);
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
  }

  // Helper Methods

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
    return this.shippingAddresses().find(
      (a) => a.id === this.selectedShippingAddressId
    );
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

    this.confirmationDialogService
      .openConfirmation({
        title: 'Confirm Delete',
        message: `Are you sure you want to delete this proposal?`,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel',
        showCancelButton: true,
      })
      .subscribe((confirmed) => {
        if (confirmed) {
          if (!this.proposalId) {
            this.notificationService.showSnackBar(
              'Proposal ID is missing. Cannot delete.',
              'failure'
            );
            return;
          }
          this.proposalService.deleteProposal([this.proposalId]).subscribe({
            next: (res: BulkOperationResponseDto) => {
              if (res.successCount > 0) {
                this.notificationService.showSnackBar(
                  res.message ?? 'Proposal deleted successfully.',
                  'success'
                );
                this.onClose();
              } else {
                this.notificationService.showSnackBar(
                  res.message ?? 'Failed to delete proposal.',
                  'failure'
                );
              }
            },
            error: () => {
              this.notificationService.showSnackBar(
                'Server error while deleting proposal.',
                'failure'
              );
            },
          });
        }
      });
  }

  onSubmitProposalClick() {
    this.onSaveInformation(false, true);
  }

  private scrollToFirstInvalidControl() {
    for (const control of this.formControls.toArray()) {
      if (control.nativeElement.classList.contains('ng-invalid')) {
        control.nativeElement.scrollIntoView({
          behavior: 'smooth',
          block: 'center',
        });
        control.nativeElement.focus();
        break;
      }
    }
  }

  resetProductData() {
    this.filteredProducts = [...this.pharmacyProducts];
  }

  checkStatusInReviewOrAcceptedOrRejectedByPatient() {
    const status = this.proposalForm.get('status')?.value;
    return status === Status.InReview ||
      status === Status.ApprovedByPatient ||
      status === Status.RejectedByPatient;
  }

}
