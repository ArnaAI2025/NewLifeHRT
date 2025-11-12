import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  AfterViewInit,
  ChangeDetectionStrategy,
  inject,
  DestroyRef,
  ViewChildren,
  QueryList,
  signal,
  ElementRef,
  ApplicationRef,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  FormControlName,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormField, MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Gender } from '../../../shared/enums/gender.enum';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { PatientService } from '../patient.services';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NotificationService } from '../../../shared/services/notification.service';
import { PatientResponseDto } from '../model/patient-response.model';
import { finalize, firstValueFrom, forkJoin, single } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PatientCreditCardResponseDto } from '../model/patient-credit-card-response';
import { SavedCard } from '../model/saved-card.model';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatButtonToggleGroup } from '@angular/material/button-toggle';
import { ViewChild } from '@angular/core';
import { provideNgxMask } from 'ngx-mask';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { MonthEnum } from '../../../shared/enums/month.enum';
import { CardTypeEnum } from '../../../shared/enums/credit-card-type.enum';
import { CreditCardDto } from '../model/credit-card.model';
import { UserManagementService } from '../../user-management/user-management.service';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { UploadFileItemDto } from '../../../shared/models/upload-file.model';
import { PatientAttachmentResponseDto } from '../model/patient-attachment-response.model ';
import { AppSettingsService } from '../../../shared/services/app-settings.service';
import { DateValidators } from '../../../shared/validators/date-validators';
import { CustomEmailValidator } from "../../../shared/validators/custom-email.validator";
import { NoWhitespaceValidator } from "../../../shared/validators/no-whitespace.validator";

@Component({
  selector: 'app-patient-add',
  standalone: true,
  templateUrl: './patient-add.html',
  styleUrls: ['./patient-add.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    FormsModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    PatientNavigationBarComponent,
    FullPageLoaderComponent,
  ],
  providers: [provideNgxMask()],
})
export class PatientAddComponent implements OnInit, AfterViewInit {
  @ViewChildren(MatFormField) formFields!: QueryList<MatFormField>;
  private destroyRef = inject(DestroyRef);
  patientForm!: FormGroup;
  creditCardForm!: FormGroup;
  profileUrl: string | null = null;
  isSaving = signal(false);
  isLoading = signal(false);
  isLoadingPage = signal(true);
  savedCards: SavedCard[] = [];
  showCreditCardModal = false;
  isEditingCreditCard = false;
  editingCreditCardIndex: number | null = null;
  activeView: 'form' | 'counselor' = 'form';
  previousView: 'form' | 'counselor' = 'form';
  patientDataToLoad: PatientResponseDto | null = null;
  @ViewChild('toggleGroup') toggleGroup!: MatButtonToggleGroup;
  private initialSavedCardsFromAPI: SavedCard[] = [];
  private userAddedCardIds: Set<number | string> = new Set();
  private readonly userManagementService = inject(UserManagementService);
  private tempIdCounter = 1;
  profileImageFile: File | null = null;
  CardTypeEnum = CardTypeEnum;
  cardEnumValues: number[] = Object.values(CardTypeEnum).filter(
    (v) => typeof v === 'number'
  ) as number[];
  MonthEnum = MonthEnum;
  monthEnumValues: number[] = Object.values(MonthEnum).filter(
    (v) => typeof v === 'number'
  ) as number[];
  years: { value: string; label: string }[] = [];
  openUploadDialog = signal(false);
  isOpenSelectCategoryDialog = signal(false);
  selectedFileUrl: string | null = null;
  selectedFileType: string | null = null;
  isFileModalOpen = false;
  isFileLoading = false;
  selectedFiles: PatientAttachmentResponseDto[] = [];
  isAllSelected = false;
  selectedCategory: DropDownResponseDto | null = null;
  files: UploadFileItemDto[] = [];
  uploadedFiles = signal<PatientAttachmentResponseDto[]>([]);
  // Dropdown lists
  doctorsList: DropDownResponseDto[] = [];
  filteredDoctorsList: DropDownResponseDto[] = [];
  isLoadingDoctor: boolean = false;

  visitTypeList: DropDownResponseDto[] = [];
  isLoadingVisitType: boolean = false;
  documentCategories: DropDownResponseDto[] = [];
  agendaList: DropDownResponseDto[] = [];
  filteredagendaList: DropDownResponseDto[] = [];
  isLoadingAgenda: boolean = false;
  isSaveAndClose = false;
  patientList: DropDownResponseDto[] = [];
  filteredPatientList: DropDownResponseDto[] = [];
  isLoadingPatient: boolean = false;

  countriesList: DropDownResponseDto[] = [];
  filteredcountriesList: DropDownResponseDto[] = [];
  isLoadingCountry: boolean = false;

  salesPersonList: DropDownResponseDto[] = [];
  filteredSalesPersonList: DropDownResponseDto[] = [];
  isLoadingSales: boolean = false;

  allStates: DropDownResponseDto[] = [];
  isLoadingStates = signal(false);

  @Input() patientId: string | null = null;
  @Output() closeForm = new EventEmitter<boolean>();
  maxDate: Date = DateValidators.getToday();

  isEditMode = false;
  genderEnum = Gender;

