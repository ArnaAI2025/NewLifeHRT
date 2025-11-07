import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  OnDestroy,
  signal,
  ViewChildren,
  ElementRef,
  QueryList,
  ApplicationRef,
  ChangeDetectorRef,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  FormControlName,
  FormControl,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatNativeDateModule } from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import {
  MatAutocompleteModule,
  MatAutocompleteSelectedEvent,
} from '@angular/material/autocomplete';
import { provideNgxMask } from 'ngx-mask';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, firstValueFrom } from 'rxjs';

import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { LeadDisplay } from '../model/lead-display.model';
import { UserManagementService } from '../../user-management/user-management.service';
import { LeadManagementService } from '../lead-management.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';
import { PatientService } from '../../patient/patient.services';
import { LeadNavigationBar } from '../lead-navigation-bar/lead-navigation-bar';
import { Gender } from '../../../shared/enums/gender.enum';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';
import { state } from '@angular/animations';
import { MatSelectModule } from '@angular/material/select';
import { CustomEmailValidator } from '../../../shared/validators/custom-email.validator';
import { DateValidators } from '../../../shared/validators/date-validators';

@Component({
  selector: 'app-lead-management-add',
  templateUrl: './lead-management-add.html',
  styleUrls: ['./lead-management-add.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatNativeDateModule,
    //NgxMaskDirective,
    MatMenuModule,
    MatAutocompleteModule,
    LeadNavigationBar,
    MatSelectModule,
  ],
  providers: [provideNgxMask()],
})
export class LeadManagementAddComponent implements OnInit {
  userForm!: FormGroup;
  leadId = signal<string | null>(null);
  isActive = signal<boolean>(false);
  isSaving = signal(false);
  activeView = signal<'form' | 'sms'>('form');
  maxDate: Date = DateValidators.getToday();
  
  salesPersonList: DropDownResponseDto[] = [];
  filteredSalesPersonList: DropDownResponseDto[] = [];
  isLoadingSalesPerson = signal(false);
  countriesList: DropDownResponseDto[] = [];
  filteredcountriesList: DropDownResponseDto[] = [];
  isLoadingCountries = signal(false);

  allStates: DropDownResponseDto[] = [];
  isLoadingStates = signal(false);

  genderOptions = Object.entries(Gender)
    .filter(([_, value]) => typeof value === 'number')
    .map(([key, value]) => ({ id: value as number, label: key }));

  private isFormSaved = false;
  @ViewChildren(FormControlName, { read: ElementRef })
  formControls!: QueryList<ElementRef>;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userManagementService: UserManagementService,
    private notificationService: NotificationService,
    private confirmationDialogService: ConfirmationDialogService,
    private leadService: LeadManagementService,
    private appRef: ApplicationRef,
    private readonly patientService: PatientService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  async ngOnInit(): Promise<void> {
    this.initializeForm();
    this.loadSalesPersons();
    this.loadCountries();
    this.subscribeToRouteParams();
    this.trackFormChanges();
  }

  private initializeForm(): void {
    this.userForm = this.fb.group({
      subject: [null, [Validators.required,NoWhitespaceValidator]],
      firstName: [null, [Validators.required,NoWhitespaceValidator]],
      lastName: [null, [Validators.required,NoWhitespaceValidator]],
      //phoneNumber: [null],
      phoneNumber: [
        null,
        [
          Validators.pattern(
            /^(\+?1\s?)?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$/
          ),
        ],
      ],
      addressLine1: [null],
      email: [null, [Validators.required, Validators.email,CustomEmailValidator]],
      dateOfBirth: [null],
      gender: [null],
      salesPersonId: [null, Validators.required],
      highLevelOwner: [null],
      description: [null],
      tags: [null],
      addressId: [null],
      ownerId: [null],
      addressDto: this.fb.group({
        addressLine1: [null],
        city: [null],
        stateId: [null],
        postalCode: [null],
        countryId: [null],
      }),
    });
  }

  private subscribeToRouteParams(): void {
    this.route.paramMap.subscribe(async (params) => {
      this.leadId.set(params.get('id'));
      const id = this.leadId();
      if (id !== null) {
        await this.loadLeadDetails(id);
      }
    });
  }

