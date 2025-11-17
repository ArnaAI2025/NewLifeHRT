import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ChangeDetectorRef,
  inject,
  Inject,
  signal,
  ApplicationRef,
  ViewChildren,
  ElementRef,
  QueryList,
  ViewChild,
} from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormControlName,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { UserManagementService } from '../user-management.service';
import { UserRole } from '../../../shared/enums/user-role.enum';
import { provideNgxMask } from 'ngx-mask';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { UserFormModel } from './../model/user-form-model';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserResponseDto } from './../model/user-response.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { ColorPickerComponent } from '../../../shared/components/color-picker/color-picker/color-picker';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { PatientService } from '../../patient/patient.services';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { CustomEmailValidator } from '../../../shared/validators/custom-email.validator';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';
import { PercentageDirective } from '../../../shared/directives/percentage.directive';

@Component({
  standalone: true,
  selector: 'app-user-management-form',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatIconModule,
    MatSelectModule,
    MatMenuModule,
    MatButtonModule,
    FullPageLoaderComponent,
    ColorPickerComponent,
    MatAutocompleteModule,
    PercentageDirective,
  ],
  providers: [provideNgxMask()],
  templateUrl: './user-management-form.html',
  styleUrl: './user-management-form.scss',
})
export class UserManagementFormComponent implements OnInit {
  @ViewChild('signatureInput') signatureInput!: ElementRef;

  userForm!: FormGroup;
  userRole = UserRole;
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly confirmationDialogService = inject(
    ConfirmationDialogService
  );
  private readonly patientService = inject(PatientService);
  FromPage!: number;
  userId = signal(0);
  isDeleted = signal(false);
  isLoadingPage = signal(true);
  isDataSaving = signal(false);
  countriesList: DropDownResponseDto[] = [];
  filteredcountriesList: DropDownResponseDto[] = [];
  isLoadingCountries = signal(false);
  availableServices: {
    id: string;
    serviceName: string;
    displayName: string;
  }[] = [];
  allStates: DropDownResponseDto[] = [];
  isLoadingStates = signal(false);
  hidePassword:boolean = true;
  signatureFile: File | null = null;
  timeZones: {
    id: number;
    standardName: string;
    abbreviation: string;
    displayName?: string;
  }[] = [];

  userModel: UserFormModel = {
    userName: '',
    firstName: '',
    lastName: '',
    password: '',
    email: '',
    phoneNumber: '',
    roleId: 0,
    address: {
      addressLine1: '',
      city: '',
      stateId: null,
      postalCode: '',
      countryId: null,
    },
    dea: '',
    npi: '',
    commisionInPercentage: 0,
    matchAsCommisionRate: true,
    replaceCommisionRate: '',
    isVacationApplicable: false,
    serviceIds: [],
    licenseInformation: [],
  };

  @ViewChildren(FormControlName, { read: ElementRef })
  formControls!: QueryList<ElementRef>;

  constructor(
    private formBuilder: FormBuilder,
    private readonly userManagementService: UserManagementService,
    private cdRef: ChangeDetectorRef,
    private notificationService: NotificationService,
    private appRef: ApplicationRef
  ) {}

  async ngOnInit() {
    this.activatedRoute.paramMap.subscribe(async (params) => {
      const fromPageParam = params.get('fromPage');
      this.FromPage =
        UserRole[fromPageParam as keyof typeof UserRole] || fromPageParam || '';
      this.userId.set(
        params.get('userId') !== null ? Number(params.get('userId')) : 0
      );
    });
    this.initializeForm();
    this.loadTimeZones();
    this.loadCountries();
    if (
      this.FromPage === UserRole.Doctor ||
      this.FromPage === UserRole.Receptionist ||
      this.FromPage === UserRole.Nurse
    ) {
      await this.clinicServiceByRoleType();
      this.userForm.get('timezoneId')?.setValidators([Validators.required]);
      this.userForm.get('timezoneId')?.updateValueAndValidity();
      this.userForm.get('color')?.setValidators([Validators.required]);
      this.userForm.get('color')?.updateValueAndValidity();
    }

    if (this.userId() > 0) {
      await this.userManagementService.getUserById(this.userId()).subscribe({
        next: (response: UserResponseDto) => {
          this.isDeleted.set(response.isDeleted || false);
          this.userModel = this.mapResponseToFormModel(response);
          this.updateFormFromModel();
        },
        error: () => {
          this.notificationService.showSnackBar(
            'Failed to fetch user details.',
            'failure',
            'Close',
            5000
          );
        },
      });
    } else {
      this.updateFormFromModel();
    }
    this.userForm.get('countryId')?.valueChanges.subscribe((countryId) => {
      if (countryId && typeof countryId === 'number') {
        this.loadStatesByCountryId(+countryId);
      } else {
        this.allStates = [];
        this.userForm.get('stateId')?.reset();
      }
    });
  }
  get licensesArray(): FormArray {
    return this.userForm.get('licensesArray') as FormArray;
  }