  genderOptions = Object.entries(Gender)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ label: key, value }));

  @ViewChildren(FormControlName, { read: ElementRef })
  formControls!: QueryList<ElementRef>;
  isPatientLoggedIn: boolean = false;
  isIndeterminate = false;

  constructor(
    private formbuilder: FormBuilder,
    private patientService: PatientService,
    private cdRef: ChangeDetectorRef,
    private notificationService: NotificationService,
    private router: Router,
    private confirmationService: ConfirmationDialogService,
    private activatedRoute: ActivatedRoute,
    private appRef: ApplicationRef,
    private appSettingsService: AppSettingsService
  ) {
    this.generateYears();
  }

  ngOnInit() {
    this.patientId = this.activatedRoute.snapshot.paramMap.get('id');
    this.checkPatientLoggedInStatus();
    this.initializeForm();
    this.initializeCreditCardForm();

    forkJoin({
      doctors: this.userManagementService.getAllActiveDoctors(),
      patients: this.patientService.getAllActivePatients(),
      salesPersons: this.userManagementService.getAllActiveSalesPerson(),
      agendas: this.patientService.getAllActiveAgenda(),
      visitTypes: this.patientService.getAllActiveVisitType(),
      documentCategories: this.patientService.getAllActiveDocumentCategory(),
      countries: this.patientService.getAllActiveCountries(),
    })
      .pipe(
        finalize(() => {
          this.cdRef.detectChanges();

          this.formFields.forEach((field) => {
            if (typeof (field as any).updateOutlineGap === 'function') {
              (field as any).updateOutlineGap();
            }
          });
        })
      )
      .subscribe({
        next: ({
          doctors,
          patients,
          salesPersons,
          agendas,
          visitTypes,
          documentCategories,
          countries,
        }) => {
          this.doctorsList = doctors;
          this.filteredDoctorsList = [...doctors];

          this.patientList = patients;
          this.filteredPatientList = [...patients];

          this.salesPersonList = salesPersons;
          this.filteredSalesPersonList = [...salesPersons];

          this.agendaList = agendas;
          this.filteredagendaList = [...agendas];

          this.countriesList = countries;
          this.filteredcountriesList = [...countries];

          this.visitTypeList = visitTypes;
          this.documentCategories = documentCategories.filter(
            (category: any) => category.value.toLowerCase() !== 'personal photo'
          );


          if (this.patientId) {
            this.isEditMode = true;
            this.getPatientById();
            this.loadUploadedDocuments();
          }
          this.isLoadingPage = signal(false);
        },
        error: (err) => {
          console.error('Failed to load reference data:', err);
          this.notificationService.showSnackBar(
            'Failed to load some data.',
            'failure'
          );
          this.isLoadingPage = signal(false);
        },
      });
  }

  loadUploadedDocuments(): void {
    this.isLoadingPage.set(true);
    this.patientService.loadUploadedDocuments(this.patientId!).subscribe({
      next: (response: PatientAttachmentResponseDto[]) => {
        if (response && response.length > 0) {
          this.uploadedFiles.set(response);
        } else {
          this.files = [];
        }
        this.cdRef.detectChanges();
        this.isLoadingPage.set(false);
      },
      error: (error) => {
        this.isLoadingPage.set(false);
        console.error('Error loading uploaded documents:', error);
        this.notificationService.showSnackBar(
          'Failed to load uploaded documents.',
          'failure'
        );
      },
    });
  }
  private generateYears(): void {
    const currentYear = new Date().getFullYear();
    for (let i = 0; i < 20; i++) {
      const year = (currentYear + i).toString();
      this.years.push({ value: year, label: year });
    }
  }

  private initializeCreditCardForm(): void {
    this.creditCardForm = this.formbuilder.group({
      creditCardNumber: [''],
      cardType: [''],
      expiryMonth: [''],
      expiryYear: [''],
    });

    this.creditCardForm.valueChanges.subscribe(() => {
      this.applyCreditCardValidation();
    });
  }

  private applyCreditCardValidation(): void {
    const formValue = this.creditCardForm.value;
    const hasAnyValue = Object.values(formValue).some(
      (value) => value && value.toString().trim() !== ''
    );

    if (hasAnyValue) {
      // If any field has a value, all fields become required
      this.creditCardForm
        .get('creditCardNumber')
        ?.setValidators([Validators.required, Validators.pattern(/^\d{16}$/)]);
      this.creditCardForm.get('cardType')?.setValidators([Validators.required]);
      this.creditCardForm
        .get('expiryMonth')
        ?.setValidators([Validators.required]);
      this.creditCardForm
        .get('expiryYear')
        ?.setValidators([Validators.required]);
    } else {
      // If no fields have values, remove all validators
      this.creditCardForm.get('creditCardNumber')?.clearValidators();
      this.creditCardForm.get('cardType')?.clearValidators();
      this.creditCardForm.get('expiryMonth')?.clearValidators();
      this.creditCardForm.get('expiryYear')?.clearValidators();
    }

    Object.keys(this.creditCardForm.controls).forEach((key) => {
      this.creditCardForm
        .get(key)
        ?.updateValueAndValidity({ emitEvent: false });
    });
  }

  getPatientById(): void {
    if (!this.patientId) {
      this.notificationService.showSnackBar('Invalid patient ID.', 'failure');
      return;
    }

    this.isLoading.set(true);

    this.patientService.getPatientById(this.patientId).subscribe({
      next: (response: PatientResponseDto) => {
        this.patientDataToLoad = response;
        this.patchFormWithPatientData();
        this.cdRef.detectChanges();
        this.markAsPristineAndUntouched();
      },
      error: (error) => {
        console.error('Error fetching patient:', error);
        const message =
          error?.error?.message ||
          error?.error ||
          'Error fetching patient details.';
        this.notificationService.showSnackBar(message, 'failure');
      },
      complete: () => {
        this.isLoading.set(false);
        this.cdRef.detectChanges();
      },
    });
  }

  private patchFormWithPatientData(): void {
    if (!this.patientDataToLoad) return;
    const basicFormData = this.mapPatientDtoToForm(this.patientDataToLoad);
    this.patientForm.patchValue(basicFormData);

    this.loadPatientCreditCards(
      this.patientDataToLoad.patientCreditCards || []
    );

    this.patchAutocompleteFields();
  }

  private loadPatientCreditCards(
    creditCards: PatientCreditCardResponseDto[]
  ): void {
    if (!creditCards || creditCards.length === 0) {
      this.savedCards = [];
      this.initialSavedCardsFromAPI = [];
      return;
    }

    this.savedCards = creditCards.map((card, index) => ({
      id: card.id || Date.now() + index,

      last4:
        card.cardNumber && card.cardNumber.length >= 4
          ? card.cardNumber.slice(-4)
          : '****',

      expiryMonth: card.month,
      expiryYear: card.year,
      cardType: card.cardType,
      cardToken: card.id,
      creditCardNumber: card.cardNumber || '',
      cardNumber: card.cardNumber || '',

      maskedNumber:
        card.cardNumber && card.cardNumber.length >= 4
          ? `****-****-****-${card.cardNumber.slice(-4)}`
          : `****-****-****-****`,
    }));

    this.initialSavedCardsFromAPI = [...this.savedCards];
    this.userAddedCardIds.clear(); // Reset user-added tracking

    this.cdRef.detectChanges();
  }

  private patchAutocompleteFields(): void {
    if (!this.patientDataToLoad) return;

    if (
      this.patientDataToLoad.assignPhysicianId &&
      this.doctorsList.length > 0
    ) {
      const physician = this.doctorsList.find(
        (doc) => doc.id === this.patientDataToLoad!.assignPhysicianId
      );
      if (physician) {
        this.patientForm.patchValue({ assignPhysicianId: physician });
      }
    }
    if (this.patientDataToLoad.referralId && this.patientList.length > 0) {
      const referral = this.patientList.find(
        (ref) => ref.id === this.patientDataToLoad!.referralId
      );
      if (referral) {
        this.patientForm.patchValue({ referralId: referral.id });
      }
    }
    if (this.patientDataToLoad.counselorId && this.salesPersonList.length > 0) {
      const counselor = this.salesPersonList.find(
        (sp) => sp.id === this.patientDataToLoad!.counselorId
      );
      if (counselor) {
        this.patientForm.patchValue({ counselorId: counselor });
      }
    }
    if (
      this.patientDataToLoad.agendaIds &&
      this.patientDataToLoad.agendaIds.length > 0 &&
      this.agendaList.length > 0
    ) {
      this.patientForm.patchValue({
        agendaId: this.patientDataToLoad.agendaIds,
      });
    }

    this.cdRef.detectChanges();
  }

  // private getGenderEnumValue(genderString: string): number | null {
  //   const genderKey = Object.keys(this.genderEnum).find(key =>
  //     key.toLowerCase() === genderString.toLowerCase()
  //   );
  //   return genderKey ? this.genderEnum[genderKey as keyof typeof this.genderEnum] : null;
  // }

  private mapPatientDtoToForm(patient: PatientResponseDto): any {
    this.profileUrl = patient.profileImageUrl || null;
    return {
      patientNumber: patient.patientNumber,
      firstName: patient.firstName,
      lastName: patient.lastName,
      previousCounselorFullName: patient.previousCounselorFullName,
      gender: this.genderEnum[patient.gender],
      email: patient.email,
      dateOfBirth: patient.dateOfBirth ? new Date(patient.dateOfBirth) : null,
      drivingLicence: patient.drivingLicence,
      phoneNumber: patient.phoneNumber,
      addressLine1: patient.address?.addressLine1,
      city: patient.address?.city,
      postalCode: patient.address?.postalCode,
      countryId: patient.address?.countryId || 1,
      stateId: patient.address?.stateId || null,
      visitTypeId: patient.visitTypeId,
      referralId: patient.referralId,
      allergies: patient.allergies,
      patientGoal: patient.patientGoal,
      splitCommission: patient.splitCommission || false,
      status: patient.status,
      isAllowMail: patient.isAllowMail,

      agendaId: patient.agendaIds || [],
      profileUrl: patient.profileImageUrl || null,
      labRenewableAlertDate: patient.labRenewableAlertDate
        ? new Date(patient.labRenewableAlertDate)
        : null,
      outstandingRefundBalance: patient.outstandingRefundBalance || 0,
    };
  }

  ngAfterViewInit(): void {
    requestAnimationFrame(() => {
      this.cdRef.detectChanges();
      setTimeout(() => {
        this.cdRef.detectChanges();
        this.updateFormFields();
        setTimeout(() => {
          this.cdRef.markForCheck();
        }, 50);
      }, 0);
    });
  }

  private updateFormFields(): void {
    setTimeout(() => {
      const formFields = document.querySelectorAll('mat-form-field');
      formFields.forEach((field: any) => {
        if (field._elementRef?.nativeElement) {
          field._elementRef.nativeElement.style.display = 'none';
          field._elementRef.nativeElement.offsetHeight;
          field._elementRef.nativeElement.style.display = '';
        }
      });
      this.cdRef.detectChanges();
    }, 100);
  }

  private initializeForm() {
    this.patientForm = this.formbuilder.group({
      patientNumber: [{ value: null, disabled: true }],
      firstName: [null, [Validators.required,NoWhitespaceValidator]],
      lastName: [null, [Validators.required,NoWhitespaceValidator]],
      gender: [null, Validators.required],
      email: [null, [Validators.required, Validators.email,CustomEmailValidator]],
      allergies: [null, [Validators.required,NoWhitespaceValidator]],
      profileUrl: [null],
      dateOfBirth: [null],
      drivingLicence: [null],
      phoneNumber: [
        null,
        [
          Validators.pattern(
            /^(\+?1\s?)?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$/
          ),
        ],
      ],
      addressLine1: [null],
      city: [null],
      postalCode: [
        null,
        [Validators.required, Validators.pattern(/^\d{5,6}$/)],
      ],
      countryId: [, [Validators.required]],
      stateId: [null, Validators.required],
      visitTypeId: [null],
      assignPhysicianId: [null],
      counselorId: [null, Validators.required],
      previousCounselorFullName: [null],
      referralId: [null],
      patientGoal: [null],
      splitCommission: [false],
      status: [true],
      isAllowMail: [true],
      labRenewableAlertDate: [null],

      agendaId: [[], []],
      outstandingRefundBalance: [0, []],
      // labRenewalDate: [null]
    });

    this.patientForm.get('counselorId')?.valueChanges.subscribe((value) => {
      if (typeof value === 'string') {
        this.filteredSalesPersonList = this.salesPersonList.filter((option) =>
          option.value.toLowerCase().includes(value.toLowerCase())
        );
      }
    });

    this.patientForm.get('assignPhysicianId')?.valueChanges.subscribe((value) => {
        if (typeof value === 'string') {
          this.filteredDoctorsList = this.doctorsList.filter((option) =>
            option.value.toLowerCase().includes(value.toLowerCase())
          );
        }
      });
    this.patientForm.get('countryId')?.valueChanges.subscribe((countryId) => {
      if (countryId && typeof countryId === 'number') {
        this.loadStatesByCountryId(countryId);
        // Clear existing state selection when country changes
        this.patientForm.get('stateId')?.setValue(null);
      } else {
        this.allStates = [];
        this.patientForm.get('stateId')?.setValue(null);
      }
    });

    this.patientForm.get('previousCounselorFullName')?.disable();

    if (this.isPatientLoggedIn) {
      this.patientForm.disable({ emitEvent: false });
    }

    setTimeout(() => {
      this.cdRef.detectChanges();
    }, 0);
  }

  displayCounselor = (user?: DropDownResponseDto): string =>
    user ? user.value : '';
  displayPhysician = (doctor?: DropDownResponseDto): string =>
    doctor ? doctor.value : '';

  onPhysicianSelected(selectedPhysician: DropDownResponseDto) {
    this.patientForm.patchValue({
      assignPhysicianId: selectedPhysician,
    });
  }

  onCounselorSelected(selectedCounselor: DropDownResponseDto) {
    this.patientForm.patchValue({
      counselorId: selectedCounselor,
    });
  }

  private shouldIncludeAddress(formValue: any): boolean {
    return !!(formValue.stateId || formValue.postalCode);
  }

  private extractIdFromDropdown(value: any): number | null {
    if (!value) return null;

    if (typeof value === 'number') return value;

    if (typeof value === 'string' && !isNaN(Number(value))) {
      return parseInt(value);
    }

    if (typeof value === 'object' && value.id) {
      return parseInt(value.id.toString());
    }

    return null;
  }

  private mapFormToFormData(): FormData {
    const formValue = this.patientForm.value;
    const formData = new FormData();

    formData.append(
      'outstandingRefundBalance',
      (formValue.outstandingRefundBalance || 0).toString()
    );
    formData.append('firstName', formValue.firstName.trim() || '');
    formData.append('lastName', formValue.lastName.trim() || '');
    formData.append(
      'gender',
      formValue.gender ? parseInt(formValue.gender.toString()).toString() : '0'
    );
    formData.append('email', formValue.email || '');
    formData.append('allergies', formValue.allergies.trim() || '');
    formData.append('status', (formValue.status === true).toString());

    formData.append(
      'dateOfBirth',
      formValue.dateOfBirth ? this.formatDate(formValue.dateOfBirth) : ''
    );

    formData.append('drivingLicence', formValue.drivingLicence || '');
    formData.append('phoneNumber', formValue.phoneNumber || '');
    formData.append(
      'visitTypeId',
      formValue.visitTypeId
        ? parseInt(formValue.visitTypeId.toString()).toString()
        : ''
    );

    formData.append(
      'assignPhysicianId',
      this.extractIdFromDropdown(formValue.assignPhysicianId)?.toString() || ''
    );
    formData.append(
      'counselorId',
      this.extractIdFromDropdown(formValue.counselorId)?.toString() || ''
    );

    if (formValue.agendaId && formValue.agendaId.length > 0) {
      formValue.agendaId.forEach((id: any, index: number) => {
        formData.append(
          `agendaId[${index}]`,
          parseInt(id.toString()).toString()
        );
      });
    }
    if (formValue.profileUrl) {
      formData.append(
        'profileImageFile',
        this.profileImageFile ? this.profileImageFile : ''
      );
    }
    formData.append(
      'referralId',
      this.extractStringIdFromDropdown(formValue.referralId) || ''
    );
    formData.append('patientGoal', formValue.patientGoal || '');
    formData.append('splitCommission', formValue.splitCommission || '');
    formData.append(
      'isAllowMail',
      formValue.isAllowMail !== null ? formValue.isAllowMail.toString() : ''
    );
    formData.append(
      'labRenewableAlertDate',
      formValue.labRenewableAlertDate
        ? this.formatDate(formValue.labRenewableAlertDate)
        : ''
    );

    // address object properties
    if (this.shouldIncludeAddress(formValue)) {
      formData.append('address.addressLine1', formValue.addressLine1 || '');
      formData.append('address.city', formValue.city || '');
      formData.append('address.stateId', formValue.stateId || null);
      formData.append('address.postalCode', formValue.postalCode || '');
      formData.append('address.countryId', formValue.countryId || '');
    }

    // Saved credit cards
    const cards = this.mapSavedCardsToDto();
    if (cards) {
      cards.forEach((card, index) => {
        if (card.id)
          formData.append(`patientCreditCards[${index}].id`, card.id);
        formData.append(`patientCreditCards[${index}].last4`, card.last4);
        formData.append(
          `patientCreditCards[${index}].cardType`,
          String(card.cardType)
        );
        formData.append(
          `patientCreditCards[${index}].month`,
          card.month.toString()
        );
        formData.append(
          `patientCreditCards[${index}].year`,
          card.year.toString()
        );
        formData.append(
          `patientCreditCards[${index}].cardNumber`,
          card.cardNumber
        );
        formData.append(
          `patientCreditCards[${index}].maskedNumber`,
          card.maskedNumber
        );
      });
    }

    // Append file if exists
    // if (this.profileImageFile) {
    //   formData.append('profileImageFile', this.profileImageFile);
    // }

    return formData;
  }

  private extractStringIdFromDropdown(value: any): string | null {
    if (!value) return null;

    if (typeof value === 'string') {
      return value.trim() || null;
    }

    if (typeof value === 'object' && value.id) {
      return value.id.toString() || null;
    }

    return null;
  }

  private mapSavedCardsToDto(): CreditCardDto[] | null {
    if (!this.savedCards || this.savedCards.length === 0) {
      return null;
    }

    return this.savedCards.map((card) => ({
      id:
        card.cardToken && !card.cardToken.startsWith('card_')
          ? card.cardToken
          : undefined,

      last4: card.last4,
      cardType: card.cardType,

      month: card.expiryMonth,
      year: card.expiryYear,

      cardNumber: card.cardNumber || card.creditCardNumber || '',

      maskedNumber: `****-****-****-${card.last4}`,
    }));
  }
  constructUploadFilesFormData(id: string | number): Promise<void> {
    return new Promise((resolve, reject) => {
      const uploadFormData = new FormData();
      this.isLoadingPage.set(true);

      uploadFormData.append('id', id.toString());
      if (this.uploadedFiles() && this.uploadedFiles().length > 0) {
        this.uploadedFiles()
          .filter((f) => f.id == null)
          .forEach((f, index) => {
            uploadFormData.append(`UploadFileItemDto[${index}].file`, f.file);
            uploadFormData.append(
              `UploadFileItemDto[${index}].DocumentCategoryId`,
              f.fileCategoryId.toString()
            );
          });
          this.patientService.uploadFiles(uploadFormData).subscribe({
            next: (uploadResponse: any) => {
              this.isLoadingPage.set(false);
              if (uploadResponse) {
                if (this.isEditMode) this.loadUploadedDocuments();
                this.files = [];
                this.notificationService.showSnackBar(
                  'Document uploaded successfully',
                  'success'
                );
                resolve();
              } else {
                this.notificationService.showSnackBar(
                  'Failed to upload document',
                  'failure'
                );
                reject();
              }
            },
            error: () => {
              this.isLoadingPage.set(false);
              this.notificationService.showSnackBar(
                'Error uploading files.',
                'failure'
              );
              reject();
            },
          });
      }
      else{
        resolve();
      }
    });

  }

  async onSubmit(): Promise<void> {
    if (this.patientForm.invalid) {
      this.markFormGroupTouched(this.patientForm);
      this.appRef.whenStable().then(() => {
        this.scrollToFirstInvalidControl();
      });
      this.notificationService.showSnackBar(
        'Please fill in all required fields correctly.',
        'normal'
      );
      return;
    }

    const formData = this.mapFormToFormData();
    this.isSaving.set(true);
    this.isLoadingPage.set(true);

    const apiCall = this.isEditMode
      ? this.patientService.updatePatient(this.patientId!, formData)
      : this.patientService.createPatient(formData);

    apiCall.subscribe({
      next: async (response: CommonOperationResponseDto) => {
        if (response.id) {
          this.isLoadingPage.set(false);

          if (!this.isEditMode) {
            await this.constructUploadFilesFormData(response.id);
          }

          this.notificationService.showSnackBar(response.message, 'success');
          this.isSaving.set(false);
          this.markAsPristineAndUntouched();
          this.cdRef.detectChanges();

          // Navigate only after upload completes
          if (!this.isSaveAndClose) {
            this.router.navigate(['/patient/edit', response.id]);
          } else {
            this.router.navigate(['/patients/view']);
          }
        }
        else{
          this.isLoadingPage.set(false);
          this.notificationService.showSnackBar(response.message, 'failure');
          this.isSaving.set(false);
        }
      },
      error: (error) => {
        const errorMsg =
          error?.error?.message || error?.error || 'Error saving patient data.';
        this.notificationService.showSnackBar(errorMsg, 'failure');
        this.isLoadingPage.set(false);
        this.isSaving.set(false);
        this.cdRef.detectChanges();
      },
    });
  }

  markAsPristineAndUntouched(): void {
    this.patientForm.markAsPristine();
    this.patientForm.markAsUntouched();
    this.creditCardForm.markAsPristine();
    this.creditCardForm.markAsUntouched();
  }
  allowOnlyNumbers(event: KeyboardEvent) {
    if (!/[0-9]/.test(event.key)) {
      event.preventDefault();
    }
  }

  validatePaste(event: ClipboardEvent) {
    const pasted = event.clipboardData?.getData('text') || '';
    if (!/^\d+$/.test(pasted)) {
      event.preventDefault();
    }
  }

  openCreditCardModal(): void {
    this.isEditingCreditCard = false;
    this.editingCreditCardIndex = null;
    this.showCreditCardModal = true;
    this.creditCardForm.reset();
  }

  closeCreditCardModal(): void {
    this.showCreditCardModal = false;
    this.isEditingCreditCard = false;
    this.editingCreditCardIndex = null;
    this.creditCardForm.reset();
  }

  editCreditCard(cardIndex: number): void {
    if (cardIndex < 0 || cardIndex >= this.savedCards.length) {
      this.notificationService.showSnackBar(
        'Invalid card selection.',
        'failure'
      );
      return;
    }

    const cardToEdit = this.savedCards[cardIndex];

    this.isEditingCreditCard = true;
    this.editingCreditCardIndex = cardIndex;

    this.creditCardForm.patchValue({
      creditCardNumber:
        cardToEdit.creditCardNumber || cardToEdit.cardNumber || '',
      cardType: cardToEdit.cardType,
      expiryMonth: cardToEdit.expiryMonth,
      expiryYear: cardToEdit.expiryYear,
    });

    this.showCreditCardModal = true;

    this.cdRef.detectChanges();
  }

  onSaveCreditCard(): void {
    const formValue = this.creditCardForm.value;
    const hasAnyValue = Object.values(formValue).some(
      (value) => value && value.toString().trim() !== ''
    );

    if (!hasAnyValue) {
      this.closeCreditCardModal();
      return;
    }

    if (this.creditCardForm.valid) {
      const cardData = this.creditCardForm.value;

      if (this.isEditingCreditCard && this.editingCreditCardIndex !== null) {
        this.updateCreditCard(cardData, this.editingCreditCardIndex);
      } else {
        this.addCreditCard(cardData);
      }
      this.closeCreditCardModal();

      this.closeCreditCardModal();
    } else {
      this.markCreditCardFormTouched();
    }
  }

  private updateCreditCard(cardData: any, cardIndex: number): void {
    if (cardIndex < 0 || cardIndex >= this.savedCards.length) {
      this.notificationService.showSnackBar('Invalid card index.', 'failure');
      return;
    }

    const existingCard = this.savedCards[cardIndex];

    const updatedCard: SavedCard = {
      ...existingCard,
      last4: cardData.creditCardNumber?.slice(-4) || '',
      expiryMonth: cardData.expiryMonth,
      expiryYear: cardData.expiryYear,
      cardType: cardData.cardType,
      creditCardNumber: cardData.creditCardNumber,
      cardNumber: cardData.creditCardNumber,
      maskedNumber: `****-****-****-${
        cardData.creditCardNumber?.slice(-4) || ''
      }`,
    };

    this.savedCards[cardIndex] = updatedCard;
    this.cdRef.detectChanges();
  }

  private addCreditCard(cardData: any): void {
    const cardToken = this.generateCardToken(cardData.creditCardNumber);
    const newCardId = Date.now();

    const newCard: SavedCard = {
      id: newCardId,
      last4: cardData.creditCardNumber?.slice(-4) || '',
      expiryMonth: cardData.expiryMonth,
      expiryYear: cardData.expiryYear,
      cardType: cardData.cardType,
      cardToken: cardToken,
      creditCardNumber: cardData.creditCardNumber,
      cardNumber: cardData.creditCardNumber,
      maskedNumber: `****-****-****-${
        cardData.creditCardNumber?.slice(-4) || ''
      }`,
    };

    this.savedCards.push(newCard);

    this.userAddedCardIds.add(newCardId);

    this.cdRef.detectChanges();
  }

  private generateCardToken(cardNumber: string): string {
    const timestamp = Date.now().toString();
    const random = Math.random().toString(36).substring(2);
    return `card_${timestamp}_${random}`;
  }

  private markCreditCardFormTouched(): void {
    Object.values(this.creditCardForm.controls).forEach((control) => {
      control.markAsTouched();
    });
  }

  isCreditCardFieldInvalid(fieldName: string): boolean {
    const field = this.creditCardForm.get(fieldName);
    return field ? field.invalid && (field.touched || field.dirty) : false;
  }

  getCreditCardFieldError(fieldName: string): string {
    const field = this.creditCardForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required'])
        return `${this.getCreditCardFieldLabel(fieldName)} is required`;
      if (field.errors['pattern'])
        return `Invalid ${this.getCreditCardFieldLabel(fieldName)}`;
    }
    return '';
  }

  private getCreditCardFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      creditCardNumber: 'Credit Card Number',
      cardType: 'Card Type',
      expiryMonth: 'Month',
      expiryYear: 'Year',
    };
    return labels[fieldName] || fieldName;
  }

  onCardNumberInput(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    value = value.substring(0, 16);
    value = value.replace(/(\d{4})(?=\d)/g, '$1 ');
    event.target.value = value;
    this.creditCardForm.patchValue({
      creditCardNumber: value.replace(/\s/g, ''),
    });
  }

  hasCreditCardData(): boolean {
    try {
      if (!this.creditCardForm) {
        return false;
      }

      const formValue = this.creditCardForm.value;

      if (!formValue) {
        return false;
      }

      return Object.values(formValue).some(
        (value) => value && value.toString().trim() !== ''
      );
    } catch (error) {
      console.error('Error checking credit card data:', error);
      return false;
    }
  }

  removeCreditCard(cardIndex: number): void {
    const removedCard = this.savedCards[cardIndex];

    if (this.userAddedCardIds.has(removedCard.id)) {
      this.userAddedCardIds.delete(removedCard.id);
    }

    this.savedCards.splice(cardIndex, 1);
    this.cdRef.detectChanges();
  }

  onUploadImage(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.profileImageFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      const imageUrl = reader.result as string;

      const control = this.patientForm.get('profileUrl');
      control?.setValue(imageUrl);
      control?.markAsDirty();

      this.cdRef.markForCheck();
    };

    reader.readAsDataURL(file);
  }

  async onClose(): Promise<void> {
    if (await this.checkForUnsavedChanges()) {
      this.router.navigateByUrl('/patients/view');
    }
  }
  toggleSelectAll() {
    this.isAllSelected = !this.isAllSelected;

    if (this.isAllSelected) {
      this.selectedFiles = [...this.uploadedFiles()];
    } else {
      this.selectedFiles = [];
    }
    this.updateIndeterminateState();
  }
  isFileSelected(file: PatientAttachmentResponseDto): boolean {
    return this.isEditMode
      ? this.selectedFiles.some((f) => f.id === file.id)
      : this.selectedFiles.some((f) => f.dummyId === file.dummyId);
  }

  toggleFileSelection(file: PatientAttachmentResponseDto, event: Event) {
    const checked = (event.target as HTMLInputElement).checked;
    if (checked) {
      if (!this.isFileSelected(file)) {
        this.selectedFiles.push(file);
      }
    } else {
      this.selectedFiles = this.selectedFiles.filter(f =>
        this.isEditMode ? f.id !== file.id : f.dummyId !== file.dummyId);
    }
    this.updateSelectionFlags();
  }

  updateSelectionFlags() {
    const total = this.uploadedFiles().length;
    const selected = this.selectedFiles.length;

    this.isAllSelected = selected === total && total > 0;
    this.isIndeterminate = selected > 0 && selected < total;
  }

  updateIndeterminateState() {
    this.isIndeterminate = false;
  }

  async onClickDeleteDocument() {
    if (this.selectedFiles.length === 0) {
      this.notificationService.showSnackBar('Please select at least one file to delete.', 'normal');
      return;
    }

    const confirmed = await this.openConfirmation('Delete', 'Are you sure you want to delete the selected document(s)?');

    if (!confirmed) {
      return;
    }

    if (this.isEditMode) {
      const fileIds = this.selectedFiles
        .map(f => f.id)
        .filter((id): id is string => !!id);

      const idsToRemove = new Set(fileIds);
      this.uploadedFiles.set(this.uploadedFiles().filter(file => file.id && !idsToRemove.has(file.id)));
      this.selectedFiles = [];
      this.isAllSelected = false;

      this.patientService.deleteFiles(fileIds).subscribe({
        next: () => {
          this.notificationService.showSnackBar('Document deleted successfully', 'success');
          this.loadUploadedDocuments();
          this.updateIndeterminateState();
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to delete selected files.', 'failure');
          this.loadUploadedDocuments();
        }
      });

    } else {
      const dummyIdsToRemove = new Set(this.selectedFiles.map(f => f.dummyId).filter(Boolean));

      this.uploadedFiles.set(this.uploadedFiles().filter(file => {
        const matchByDummyId = file.dummyId && dummyIdsToRemove.has(file.dummyId);
        const matchByRef = this.selectedFiles.includes(file);
        return !(matchByDummyId || matchByRef);
      }));

      this.selectedFiles = [];
      this.isAllSelected = false;
      this.updateIndeterminateState();
      this.notificationService.showSnackBar('Document removed', 'success');
    }
  }


  onClickDownloadDocument() {
    if (this.selectedFiles.length === 0) {
      this.notificationService.showSnackBar(
        'Please select at least one file to download.',
        'failure'
      );
      return;
    }

    this.selectedFiles.forEach((file) => {
      if (file.fileUrl) {
        this.downloadFileFromUrl(file.fileUrl);
      }
    });

    this.selectedFiles = [];
    this.isAllSelected = false;
  }

  private downloadFileFromUrl(fileUrl: string) {
    const link = document.createElement('a');
    link.href = fileUrl;
    link.target = '_blank';
    link.download = '';
    link.click();
    link.remove();
  }
  // component.ts
  openFileModal(file: PatientAttachmentResponseDto): void {
    if (!file?.id) {
      this.notificationService.showSnackBar(
        'Invalid file selected.',
        'failure'
      );
      return;
    }

    this.isFileLoading = true;
    this.isFileModalOpen = false;

    this.patientService.getFileUrl(file.id).subscribe({
      next: (res: CommonOperationResponseDto) => {
        if (!res?.message) {
          this.notificationService.showSnackBar(
            'File URL not found.',
            'failure'
          );
          this.isFileLoading = false;
          return;
        }

        this.selectedFileUrl = res.message;

        // Detect file type from extension
        const ext = (file.extension || '').replace(/^\./, '').toLowerCase();
        if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'].includes(ext)) {
          this.selectedFileType = 'image';
        } else if (ext === 'pdf') {
          this.selectedFileType = 'pdf';
        } else if (
          ['doc', 'docx', 'xls', 'xlsx', 'ppt', 'pptx'].includes(ext)
        ) {
          this.selectedFileType = 'office';
        } else {
          this.selectedFileType = 'other';
        }

        this.isFileModalOpen = true;
        this.isFileLoading = false;
      },
      error: (err) => {
        console.error('Error fetching file URL:', err);
        this.notificationService.showSnackBar('Error loading file.', 'failure');
        this.isFileLoading = false;
      },
    });
  }

  /**
   * Closes file modal and resets selection
   */
  closeFileModal(): void {
    this.selectedFileUrl = null;
    this.selectedFileType = null;
    this.isFileModalOpen = false;
  }

  /**
   * Generates a safe download filename from the file URL
   */
  getDownloadFileName(): string {
    try {
      if (!this.selectedFileUrl) return 'download';
      const urlParts = this.selectedFileUrl.split('/');
      const fileNameWithQuery = urlParts[urlParts.length - 1];
      return fileNameWithQuery.split('?')[0] || 'download';
    } catch {
      return 'download';
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.patientForm.get(fieldName);
    return field ? field.invalid && (field.touched || field.dirty) : false;
  }

  getFieldError(fieldName: string): string {
    const field = this.patientForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required'])
        return `${this.labelFor(fieldName)} is required`;
      if (field.errors['pattern']) return `Invalid ${this.labelFor(fieldName)}`;
      if (field.errors['email'] || field.errors['invalidEmail']) return `Invalid email`;
      if (field.errors['whitespace']) return `${this.labelFor(fieldName)} cannot be empty or only spaces`;
    }
    return '';
  }

  labelFor(fieldName: string): string {
    const map: { [key: string]: string } = {
      firstName: 'First Name',
      lastName: 'Last Name',
      phoneNumber: 'Phone Number',
      gender: 'Gender',
      dateOfBirth: 'Date of Birth',
      patientNumber: 'Patient Number',
      drivingLicence: 'Driving Licence',
      email: 'Email',
      stateId: 'State',
      countryId: 'Country',
      postalCode: 'Postal Code',
      allergies: 'Allergies',
      visitTypeId: 'Visit Type',
      assignPhysicianId: 'Physician',
      status: 'Status',
      patientGoal: 'Patient Goal',
      counselorId: 'Counselor',
      outstandingRefundBalance: 'Outstanding Refund Balance',
    };
    return map[fieldName] || fieldName;
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach((control) => {
      control.markAsTouched();
    });
  }

  getInitials(): string {
    const first = this.patientForm?.get('firstName')?.value || '';
    const last = this.patientForm?.get('lastName')?.value || '';
    return `${first.charAt(0)}${last.charAt(0)}`.toUpperCase() || 'U';
  }

  openSelectCategoryDialog() {
    this.isOpenSelectCategoryDialog.set(true);
  }

  selectCategory(cat: DropDownResponseDto) {
    this.selectedCategory = cat;
  }

  proceed() {
    if (this.selectedCategory) {
      this.isOpenSelectCategoryDialog.set(false);
      this.openUploadDialog.set(true);
    }
  }

  // Step 2 - file selection
  onFileChange(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && this.selectedCategory) {
      Array.from(target.files).forEach((file) => {
        this.files.push({
          file,
          fileCategory: this.selectedCategory!.value,
          fileCategoryId: this.selectedCategory!.id as number,
        });
      });
    }
    target.value = '';
  }

  removeFile(index: number) {
    this.files.splice(index, 1);
  }

  save(action: 'close' | 'addMore') {
    const mappedFiles: PatientAttachmentResponseDto[] = this.files.map((f) => ({
      id: undefined,
      attachmentId: undefined,
      attachmentName: f.file.name,
      fileType: f.file.type,
      extension: f.file.name.split('.').pop() || '',
      createdAt: new Date().toISOString(),
      createdBy: '',
      updatedAt: undefined,
      updatedBy: undefined,
      categoryName: f.fileCategory,
      file: f.file,
      fileCategory: f.fileCategory,
      fileCategoryId: f.fileCategoryId,
      dummyId: this.tempIdCounter++,
    }));

    this.uploadedFiles.set([...this.uploadedFiles(), ...mappedFiles]);
    this.files = [];
    if (this.isEditMode) {
      this.constructUploadFilesFormData(this.patientId || 0);
    }
    if (action === 'close') {
      this.openUploadDialog.set(false);
      this.selectedCategory = null;
    } else if (action === 'addMore') {
      this.openUploadDialog.set(false);
      this.isOpenSelectCategoryDialog.set(true);
    }
  }

  cancel() {
    this.files = [];
    this.openUploadDialog.set(false);
    this.isOpenSelectCategoryDialog.set(false);
    this.selectedCategory = null;
  }

  loadSalesPerson(): void {
    this.isLoadingSales = true;

    this.userManagementService
      .getAllActiveSalesPerson()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: DropDownResponseDto[]) => {
          this.salesPersonList = response;
          this.filteredSalesPersonList = response;

          if (this.patientDataToLoad) {
            this.patchAutocompleteFields();
          }
        },
        error: () => {
          console.error('Failed to load salespeople.');
        },
        complete: () => {
          this.isLoadingSales = false;
        },
      });
  }

  loadDoctors(): void {
    this.isLoadingDoctor = true;

    this.userManagementService
      .getAllActiveDoctors()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: DropDownResponseDto[]) => {
          this.doctorsList = response;
          this.filteredDoctorsList = response;

          if (this.patientDataToLoad) {
            this.patchAutocompleteFields();
          }
        },
        error: () => {
          console.error('Failed to load doctors.');
        },
        complete: () => {
          this.isLoadingDoctor = false;
        },
      });
  }

  loadVisitTypes(): void {
    this.isLoadingVisitType = true;

    this.patientService
      .getAllActiveVisitType()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: DropDownResponseDto[]) => {
          this.visitTypeList = response;
        },
        error: () => {
          console.error('Failed to load visitType.');
        },
        complete: () => {
          this.isLoadingVisitType = false;
        },
      });
  }

  loadAgenda(): void {
    this.isLoadingAgenda = true;

    this.patientService
      .getAllActiveAgenda()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: DropDownResponseDto[]) => {
          this.agendaList = response;
          this.filteredagendaList = response;

          if (this.patientDataToLoad) {
            this.patchAutocompleteFields();
          }
        },
        error: () => {
          console.error('Failed to load agenda.');
        },
        complete: () => {
          this.isLoadingAgenda = false;
        },
      });
  }

  loadRefferal(): void {
    this.isLoadingAgenda = true;

    this.patientService
      .getAllActivePatients()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response: DropDownResponseDto[]) => {
          this.patientList = response;
          this.filteredPatientList = response;

          if (this.patientDataToLoad) {
            this.patchAutocompleteFields();
          }
        },
        error: () => {
          console.error('Failed to load agenda.');
        },
        complete: () => {
          this.isLoadingAgenda = false;
        },
      });
  }

  async onSaveAndClose(): Promise<void> {
    this.isSaveAndClose = true;
    await this.onSubmit();
  }

  togglePatientActiveStatus(isActivating: boolean): void {
    if (!this.patientId) return;

    const actionText = isActivating ? 'activate' : 'deactivate';

    this.confirmationService
      .openConfirmation({
        title: `Confirm ${actionText}`,
        message: `Are you sure you want to <strong>${actionText}</strong> this patient?`,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
      })
      .subscribe((confirmed) => {
        if (!confirmed) return;

        // Only runs when Yes is clicked
        this.patientService
          .togglePatientStatusById(this.patientId!, isActivating)
          .subscribe({
            next: (res: CommonOperationResponseDto) => {
              this.notificationService.showSnackBar(res.message, 'success');
              this.getPatientById();
            },
            error: () => {
              const message = isActivating
                ? 'Failed to activate the patient.'
                : 'Failed to deactivate the patient.';
              this.notificationService.showSnackBar(message, 'failure');
            },
          });
      });
  }
  private checkChanges(): { hasChanges: boolean; itemsText: string } {
    const formHasChanges = this.patientForm.dirty;
    const creditFormHasChanges = this.creditCardForm.dirty;
    const hasNewCardChanges = this.hasUserModifiedCreditCards();

    const hasFiles = this.files.length > 0;

    const hasChanges =
      formHasChanges || creditFormHasChanges || hasNewCardChanges || hasFiles;

    if (!hasChanges) {
      return { hasChanges: false, itemsText: '' };
    }

    let loseItems: string[] = [];

    if (formHasChanges) loseItems.push('form data');
    if (creditFormHasChanges) loseItems.push('unsaved credit card information');
    if (hasNewCardChanges) {
      const count = this.getNewCardsCount();
      if (count > 0) loseItems.push(`${count} newly added card(s)`);
    }
    if (hasFiles) loseItems.push(`${this.files.length} uploaded file(s)`);

    const itemsText = loseItems.join(', ');

    return { hasChanges: true, itemsText };
  }

  private async checkForUnsavedChanges(): Promise<boolean> {
    const { hasChanges, itemsText } = this.checkChanges();

    if (!hasChanges) {
      return true;
    }

    const message = 'You have unsaved changes. Continue without saving?';
    const confirmed = await this.openConfirmation('patient', message);
    return confirmed;
  }

  async onClickAddPatient() {
    if (await this.checkForUnsavedChanges()) {
      this.router.navigateByUrl('/patient/add');
    }
  }
  private hasUserModifiedCreditCards(): boolean {
    const creditCardFormHasData = this.hasCreditCardData();
    const hasUserAddedCards = this.userAddedCardIds.size > 0;
    const hasModifiedExistingCards = this.hasModifiedExistingCards();

    return (
      creditCardFormHasData || hasUserAddedCards || hasModifiedExistingCards
    );
  }

  private getNewCardsCount(): number {
    return this.userAddedCardIds.size;
  }

  private hasModifiedExistingCards(): boolean {
    if (this.initialSavedCardsFromAPI.length !== this.savedCards.length) {
      return true;
    }

    return this.savedCards.some((currentCard, index) => {
      const initialCard = this.initialSavedCardsFromAPI[index];
      if (!initialCard) return true;

      return (
        currentCard.creditCardNumber !== initialCard.creditCardNumber ||
        currentCard.cardType !== initialCard.cardType ||
        currentCard.expiryMonth !== initialCard.expiryMonth ||
        currentCard.expiryYear !== initialCard.expiryYear
      );
    });
  }

  async openConfirmation(action: string, message: string): Promise<boolean> {
    const data: ConfirmationDialogData = {
      title: `${action} Confirmation`,
      message: `${message}`,
    };

    const confirmed = await firstValueFrom(
      this.confirmationService.openConfirmation(data)
    );
    return confirmed ?? false;
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

  onPatientDelete(): void {
    if (!this.patientId) {
      return;
    }

    this.confirmationService
      .openConfirmation({
        title: 'Confirm Deletion',
        message:
          'Are you sure you want to <strong>delete</strong> this patient?',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
      })
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }

        this.patientService.bulkDeletePatients([this.patientId!]).subscribe({
          next: (res: CommonOperationResponseDto) => {
            this.notificationService.showSnackBar(
              'Patient deleted successfully',
              'success'
            );
            this.router.navigate(['/patients/view']);
          },
          error: (err) => {
            this.notificationService.showSnackBar(
              'Failed to delete patient',
              'failure'
            );
          },
        });
      });
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  getCardTypeFullName(type: CardTypeEnum): string {
    switch (type) {
      case CardTypeEnum.Visa:
        return 'Visa';
      case CardTypeEnum.MasterCard:
        return 'MasterCard';
      case CardTypeEnum.AmericanExpress:
        return 'American Express';
      case CardTypeEnum.Others:
        return 'Others';
      default:
        return '';
    }
  }

  onStateInput(event: Event) {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/[^A-Za-z]/g, ''); // keep only letters
    this.patientForm
      .get('stateId')
      ?.setValue(input.value, { emitEvent: false });
  }

  checkPatientLoggedInStatus(): void {
    this.isPatientLoggedIn = this.appSettingsService.isUserPatient();
  }
  displayCountryName = (id: number | string | null): string => {
    if (!id) return '';
    const found = this.countriesList.find((c) => c.id === id);
    return found ? found.value || '' : '';
  };
  onCountrySelected(selectedId: number | string): void {
    this.patientForm.get('countryId')?.setValue(selectedId);
  }
  onCountryInput(value: string): void {
    const filterValue = value.toLowerCase();
    this.filteredcountriesList = this.countriesList.filter((c) =>
      c.value.toLowerCase().includes(filterValue)
    );
  }
  private loadStatesByCountryId(countryId: number, stateIdToMatch?: string) {
    this.isLoadingStates.set(true);
    this.patientService.getAllActiveStates(countryId).subscribe({
      next: (states: DropDownResponseDto[]) => {
        this.allStates = states;
        this.cdRef.detectChanges();

        if (stateIdToMatch) {
          const matchedState = this.allStates.find(
            (s) => s.id === stateIdToMatch
          );
          if (matchedState) {
            this.patientForm
              .get('stateId')
              ?.setValue(matchedState.id, { emitEvent: false });
          }
        }
      },
      error: () => {
        this.notificationService.showSnackBar(
          'Failed to load states.',
          'failure'
        );
        this.allStates = [];
      },
      complete: () => this.isLoadingStates.set(false),
    });
  }

  onStateSelected(state: any): void {
    if (state && state.value) {
      this.patientForm.get('stateId')?.setValue(state, { emitEvent: false });
    }
  }
}
