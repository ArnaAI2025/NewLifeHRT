import {
  Component,
  OnInit,
  OnChanges,
  SimpleChanges,
  Input,
  Output,
  EventEmitter,
  inject,
  signal
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  FormControl,
  ReactiveFormsModule
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { PatientService } from '../patient.services';
import { ShippingAddressRequestDto } from '../model/shipping-address.model';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-shipping-address-add',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatOptionModule,
    MatAutocompleteModule
  ],
  templateUrl: './shipping-address-add.html',
  styleUrls: ['./shipping-address-add.scss']
})
export class ShippingAddressAddComponent implements OnInit, OnChanges {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly notificationService = inject(NotificationService);
  private readonly confirmationDialogService = inject(ConfirmationDialogService);

  // Inputs
  @Input() patientId!: string;
  @Input() shippingAddressId?: string;
  @Input() isVisible = false;

  // Outputs
  @Output() addressCreated = new EventEmitter<any>();
  @Output() addressUpdated = new EventEmitter<any>();
  @Output() modalClosed = new EventEmitter<void>();

  form!: FormGroup;
  isFormSaved = false;
  isSubmitting = signal(false);
  isLoading = signal(false);
  isLoadingCountries = false;
  isLoadingStates = signal(false);

  countriesList: DropDownResponseDto[] = [];
  filteredcountriesList: DropDownResponseDto[] = [];
  allStates: DropDownResponseDto[] = [];