  get availableStates() {
    const chosenStateIds = this.licensesArray.controls.map(
      (c) => c.get('stateId')?.value
    );
    return this.allStates.filter((s) => !chosenStateIds.includes(s.id));
  }
  get licenseFormGroups(): FormGroup[] {
    return this.licensesArray.controls as FormGroup[];
  }

  addLicense(state: any) {
    this.licensesArray.push(
      this.formBuilder.group({
        stateId: [state.id, Validators.required],
        stateName: [state.value],
        licenseNumber: ['', Validators.required],
      })
    );
  }

  removeLicense(idx: number) {
    this.licensesArray.removeAt(idx);
  }
loadCountries(): Promise<void> {
  return new Promise((resolve) => {
    this.patientService.getAllActiveCountries().subscribe({
      next: (countries: DropDownResponseDto[]) => {
        this.countriesList = countries;
        this.filteredcountriesList = countries;
        resolve();
      },
      error: () => {
        this.notificationService.showSnackBar('Failed to load countries', 'failure');
        resolve();
      }
    });
  });
}

  private loadStatesByCountryId(countryId: number): void {
    this.isLoadingStates.set(true);
    this.patientService.getAllActiveStates(countryId).subscribe({
      next: (states: DropDownResponseDto[]) => {
        this.allStates = states;
        this.cdRef.detectChanges();
        const stateIdToMatch = this.userForm.get('stateId')?.value;
        if (this.allStates.length > 0 && stateIdToMatch) {
          const matchedState = this.allStates.find(
            (s) => s.id === stateIdToMatch
          );
          if (matchedState) {
            this.userForm
              .get('stateId')
              ?.setValue({ ...matchedState }, { emitEvent: false });
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
      complete: () => {
        this.isLoadingStates.set(false);
      },
    });
  }

  onCountrySelected(selectedId: number | string): void {
    this.userForm.get('countryId')?.setValue(selectedId);
  }

  displayCountryName = (id: number | string | null): string => {
    if (!id) return '';
    const found = this.countriesList.find((c) => c.id === id);
    return found ? found.value || '' : '';
  };
  private mapResponseToFormModel(data: UserResponseDto): UserFormModel {
    if (data.licenseInformation && data.licenseInformation.length > 0) {
      data.licenseInformation.forEach((license) => {
        this.licensesArray.push(
          this.formBuilder.group({
            stateId: [license.stateId, Validators.required],
            stateName: [license.stateName],
            licenseNumber: [license.number, Validators.required],
          })
        );
      });
    }
    return {
      userName: data.userName,
      firstName: data.firstName,
      lastName: data.lastName,
      password: data.password ?? '',
      email: data.email,
      phoneNumber: data.phoneNumber ?? '',
      signatureUrl: data.signatureUrl ?? '',
      roleId: data.roleId,
      address: {
        addressLine1: data.address?.addressLine1 ?? '',
        city: data.address?.city ?? '',
        stateId: data.address?.stateId ? Number(data.address.stateId) : null,
        postalCode: data.address?.postalCode ?? '',
        country: data.address?.country ?? '',
        countryId: data.address?.countryId ?? null,
      },
      dea: data.dea ?? '',
      npi: data.npi ?? '',
      commisionInPercentage: data.commisionInPercentage ?? null,
      matchAsCommisionRate: data.matchAsCommisionRate ?? false,
      replaceCommisionRate: data.replaceCommisionRate ?? '',
      isVacationApplicable: (data as any).isVacationApplicable ?? false,
      serviceIds: data.serviceIds ?? [],
      timezoneId: data.timezoneId ?? null,
      color: data.color ?? null,
      licenseInformation: data.licenseInformation ?? [],
    };
  }

  async clinicServiceByRoleType() {
    this.userManagementService
      .clinicServiceByRoleType('appointment')
      .subscribe({
        next: (response) => {
          if (Array.isArray(response)) {
            this.availableServices = response;
            this.addDynamicServiceControls();
          }
        },
        error: () => {
          this.notificationService.showSnackBar(
            'Failed to load clinic services.',
            'failure'
          );
        },
      });
  }

  private initializeForm(): void {
    this.userForm = this.formBuilder.group({
      userName: [{ value: '', disabled: this.userId() > 0 }, [Validators.required, Validators.minLength(3), this.noSpaceValidator()]],
      firstName: ['', [Validators.required,NoWhitespaceValidator]],
      lastName: ['', [Validators.required,NoWhitespaceValidator]],
      password: this.userId() > 0 ? [''] : ['', [Validators.required, Validators.minLength(6)]],
      email: ['', [Validators.required, Validators.email,CustomEmailValidator]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^(\+?1\s?)?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$/)]],
      dea: [''],
      npi: [''],
      commisionInPercentage: [null],
      matchAsCommisionRate: [true],
      replaceCommisionRate: [null],
      addressLine1: [''],
      city: [''],
      postalCode: [''],
      isVacationApplicable: [false],
      timezoneId: ['', Validators.required],
      color: ['#d53434', Validators.required],
      services: this.formBuilder.group({}),
      countryId: [null],
      stateId: [null],
      signatureUrl: [''],
      licensesArray: this.formBuilder.array([]),
    });
    this.applyReplaceCommissionValidators(
      this.userForm.get('matchAsCommisionRate')?.value === true
    );

    this.userForm
      .get('matchAsCommisionRate')
      ?.valueChanges.subscribe((checked) => {
        this.applyReplaceCommissionValidators(checked === true);
      });

    this.userForm.valueChanges.subscribe((value) =>
      this.updateModelFromForm(value)
    );
  }

  private addDynamicServiceControls(): void {
    const servicesGroup = this.userForm.get('services') as FormGroup;
    if (!servicesGroup) return;

    this.availableServices.forEach((service) => {
      if (!servicesGroup.get(service.serviceName)) {
        servicesGroup.addControl(
          service.serviceName,
          this.formBuilder.control(false)
        );
      }
    });

    this.cdRef.detectChanges();
  }
  private applyReplaceCommissionValidators(isMatch: boolean): void {
    if (this.FromPage !== UserRole.SalesPerson) {
      return;
    }
    const replaceControl = this.userForm.get('replaceCommisionRate');
    if (!replaceControl) {
      return;
    }
    if (isMatch) {
      replaceControl.clearValidators();
      replaceControl.reset();
    } else {
      replaceControl.setValidators([Validators.required]);
    }
    replaceControl.updateValueAndValidity({ emitEvent: false });
  }
  private updateFormFromModel(): void {
    const stateIdToMatch = this.userModel?.address?.stateId;
    this.userForm.patchValue({
      userName: this.userModel.userName,
      firstName: this.userModel.firstName,
      lastName: this.userModel.lastName,
      password: this.userModel.password,
      email: this.userModel.email,
      phoneNumber: this.userModel.phoneNumber,
      signatureUrl: this.userModel.signatureUrl,
      dea: this.userModel.dea,
      npi: this.userModel.npi,
      commisionInPercentage: this.userModel.commisionInPercentage,
      matchAsCommisionRate: this.userModel.matchAsCommisionRate,
      replaceCommisionRate: this.userModel.replaceCommisionRate,
      addressLine1: this.userModel.address.addressLine1,
      city: this.userModel.address.city,
      postalCode: this.userModel.address.postalCode,
      countryId: this.userModel.address.countryId,
      stateId: stateIdToMatch,
      isVacationApplicable: this.userModel.isVacationApplicable,
      timezoneId: this.userModel.timezoneId,
      color: this.userModel.color || '#d53434',
    });

    if (this.userId() > 0) {
      this.userForm.get('userName')?.disable();
    }

    // Set services checkboxes
    const servicesGroup = this.userForm.get('services') as FormGroup;
    if (servicesGroup) {
      Object.keys(servicesGroup.controls).forEach((controlName) => {
        servicesGroup.get(controlName)?.setValue(false, { emitEvent: false });
      });

      if (Array.isArray(this.userModel.serviceIds)) {
        this.userModel.serviceIds.forEach((serviceId) => {
          const matchedService = this.availableServices.find(
            (s) => s.id === serviceId
          );
          if (matchedService) {
            servicesGroup
              .get(matchedService.serviceName)
              ?.setValue(true, { emitEvent: false });
          }
        });
      }
    }
    if (stateIdToMatch && this.allStates.length > 0) {
      const matchedState = this.allStates.find((s) => s.id === stateIdToMatch);
      if (matchedState) {
        this.userForm
          .get('stateId')
          ?.setValue({ ...matchedState }, { emitEvent: false });
      }
    }
    this.markFormPristineAndUntouched(this.userForm);
    this.isLoadingPage = signal(false);
  }

  onStateSelected(state: any): void {
    if (state && state.value) {
      this.userForm.get('stateId')?.setValue({ ...state }, { emitEvent: true });
    }
  }

  private updateModelFromForm(formValue: any): void {
    const stateObj =
      formValue.stateId && typeof formValue.stateId === 'object'
        ? { ...formValue.stateId }
        : null;
    this.userModel = {
      ...this.userModel,
      userName: formValue.userName || '',
      firstName: formValue.firstName || '',
      lastName: formValue.lastName || '',
      password: formValue.password || '',
      email: formValue.email || '',
      phoneNumber: formValue.phoneNumber || '',
      dea: formValue.dea || '',
      npi: formValue.npi || '',
      commisionInPercentage: formValue.commisionInPercentage || null,
      matchAsCommisionRate: formValue.matchAsCommisionRate || false,
      replaceCommisionRate: formValue.replaceCommisionRate || '',
      isVacationApplicable: formValue.isVacationApplicable || false,
      address: {
        addressLine1: formValue.addressLine1 || '',
        city: formValue.city || '',
        stateId: stateObj,
        countryId: formValue.countryId || null,
        postalCode: formValue.postalCode || '',
      },
      licenseInformation: this.licensesArray.controls.map((control) => ({
        stateId: control.get('stateId')?.value,
        number: control.get('licenseNumber')?.value,
      })),
    };
  }
  onOpenEditMode() {
    this.router.navigate([
      '/edit',
      this.userRole[this.FromPage],
      this.userId(),
    ]);
  }
  private getInvalidControls(): string[] {
    const invalidControls: string[] = [];
    const controls = this.userForm.controls;
    for (const name in controls) {
      if (controls[name].invalid) {
        invalidControls.push(name);
      }
    }
    return invalidControls;
  }

  async onSaveInformation(): Promise<number> {
    return new Promise(async (resolve) => {
      this.isDataSaving.set(true);
      if (this.userForm.invalid) {
        this.markFormGroupTouched();
        this.appRef.whenStable().then(() => {
          this.scrollToFirstInvalidControl();
        });
        this.notificationService.showSnackBar(
          'Please fill in all required fields correctly.',
          'normal',
          'Close'
        );
        this.isDataSaving.set(false);
        return resolve(0);
      }

      const formData = new FormData();
      const formValue = this.userForm.getRawValue();
      const servicesGroup = this.userForm.get('services') as FormGroup;

      const selectedServiceIds = this.availableServices
        .filter((service) => servicesGroup?.get(service.serviceName)?.value)
        .map((service) => service.id);

      formData.append('UserName', formValue.userName);
      formData.append('FirstName', formValue.firstName.trim());
      formData.append('LastName', formValue.lastName.trim());
      formData.append('Email', formValue.email);
      formData.append('PhoneNumber', formValue.phoneNumber || '');
      formData.append('Password', formValue.password);
      formData.append('RoleId', this.FromPage.toString());
      formData.append('DEA', formValue.dea || '');
      formData.append('NPI', formValue.npi || '');
      formData.append(
        'CommisionInPercentage',
        formValue.commisionInPercentage?.toString() || ''
      );
      formData.append(
        'MatchAsCommisionRate',
        (formValue.matchAsCommisionRate || false).toString()
      );
      formData.append(
        'ReplaceCommisionRate',
        formValue.replaceCommisionRate || ''
      );
      formData.append(
        'IsVacationApplicable',
        formValue.isVacationApplicable?.toString() || 'false'
      );
      formData.append('TimezoneId', formValue.timezoneId?.toString() || '');
      formData.append('Color', formValue.color || '#d53434');
      formData.append('MustChangePassword', (this.userId() === 0).toString());

      // Append address properties with exact DTO structure
      formData.append('Address.AddressLine1', formValue.addressLine1);
      formData.append('Address.City', formValue.city || '');
      formData.append('Address.PostalCode', formValue.postalCode || '');
      formData.append(
        'Address.CountryId',
        formValue.countryId?.toString() || ''
      );
      formData.append('Address.StateId', formValue.stateId?.id || '');

      // Append service IDs (List<Guid>) with exact property name
      if (selectedServiceIds && selectedServiceIds.length > 0) {
        selectedServiceIds.forEach((id) => {
          formData.append('ServiceIds', id.toString());
        });
      }

      // Append license information with exact property names
      if (this.licensesArray.length > 0) {
        this.licensesArray.controls.forEach((control, index) => {
          const stateId = control.get('stateId')?.value;
          const licenseNumber = control.get('licenseNumber')?.value;

          if (stateId && licenseNumber) {
            formData.append(
              `LicenseInformation[${index}].StateId`,
              stateId.toString()
            );
            formData.append(
              `LicenseInformation[${index}].Number`,
              licenseNumber
            );
          }
        });
      }

      // Append signature file with exact property name
      if (this.signatureFile) {
        formData.append('SignatureFile', this.signatureFile);
      }

      // Submit the form data
      if (this.userId() < 1) {
        this.userManagementService.addUser(formData).subscribe({
          next: (res: CommonOperationResponseDto) => {
            this.userId.set(Number(res?.id));
            this.markFormPristineAndUntouched(this.userForm);
            this.notificationService.showSnackBar(
              `${this.userRole[this.FromPage]} created successfully`,
              'success'
            );
            this.onOpenEditMode();
            resolve(1);
            this.isDataSaving.set(false);
          },
          error: (e) => {
            var msg =
              e.error?.message ??
              `Failed to create ${this.userRole[this.FromPage].toLowerCase()}`;
            this.notificationService.showSnackBar(msg, 'failure', 'Close');
            resolve(-1);
            this.isDataSaving.set(false);
          },
        });
      } else {
        this.userManagementService
          .updateUser(this.userId(), formData)
          .subscribe({
            next: () => {
              this.markFormPristineAndUntouched(this.userForm);
              this.notificationService.showSnackBar(
                `${this.userRole[this.FromPage]} updated successfully`,
                'success'
              );
              resolve(1);
              this.isDataSaving.set(false);
            },
            error: () => {
              this.notificationService.showSnackBar(
                `Failed to update ${this.userRole[
                  this.FromPage
                ].toLowerCase()}`,
                'failure',
                'Close'
              );
              resolve(-1);
              this.isDataSaving.set(false);
            },
          });
      }
    });
  }

  async onSaveAndClose(): Promise<void> {
    const status = await this.onSaveInformation();
    if (status > 0) {
      this.onClose();
    }
  }

  private markFormGroupTouched(): void {
    Object.values(this.userForm.controls).forEach((control) => {
      if (control instanceof FormGroup) {
        Object.values(control.controls).forEach((ctrl) => ctrl.markAsTouched());
      } else {
        control.markAsTouched();
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.userForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }
  onCountryInput(value: string): void {
    const filterValue = value.toLowerCase();
    this.filteredcountriesList = this.countriesList.filter((c) =>
      c.value.toLowerCase().includes(filterValue)
    );
  }
  get countryControl(): FormControl {
    return this.userForm.get('countryId') as FormControl;
  }

  getFieldError(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    const formattedName = this.formatFieldName(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${formattedName} is required`;
      if (field.errors['whitespace']) return `${formattedName} cannot be empty or only spaces`;
      if (field.errors['email'] || field.errors['invalidEmail']) return 'Please enter a valid email';
      if (field.errors['minlength']) return `${formattedName} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['pattern']) return `${formattedName} format is invalid`;
      if (field.errors['noSpace']) return `${formattedName} cannot contain spaces`;
    }
    return '';
  }

  labelFor(fieldName: string): string {
    const map: { [key: string]: string } = {
      userName: 'Username',
      firstName: 'First Name',
      lastName: 'Last Name',
      password: 'Password',
      email: 'Email',
      phoneNumber: 'Phone Number',
      dea: 'DEA Number',
      npi: 'NPI Number',
      commisionInPercentage: 'Commission Percentage',
      matchAsCommisionRate: 'Match As Commission Rate',
      replaceCommisionRate: 'Replace Commission Rate',
      addressLine1: 'Address Line 1',
      city: 'City',
      postalCode: 'Postal Code',
      stateId: 'State / Province',
      countryId: 'Country',
      isVacationApplicable: 'Vacation Applicable',
      timezoneId: 'Timezone',
      color: 'Color Code',
      signatureUrl: 'Signature',
    };
    return map[fieldName] || fieldName;
  }

  resetForm(): void {
    this.userForm.reset();
    this.userModel = {
      userName: '',
      firstName: '',
      lastName: '',
      password: '',
      email: '',
      phoneNumber: '',
      roleId: 0,
      dea: '',
      npi: '',
      commisionInPercentage: 0,
      matchAsCommisionRate: true,
      replaceCommisionRate: '',
      address: {
        addressLine1: '',
        city: '',
        postalCode: '',
        countryId: null,
        stateId: null,
      },
      isVacationApplicable: false,
      serviceIds: [],
      licenseInformation: [],
    };
  }
  onClickAdd(): void {
    if (this.userForm.dirty) {
      this.confirmationDialogService
        .openConfirmation({
          title: 'Unsaved Changes',
          message:
            'You have unsaved changes. Are you sure to continue without saving?',
        })
        .subscribe((confirmed) => {
          if (confirmed) {
            this.resetForm();
            this.userId.set(0);
            this.updateFormFromModel();
            this.router.navigate(['/add', this.userRole[this.FromPage]]);
          }
        });
    } else {
      this.resetForm();
      this.userId.set(0);
      this.updateFormFromModel();
      this.router.navigate(['/add', this.userRole[this.FromPage]]);
    }
  }
  onClose(): void {
    if (this.userForm.dirty) {
      this.confirmationDialogService
        .openConfirmation({
          title: 'Unsaved Changes',
          message:
            'You have unsaved changes. Are you sure you want to close without saving?',
        })
        .subscribe((confirmed) => {
          if (confirmed) {
            this.navigateBasedOnUserRole();
          }
          // if not confirmed, do nothing (stay on the page)
        });
    } else {
      this.navigateBasedOnUserRole();
    }
  }
  private markFormPristineAndUntouched(formGroup: FormGroup) {
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
  private navigateBasedOnUserRole(): void {
    switch (this.FromPage) {
      case UserRole.Doctor:
        this.router.navigate(['/doctor/view']);
        break;

      case UserRole.Receptionist:
        this.router.navigate(['/receptionist/view']);
        break;

      case UserRole.Nurse:
        this.router.navigate(['/nurse/view']);
        break;

      case UserRole.Admin:
        this.router.navigate(['/admin/view']);
        break;
      case UserRole.SalesPerson:
        this.router.navigate(['/sales-person/view']);
        break;

      default:
        break;
    }
  }

  onDeleteClick(): void {
    const roleName = this.userRole[this.FromPage]; // Moved out so available for all scopes
    this.confirmationDialogService
      .openConfirmation({
        title: 'Confirm Delete',
        message: `Are you sure you want to delete this ${roleName}?`,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel',
        showCancelButton: true,
      })
      .subscribe((confirmed) => {
        if (confirmed) {
          this.userManagementService.deleteById(this.userId()).subscribe({
            next: (res: CommonOperationResponseDto) => {
              if (res.id) {
                this.notificationService.showSnackBar(
                  `${roleName} deleted successfully`,
                  'success'
                );
                this.onClose();
              }
              else {
                this.notificationService.showSnackBar(
                  `Failed to delete ${roleName}`,
                  'failure'
                );
              }
            },
            error: () => {
              this.notificationService.showSnackBar(
                `Failed to delete ${roleName}`,
                'failure'
              );
            },
          });
        }
      });
  }

  onToggleActive(action: boolean): void {
    if (this.userId() <= 0) {
      this.notificationService.showSnackBar('No user', 'failure');
      return;
    }
    const actionText = action ? 'activate' : 'deactivate';

    this.confirmationDialogService
      .openConfirmation({
        title: `Confirm Action`,
        message: `Are you sure you want to <strong>${actionText}</strong> this user?`,
        confirmButtonText:
          actionText.charAt(0).toUpperCase() + actionText.slice(1),
        cancelButtonText: 'Cancel',
        showCancelButton: true,
      })
      .subscribe((confirmed) => {
        if (confirmed) {
          this.userManagementService
            .bulkToggleActive([this.userId()], action)
            .subscribe({
              next: () => {
                this.isDeleted.set(!action);
                this.notificationService.showSnackBar(
                  `${
                    this.userRole[this.FromPage]
                  } ${actionText}d successfully.`,
                  'success'
                );
              },
              error: () => {
                this.notificationService.showSnackBar(
                  `Failed to ${actionText} the ${this.userRole[
                    this.FromPage
                  ].toLowerCase()}`,
                  'failure'
                );
              },
            });
        }
      });
  }

  pascalToSpace(str: string): string {
    return str.replace(/([A-Z])/g, ' $1').trim();
  }

  private noSpaceValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      const hasSpace = /\s/.test(control.value);
      return hasSpace ? { noSpace: true } : null;
    };
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
  private loadTimeZones(): void {
    this.userManagementService.getAllTimezones().subscribe({
      next: (response: any[]) => {
        this.timeZones = response.map((tz) => ({
          id: tz.id,
          standardName: tz.standardName,
          abbreviation: tz.abbreviation,
          displayName: `${tz.standardName} (${tz.abbreviation})`,
        }));
        this.cdRef.detectChanges();
      },
      error: () => {
        this.notificationService.showSnackBar(
          'Failed to load timezones.',
          'failure'
        );
      },
    });
  }

  onStateInput(event: Event) {
    const input = event.target as HTMLInputElement;
    this.userForm.get('stateId')?.setValue(input.value, { emitEvent: false });
  }
  onSignatureSelected(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    // Validate file type
    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'image/gif',
      'image/webp',
      'application/pdf',
    ];
    if (!allowedTypes.includes(file.type)) {
      this.notificationService.showSnackBar(
        'Only image files and PDFs are allowed.',
        'failure'
      );
      return;
    }

    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      this.notificationService.showSnackBar(
        'File size must be less than 5MB.',
        'failure'
      );
      return;
    }

    this.signatureFile = file;
    // Create preview URL
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.userForm.get('signatureUrl')?.setValue(e.target.result);
      this.cdRef.detectChanges();
    };
    reader.readAsDataURL(file);
  }

  removeSignature(): void {
    this.signatureInput.nativeElement.value = '';
    this.signatureFile = null;
    this.userForm.get('signatureUrl')?.setValue('');
    this.cdRef.detectChanges();
  }
  compareStates(s1: DropDownResponseDto, s2: DropDownResponseDto): boolean {
    return s1 && s2 ? s1.id === s2.id : s1 === s2;
  }

  formatFieldName(fieldName: string): string {
    const formatted = fieldName.replace(/([A-Z])/g, ' $1').trim();
    return formatted.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
  }

}