  private trackFormChanges(): void {
    this.userForm.valueChanges.subscribe(() => {
      this.isFormSaved = false;
    });
    const addressGroup = this.userForm.get('addressDto') as FormGroup;
    addressGroup.get('countryId')?.valueChanges.subscribe((countryId) => {
      if (countryId && typeof countryId === 'number') {
        this.loadStatesByCountryId(countryId);
        addressGroup.get('stateId')?.setValue(null);
      } else {
        this.allStates = [];
        addressGroup.get('stateId')?.setValue(null);
      }
    });
  }
  private loadStatesByCountryId(countryId: number, stateIdToMatch?: string) {
    this.isLoadingStates.set(true);
    this.patientService.getAllActiveStates(countryId).subscribe({
      next: (states: DropDownResponseDto[]) => {
        this.allStates = states;
        this.cdr.detectChanges();
        if (stateIdToMatch) {
          const matchedState = this.allStates.find(
            (s) => s.id === stateIdToMatch
          );
          if (matchedState) {
            (this.userForm.get('addressDto') as FormGroup)
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
      this.userForm.get('stateId')?.setValue(state, { emitEvent: false });
    }
  }
  private loadSalesPersons(): void {
    this.isLoadingSalesPerson.set(true);

    this.userManagementService.getAllActiveSalesPerson()
      .pipe(finalize(() => this.isLoadingSalesPerson.set(false)))
      .subscribe({
        next: (response) => {
          this.salesPersonList = response;
          this.filteredSalesPersonList = response;
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to load sales persons', 'failure');
        }
      });
  }

  private async loadLeadDetails(id: string): Promise<void> {
    try {
      const lead = await firstValueFrom(this.leadService.getLeadById(id));
      if (lead) {
        this.populateFormWithLeadData(lead);
        this.isActive.set(!!lead.isActive);
      }
    } catch (error) {
      this.notificationService.showSnackBar(
        'Failed to load lead details',
        'failure'
      );
    }
  }

  async loadCountries(): Promise<void> {
    this.isLoadingCountries.set(true);
    return new Promise((resolve, reject) => {
      this.patientService.getAllActiveCountries().subscribe({
        next: (countries: DropDownResponseDto[]) => {
          this.countriesList = countries;
          this.filteredcountriesList = countries;
          resolve();
        },
        error: (err) => {
          this.notificationService.showSnackBar(
            'Failed to load countries',
            'failure'
          );
          reject(err);
        },
        complete: () => this.isLoadingCountries.set(false),
      });
    });
  }

  displayCountryName = (id: number | string | null): string => {
    if (!id) return '';
    const found = this.countriesList.find((c) => c.id === id);
    return found ? found.value || '' : '';
  };
  displayStateName = (id: number | string | null): string => {
    if (!id) return '';
    const found = this.allStates.find((c) => c.id === id);
    return found ? found.value || '' : '';
  };

  get countryControl(): FormControl {
    return this.userForm.get('addressDto.countryId') as FormControl;
  }

  onCountryInput(value: string): void {
    const filterValue = value.toLowerCase();
    this.filteredcountriesList = this.countriesList.filter((c) =>
      c.value.toLowerCase().includes(filterValue)
    );
  }

  onCountrySelected(selectedId: number | string): void {
    const addressGroup = this.userForm.get('addressDto') as FormGroup;
    addressGroup.get('countryId')?.setValue(selectedId);
    // reset state on country change
    addressGroup.get('stateId')?.setValue(null);
    this.loadStatesByCountryId(Number(selectedId));
  }

  private populateFormWithLeadData(lead: any): void {
    const address = lead.address || {};
    this.userForm.patchValue({
      ...lead,
      dateOfBirth: lead.dateOfBirth ? new Date(lead.dateOfBirth) : null,
      gender: lead.gender,
      salesPersonId: lead.ownerId,
      countryId: lead.countryId,
      addressDto: {
        addressLine1: address.addressLine1 || null,
        city: address.city || null,
        postalCode: address.postalCode || null,
        countryId: address.countryId || null,
        stateId: address.stateId || null,
      },
    });
    if (address.countryId) {
      this.loadStatesByCountryId(address.countryId, address.stateId);
    }
  }

  getSelectedSalesPersonValue(): string {
    const id =
      this.userForm.get('salesPersonId')?.value ??
      this.userForm.get('ownerId')?.value;
    const person = this.salesPersonList.find((sp) => sp.id === id);
    return person ? person.value : '';
  }

  displayGenderLabel = (genderId: number | string | null): string => {
      if (genderId) {
          const gender = this.genderOptions.find(g => g.id === genderId);
          return gender ? gender.label : '';
      }
      return '';
  };

  isFieldInvalid(field: string, group: FormGroup = this.userForm): boolean {
    const control = group.get(field);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  getFieldError(
    field: string,
    group: FormGroup = this.userForm
  ): string | null {
    const control = group.get(field);
    const formattedName = this.formatFieldName(field);
    if (!control || !control.errors) return null;
    if (control.errors['required']) return `${formattedName}  is required`;
    if (control.errors['email']) return 'Please enter a valid email';
    if (control.errors['pattern']) return `${formattedName} format is invalid`;
    if (control.errors['minlength']) return `${formattedName} must be at least ${control.errors['minlength'].requiredLength} characters`;
    if (control.errors['whitespace']) return `This field cannot be empty or only spaces`;

    return null;
  }

  private resetFormState(): void {
    this.userForm.markAsPristine();
    this.userForm.markAsUntouched();
    this.markNestedControlsAsPristine(this.userForm);
    this.isFormSaved = true;
  }

  private markNestedControlsAsPristine(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        control.markAsPristine();
        control.markAsUntouched();
        this.markNestedControlsAsPristine(control);
      } else if (control) {
        control.markAsPristine();
        control.markAsUntouched();
      }
    });
  }

  onSalesPersonSelected(event: MatAutocompleteSelectedEvent): void {
    const id = event.option.value;
    this.userForm.patchValue({ salesPersonId: id, ownerId: id });
  }

  onGenderSelected(event: MatAutocompleteSelectedEvent): void {
    const id = event.option.value;
    this.userForm.get('gender')?.setValue(id);
  }

  async onSubmit(isSaveAndClose = false): Promise<void> {
    if (!this.userForm.valid) {
      this.userForm.markAllAsTouched();
      this.appRef.whenStable().then(() => {
        this.scrollToFirstInvalidControl();
      });
      this.notificationService.showSnackBar(
        'Please fill all the required field in the form.',
        'normal'
      );
      return;
    }

    this.isSaving.set(true);
    const payload = this.preparePayload();

    try {
      if (this.leadId()) {
        await this.updateLead(payload);
      } else {
        await this.createLead(payload);
      }
      if (isSaveAndClose) {
        this.navigateToView();
      }
      this.resetFormState();
    } catch (error) {
      this.notificationService.showSnackBar(
        'Save operation failed.',
        'failure'
      );
    } finally {
      this.isSaving.set(false);
    }
  }

  private preparePayload(): any {
  const raw = this.userForm.value;

  return {
    subject: raw.subject.trim(),
    firstName: raw.firstName.trim(),
    lastName: raw.lastName.trim(),
    phoneNumber: raw.phoneNumber || null,
    email: raw.email || null,
    dateOfBirth: raw.dateOfBirth ? this.formatDate(raw.dateOfBirth) : null,
    gender: raw.gender,
    highLevelOwner: raw.highLevelOwner,
    description: raw.description,
    tags: raw.tags,
    ownerId: raw.salesPersonId ?? raw.ownerId,
    addressId: raw.addressId ?? null,
    addressDto: raw.addressDto ?? null,
  };
}

  private async updateLead(payload: LeadDisplay): Promise<void> {
    try {
      await firstValueFrom(
        this.leadService.updateLead(this.leadId()!, payload)
      );
      this.notificationService.showSnackBar(
        'Lead updated successfully',
        'success'
      );
    } catch (error: any) {
      this.notificationService.showSnackBar(
        error?.error?.message ?? 'Failed to update Lead',
        'failure'
      );
    }
  }

  private async createLead(payload: LeadDisplay): Promise<void> {
    try {
      const createdLead = await firstValueFrom(
        this.leadService.createLead(payload)
      );
      this.notificationService.showSnackBar(
        'Lead created successfully',
        'success'
      );
      if (createdLead?.id) {
        this.router.navigate(['/lead-management/edit', createdLead.id]);
      } else {
        this.router.navigate(['/lead-management/view']);
      }
    } catch (error: any) {
      this.notificationService.showSnackBar('Failed to create Lead', 'failure');
    }
  }

  // NAV BAR HANDLERS

  async onSaveInformation(): Promise<void> {
    await this.onSubmit();
  }

  async onSaveAndClose(): Promise<void> {
    await this.onSubmit(true);
  }

  onClose(): void {
    if (this.hasUnsavedChanges()) {
      this.showCloseConfirmation();
    } else {
      this.navigateToView();
    }
  }

  onDeleteClick(): void {
    this.confirmationDialogService
      .openConfirmation({
        title: 'Delete Lead',
        message: 'Are you sure you want to <strong>delete</strong> this lead?',
      })
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.leadId()) {
          this.deleteLead();
        }
      });
  }

  private deleteLead(): void {
    this.leadService.bulkDeleteLeads([this.leadId()!]).subscribe({
      next: (): void => {
        this.notificationService.showSnackBar(
          'Lead deleted successfully',
          'success'
        );
        this.navigateToView();
      },
      error: (): void => {
        this.notificationService.showSnackBar(
          'Failed to delete lead.',
          'failure'
        );
      },
    });
  }

  onToggleActiveClicked(): void {
    const currentlyActive = this.isActive();
    const newActive = !currentlyActive;
    const action = newActive ? 'activate' : 'deactivate';
    this.confirmationDialogService
      .openConfirmation({
        title: `${newActive ? 'Activate' : 'Deactivate'} Lead`,
        message: `Are you sure you want to <strong>${action}</strong> this lead?`,
      })
      .subscribe((confirmed: boolean) => {
        if (confirmed && this.leadId()) {
          this.toggleLeadStatus(newActive, action);
        }
      });
  }
  private toggleLeadStatus(active: boolean, action: string): void {
    this.leadService.bulkToggleActive([this.leadId()!], active).subscribe({
      next: () => {
        this.isActive.set(active);

        if (this.userForm.contains('isActive')) {
          this.userForm.get('isActive')?.setValue(active);
        }
        this.userForm.markAsPristine();
        this.userForm.markAsUntouched();
        this.markNestedControlsAsPristine(this.userForm);
        this.notificationService.showSnackBar(
          `Lead ${active ? 'activated' : 'deactivated'} successfully`,
          'success'
        );
      },
      error: () => {
        this.notificationService.showSnackBar(
          `Failed to ${action} lead.`,
          'failure'
        );
      },
    });
  }

  onClickAddLead(): void {
    if (this.hasUnsavedChanges()) {
      this.showNewLeadConfirmation();
    } else {
      this.resetFormForNewLead();
    }
  }

  private showNewLeadConfirmation(): void {
    this.confirmationDialogService
      .openConfirmation({
        title: 'Discard Changes',
        message:
          'You have unsaved changes. Are you sure you want to start a new lead?',
      })
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.resetFormForNewLead();
        }
      });
  }

  private resetFormForNewLead(): void {
    this.router.navigate(['/lead-management/add']);
    this.userForm.reset();
    this.leadId.set(null);
    this.isActive.set(false);
    this.isFormSaved = false;
    this.userForm.patchValue({
      addressDto: {
        addressLine1: null,
        city: null,
        stateOrProvince: null,
        postalCode: null,
        country: null,
      },
    });
    this.userForm.markAsPristine();
    this.userForm.markAsUntouched();
    this.markNestedControlsAsPristine(this.userForm);
  }

  private hasUnsavedChanges(): boolean {
    if (this.isFormSaved) return false;
    return (
      this.userForm.dirty ||
      this.userForm.touched ||
      this.hasNestedFormChanges(this.userForm)
    );
  }

  private hasNestedFormChanges(formGroup: FormGroup): boolean {
    return Object.keys(formGroup.controls).some((key) => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        return (
          control.dirty || control.touched || this.hasNestedFormChanges(control)
        );
      }
      return control ? control.dirty || control.touched : false;
    });
  }

  private showCloseConfirmation(): void {
    this.confirmationDialogService
      .openConfirmation({
        title: 'Discard Changes',
        message:
          'You have unsaved changes. Are you sure you want to close without saving?',
      })
      .subscribe((confirmed: boolean) => {
        if (confirmed) {
          this.navigateToView();
        }
      });
  }

  private navigateToView(): void {
    this.router.navigate(['/lead-management/view']);
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

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  onStateInput(event: Event) {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/[^A-Za-z]/g, ''); // keep only letters
    this.userForm
      .get('addressDto.stateId')
      ?.setValue(input.value, { emitEvent: false });
  }

  salesPersonDisplayFn = (id: any): string => {
    if (!id) return '';
    const person = this.salesPersonList.find((sp) => sp.id === id);
    return person ? person.value : '';
  };

  get addressForm(): FormGroup {
    return this.userForm.get('addressDto') as FormGroup;
  }

  formatFieldName(fieldName:string) {
    let cleaned = fieldName.replace(/Id$/i, '');
    const formatted = cleaned.replace(/([A-Z])/g, ' $1').trim();
    return formatted.split(' ').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
  }
}
