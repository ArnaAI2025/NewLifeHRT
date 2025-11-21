import {
  ApplicationRef,
  Component,
  ElementRef,
  Inject,
  OnInit,
  QueryList,
  ViewChildren,
  inject,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  FormControlName,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { DoctorManagementAndMedicalRecommandationService } from '../doctor-management-and-medical-recommandation.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { CommonModule } from '@angular/common';
import { MedicalRecommendationRequestDto } from '../model/medical-recommadation-request.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { PatientResponseDto } from '../../patient/model/patient-response.model';
import { ConfirmationDialogService } from "../../../shared/components/confirmation-dialog/confirmation-dialog.services";
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { CommonOperationResponseDto } from '../../../shared/models/common-operation-response.model';


@Component({
  selector: 'app-medical-recommendation-add',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatSnackBarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    PatientNavigationBarComponent,
  ],
  templateUrl: './medical-recommendation-add.html',
  styleUrls: ['./medical-recommendation-add.scss'],
})
export class MedicalRecommendationAddComponent implements OnInit {
  medicalRecommendationForm!: FormGroup;
  patientDataToLoad: PatientResponseDto | null = null;
  medicationTypeList = signal<DropDownResponseDto[]>([]);
  followUpLabTestList: DropDownResponseDto[] = [];
  isOtherMedicationTypeSelected = false;
  isLoadingMedicationTypeList = true;
  isLoadingFollowUpLabTestList = true;

  patientId!: string;
  doctorId?: number | null;
  medicalRecommendationId?: string;
  isLoading: boolean = false;
  @ViewChildren(FormControlName, { read: ElementRef })
  formControls!: QueryList<ElementRef>;

  constructor(
    private doctorManagementAndMedicalRecommandationService: DoctorManagementAndMedicalRecommandationService,
    private formBuilder: FormBuilder,
    private router: Router,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private confirmationService: ConfirmationDialogService,
    private appRef: ApplicationRef
  ) {}

  ngOnInit(): void {
    this.initializeForm();

    // Extract route params
    this.patientId = this.route.snapshot.paramMap.get('patientId')!;
    this.medicalRecommendationId =
      this.route.snapshot.paramMap.get('medicalRecommendationId') ?? undefined;
    this.getPatientById();
    // Fetch dropdown data
    this.loadDropdowns();

    // If editing, load existing recommendation
    if (this.medicalRecommendationId) {
      this.loadMedicalRecommendation(this.medicalRecommendationId);
    }
  }
  getPatientById(): void {
    if (!this.patientId) {
      this.notificationService.showSnackBar('Invalid patient ID.', 'failure');
      return;
    }

    this.isLoading = true;

    this.doctorManagementAndMedicalRecommandationService
      .getPatientById(this.patientId)
      .subscribe({
        next: (response: PatientResponseDto) => {
          this.patientDataToLoad = response;
          this.doctorId = this.patientDataToLoad?.assignPhysicianId;

          if (!this.doctorId) {
            this.confirmationService
              .openConfirmation({
                title: 'Doctor Not Assigned',
                message: 'Please assign a doctor before proceeding.',
                confirmButtonText: 'OK',
                showCancelButton: false,
              })
              .subscribe(() => {
                this.isLoading = false;
                this.router.navigate(['patient/edit', this.patientId]);
              });
          }
        },
        error: (error) => {
          console.error('Error fetching patient:', error);
          const message =
            error?.error?.message ||
            error?.error ||
            'Error fetching patient details.';
          this.notificationService.showSnackBar(message, 'failure');
          this.isLoading = false;
        },
      });
  }

  loadDropdowns() {
    this.doctorManagementAndMedicalRecommandationService
      .getAllMedicationType()
      .subscribe({
        next: (meds) => {
          this.medicationTypeList.set(meds);
          this.isLoadingMedicationTypeList = false;
        },
        error: () => {
          this.isLoadingMedicationTypeList = false;
        },
      });

    this.doctorManagementAndMedicalRecommandationService
      .getAllFollowUpTests()
      .subscribe({
        next: (tests) => {
          this.followUpLabTestList = tests;
          this.isLoadingFollowUpLabTestList = false;
        },
        error: () => {
          this.isLoadingFollowUpLabTestList = false;
        },
      });
  }
  goBackToRecommendationView(): void {
    const form = this.medicalRecommendationForm;
    // If form isn't dirty or touched: direct navigation
    if (!form.dirty && !form.touched) {
      this.router.navigate(['/medication-recommendation-view', this.patientId]);
      return;
    }

    this.confirmationService
      .openConfirmation({
        title: 'Discard Changes?',
        message:
          'You have unsaved changes. Are you sure you want to leave this form?',
      })
      .subscribe((confirmed) => {
        if (confirmed) {
          this.router.navigate([
            '/medication-recommendation-view',
            this.patientId,
          ]);
        }
      });
  }