  ngOnInit(): void {
    this.initForm();
    this.loadCountries();

    // Load existing address if edit mode
    if (this.shippingAddressId) {
      this.loadShippingAddress();
    }
    this.form.get('countryId')?.valueChanges.subscribe(countryId => {
      if (countryId && typeof countryId === 'number') {
        this.loadStatesByCountryId(Number(countryId));
      } else {
        this.allStates = [];
        this.form.get('stateId')?.reset();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['shippingAddressId']) {
      const currentId = changes['shippingAddressId'].currentValue;
      const previousId = changes['shippingAddressId'].previousValue;

      if (!changes['shippingAddressId'].firstChange && currentId !== previousId) {
        currentId ? this.loadShippingAddress() : this.resetForm();
      }
    }

    if (changes['isVisible']) {
      const isCurrentlyVisible = changes['isVisible'].currentValue;
      const wasPreviouslyVisible = changes['isVisible'].previousValue;

      if (isCurrentlyVisible && !wasPreviouslyVisible) {
        this.shippingAddressId ? this.loadShippingAddress() : this.resetForm();
      }
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      addressType: [null, Validators.required],
      addressLine1: [null, Validators.required],
      city: [null, Validators.required],
      postalCode: [null, [Validators.required, Validators.maxLength(6)]],
      countryId: [null, Validators.required],
      stateId: [null, Validators.required]
    });
  }

  private loadCountries(): void {
    this.isLoadingCountries = true;
    this.patientService.getAllActiveCountries().subscribe({
      next: (countries: DropDownResponseDto[]) => {
        this.countriesList = countries;
        this.filteredcountriesList = countries;
      },
      error: () => console.error('Failed to load countries.'),
      complete: () => (this.isLoadingCountries = false)
    });
  }

  private loadStatesByCountryId(countryId: number, stateIdToMatch?: number | string): void {
    this.isLoadingStates.set(true);

    this.patientService.getAllActiveStates(countryId).subscribe({
      next: (states: DropDownResponseDto[]) => {
        this.allStates = states;

        // Preselect state if editing
        if (stateIdToMatch) {
          const matchedState = this.allStates.find(s => s.id == stateIdToMatch);
          if (matchedState) {
            this.form.get('stateId')?.setValue(matchedState.id, { emitEvent: false });
          }
        }
      },
      error: () => {
        this.notificationService.showSnackBar('Failed to load states.', 'failure');
        this.allStates = [];
      },
      complete: () => this.isLoadingStates.set(false)
    });
  }

  displayCountryName = (id: number | string | null): string => {
    if (!id) return '';
    const found = this.countriesList.find(c => c.id === id);
    return found ? found.value : '';
  };

  onCountryInput(value: string): void {
    const filterValue = value.toLowerCase();
    this.filteredcountriesList = this.countriesList.filter(c =>
      c.value.toLowerCase().includes(filterValue)
    );
  }

  onCountrySelected(selectedId: number | string): void {
    this.form.get('countryId')?.setValue(selectedId);
    this.form.get('stateId')?.reset();
    this.allStates = [];

    if (selectedId) {
      this.loadStatesByCountryId(Number(selectedId));
    }
  }

  async loadShippingAddress(): Promise<void> {
    if (!this.shippingAddressId) return;

    this.isLoading.set(true);
    try {
      const address = await firstValueFrom(this.patientService.getByShippingId(this.shippingAddressId));

      if (address) {
        this.form.patchValue({
          addressLine1: address.addressLine1,
          addressType: address.addressType,
          city: address.city,
          //stateOrProvince: address.stateOrProvince,
          postalCode: address.postalCode,
          countryId: address.countryId
        });

        if (address.countryId) {
          this.loadStatesByCountryId(address.countryId, address.stateId);
        }
      }
    } catch (error) {
      console.error('Error loading shipping address:', error);
      this.notificationService.showSnackBar('Failed to load address', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  resetForm(): void {
    this.form.reset();
    this.isFormSaved = false;
    this.form.markAsPristine();
    this.form.markAsUntouched();
  }

  getModalTitle(): string {
    return this.shippingAddressId ? 'Edit Address' : 'Add Address';
  }

  isFieldInvalid(name: string): boolean {
    const control = this.form.get(name);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getFieldError(name: string): string {
    const control = this.form.get(name);
    if (control?.hasError('required')) return 'This field is required';
    if (control?.hasError('maxlength')) return 'Too many characters';
    return '';
  }

  get countryControl(): FormControl {
    return this.form.get('countryId') as FormControl;
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notificationService.showSnackBar('Please fill all the required fields!', 'normal');
      return;
    }

    this.isSubmitting.set(true);
    const dto: ShippingAddressRequestDto = {
      id: this.shippingAddressId ?? null,
      patientId: this.patientId,
      address: this.form.value
    };

    const isEdit = !!this.shippingAddressId;

    try {
      if (isEdit) {
        const response = await firstValueFrom(this.patientService.updateShippingAddress(dto));
        this.notificationService.showSnackBar('Shipping address updated successfully', 'success');
        this.isFormSaved = true;
        if (response.id) {
          this.addressUpdated.emit(response);
          this.onClose();
        }
      } else {
        const response = await firstValueFrom(this.patientService.createShippingAddress(dto));
        this.notificationService.showSnackBar('Shipping address created successfully', 'success');
        this.isFormSaved = true;
        if (response.id) {
          this.addressCreated.emit(response);
          this.onClose();
        }
      }
    } catch {
      this.notificationService.showSnackBar(
        isEdit ? 'Failed to update shipping address' : 'Failed to create shipping address',
        'failure'
      );
    } finally {
      this.isSubmitting.set(false);
    }
  }

  allowOnlyNumbers(event: KeyboardEvent): void {
    const charCode = event.key.charCodeAt(0);
    if (charCode < 48 || charCode > 57) event.preventDefault();
  }

  onClose(): void {
    if (this.hasUnsavedChanges()) {
      this.confirmationDialogService
        .openConfirmation({
          title: 'Discard changes?',
          message: 'You have unsaved changes. Do you want to leave without saving?'
        })
        .subscribe(confirmed => {
          if (confirmed) {
            this.resetForm();
            this.modalClosed.emit();
          }
        });
    } else {
      this.resetForm();
      this.modalClosed.emit();
    }
  }

  hasUnsavedChanges(): boolean {
    if (this.isFormSaved) return false;
    return this.form.dirty || this.form.touched;
  }
}