  loadMedicalRecommendation(id: string) {
    this.doctorManagementAndMedicalRecommandationService
      .getMedicalRecommendationById(id)
      .subscribe({
        next: (data) => {
          this.medicalRecommendationForm.patchValue({
            consultationDate: data.consultationDate,
            medicationTypeId: data.medicationTypeId,
            otherMedicationType:
              data.medicationTypeId === 8 ? data.otherMedicationType : '',
            title: data.title,
            pMHx: data.pMHx,
            pSHx: data.pSHx,
            fHx: data.fHx,
            suppliments: data.suppliments,
            medication: data.medication,
            socialHistory: data.socialHistory,
            allergies: data.allergies,
            hrt: data.hrt,
            followUpLabTestId: data.followUpLabTestId,
            subjective: data.subjective,
            objective: data.objective,
            assessment: data.assessment,
            plan: data.plan,
            socialPoint: data.socialPoint,
            notes: data.notes,
          });
          this.onMedicationTypeChange(data.medicationTypeId);
        },
      });
  }

  initializeForm(): void {
    this.medicalRecommendationForm = this.formBuilder.group({
      consultationDate: [null, Validators.required],
      medicationTypeId: [null, Validators.required],
      otherMedicationType: [{ value: '', disabled: true }],
      title: [null],
      pMHx: [null],
      pSHx: [null],
      fHx: [null],
      suppliments: [null],
      medication: [null],
      socialHistory: [null],
      allergies: [null],
      hrt: [null],
      followUpLabTestId: [null],
      subjective: [null],
      objective: [null],
      assessment: [null],
      plan: [null],
      socialPoint: [null],
      notes: [null],
    });
  }
  onMedicationTypeChange(selectedId: number): void {
    const selected = this.medicationTypeList().find(
      (mt) => mt.id === selectedId
    );
    this.isOtherMedicationTypeSelected =
      selected?.value?.toLowerCase() === 'other';

    const otherCtrl = this.medicalRecommendationForm.get('otherMedicationType');
    if (this.isOtherMedicationTypeSelected) {
      otherCtrl?.enable();
    } else {
      otherCtrl?.disable();
      otherCtrl?.setValue('');
    }
    otherCtrl?.updateValueAndValidity();
  }
  isFieldInvalid(controlName: string): boolean {
    const control = this.medicalRecommendationForm.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getFieldError(controlName: string): string {
    const control = this.medicalRecommendationForm.get(controlName);
    const formattedName = this.formatFieldName(controlName);
    if (control?.errors?.['required']) {
      return `${formattedName} is required`;
    }
    return '';
  }

  onSubmit(): void {
    if (this.medicalRecommendationForm.valid) {
      const rawFormData = this.medicalRecommendationForm.value;

      const formData: MedicalRecommendationRequestDto = {
        ...rawFormData,
        consultationDate: this.formatDate(rawFormData.consultationDate),
        patientId: this.patientId,
        doctorId: this.doctorId,
        id: this.medicalRecommendationId ?? null,
      };
      const isedit = this.medicalRecommendationId ? true : false;
      this.doctorManagementAndMedicalRecommandationService
        .medicalRecommandation(formData, isedit)
        .subscribe({
          next: (res: CommonOperationResponseDto) => {
            this.medicalRecommendationForm.markAsPristine();
            this.medicalRecommendationForm.markAsUntouched();
            if (!isedit) {
              this.router.navigate([
                '/medication-recommendation-edit',
                this.patientId,
                res.id,
              ]);
            }
            this.notificationService.showSnackBar(
              `Medical recommendation ${
                this.medicalRecommendationId ? 'updated' : 'created'
              } successfully`,
              'success'
            );
          },
          error: () => {
            this.notificationService.showSnackBar(
              `Failed to ${
                this.medicalRecommendationId ? 'update' : 'create'
              } medical recommendation`,
              'failure'
            );
          },
        });
    } else {
      this.medicalRecommendationForm.markAllAsTouched();
      this.appRef.whenStable().then(() => {
        this.scrollToFirstInvalidControl();
      });
    }
  }
  togglePatientActiveStatus(status: boolean): void {}
  onSaveAndClose(): void {}
  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }
  onClose(): void {
    this.router.navigate(['/patients/view']);
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

  formatDate(date: string | Date): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    if (!d || isNaN(d.getTime())) {
      return '';
    }
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatFieldName(fieldName: string) {
    let cleaned = fieldName.replace(/Id$/i, '');
    const formatted = cleaned.replace(/([A-Z])/g, ' $1').trim();
    return formatted
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');
  }
}
