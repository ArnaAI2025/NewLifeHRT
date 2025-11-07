import { Component, OnInit, signal } from '@angular/core';
import { MatButtonModule } from "@angular/material/button";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormField, MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatMenuModule } from "@angular/material/menu";
import { MatSelectModule } from "@angular/material/select";
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { AppointmentService } from '../appointment.services';
import { AppointmentServiceResponse } from '../model/appointment-service-response.model';
import { forkJoin } from 'rxjs';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserDto } from '../../user-management/model/user-dto.model';
import { AppointmentModeResponse } from '../model/appointment-modes.model';
import { PatientService } from '../../patient/patient.services';
import { DropDownResponseDto } from '../../../shared/models/drop-down-response.model';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { SlotResponse } from '../model/appointment-slots-response.model';
import { CreateAppointmentRequestDto } from '../model/create-appointment-request.model';
import { NotificationService } from '../../../shared/services/notification.service';
import { AppointmentGetResponseDto } from '../model/appointment-get-response.model';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-appointment-add',
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatProgressSpinnerModule,
    MatButtonToggleModule,
    ReactiveFormsModule,
    CommonModule,
    MatCardModule,
    FullPageLoaderComponent,
    MatTooltipModule
  ],
  templateUrl: './appointment-add.html',
  styleUrl: './appointment-add.scss'
})
export class AppointmentAddComponent implements OnInit {

  appointmentId: string | null = '';
  isEditMode = signal(false);
  clinicAppointmentServices = signal<AppointmentServiceResponse[]>([]);
  appointmentForm!: FormGroup;
  availableProviders = signal<UserDto[]>([]);
  appointmentModes = signal<AppointmentModeResponse[]>([]);
  patients = signal<DropDownResponseDto[]>([]);
  appointmentSlots = signal<SlotResponse[]>([]);
  isLoading = signal(false);
  isLoadingSlots = signal(false);
  today = new Date();
  selectedServiceId = signal<string | null>(null);

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private appointmentService: AppointmentService,
    private fb: FormBuilder,
    private patientService: PatientService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.initializeForm();
    this.appointmentId = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!this.appointmentId);

    forkJoin({
      appointments: this.appointmentService.getAllClinicAppointmentServices(),
      appointmentModes: this.appointmentService.getAllAppointmentModes(),
      patients: this.patientService.getAllActivePatients()
    }).subscribe({
      next: (res) => {
        this.clinicAppointmentServices.set(res.appointments);
        this.appointmentModes.set(res.appointmentModes);
        this.patients.set(res.patients);
        if (this.isEditMode()) {
          this.loadAppointmentForEdit(this.appointmentId!);
          this.appointmentForm.get('patientId')?.disable();
        }
      },
      error: (err) => {
        console.error('Error fetching services', err);
      }
    });

    this.appointmentForm.get('clinicService')?.valueChanges.subscribe(serviceId => {
      const selectedService = this.clinicAppointmentServices().find(s => s.id === serviceId);
      this.availableProviders.set(selectedService?.users ?? []);
      if (!this.isEditMode()) {
        this.appointmentForm.patchValue({ doctorId: null });
      }
      this.resetAppointmentDateAndSlots();
    });

    this.appointmentForm.get('appointmentDate')?.valueChanges.subscribe(date => {
      if (date) {
        this.appointmentSlots.set([]);
        this.fetchAppointments(date);
      }
    });

    this.appointmentForm.get('doctorId')?.valueChanges.subscribe(() => {
      this.resetAppointmentDateAndSlots();
    });

  }

  initializeForm() {
    this.appointmentForm = this.fb.group({
      clinicService: [null, [Validators.required]],
      doctorId: [null, [Validators.required]],
      appointmentDate: [null],
      patientId: [null, [Validators.required]],
      mode: [null, [Validators.required]],
      description: [''],
      slotId: [null, [Validators.required]]
    });
  }

  gotoAppointmentView() {
    this.router.navigate(['/appointment/view']);
  }

  fetchAppointments(date: Date) {
    const doctorId = this.appointmentForm.get('doctorId')?.value;
    const provider = this.availableProviders().find(x => x.userId === doctorId);
    if (!provider) return;

    this.isLoadingSlots.set(true);

    this.appointmentService.getAppointmentSlots(
      provider.userServiceLinkId,
      provider.userId,
      date
    ).subscribe({
      next: (slots) => {
        // Sort slots by startTime
        slots.sort((a, b) => this.parseTime(a.startTime) - this.parseTime(b.startTime));
        this.appointmentSlots.set(slots);
        this.isLoadingSlots.set(false);
      },
      error: (err) => {
        console.error("Error fetching slots:", err);
        this.isLoadingSlots.set(false);
      }
    });
  }

  parseTime(time: string): number {
    const [t, modifier] = time.split(" ");
    let [hours, minutes] = t.split(":").map(Number);
    if (modifier === "PM" && hours < 12) {
      hours += 12;
    }
    if (modifier === "AM" && hours === 12) {
      hours = 0;
    }
    return hours * 60 + minutes; // return total minutes from midnight
  }

  onConfirmAppointment(): void {
    if (this.appointmentForm.invalid) {
      this.appointmentForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const formValues = this.appointmentForm.value;
    const patientId = this.isEditMode() ? this.appointmentForm.get('patientId')?.value : formValues.patientId;
    const request: CreateAppointmentRequestDto = {
      slotId: formValues.slotId,
      appointmentDate: this.formatDate(formValues.appointmentDate)!,
      patientId: patientId,
      doctorId: formValues.doctorId,
      modeId: formValues.mode,
      description: formValues.description
    };

    const apiCall = this.isEditMode()
      ? this.appointmentService.updateAppointment(this.appointmentId!, request)
      : this.appointmentService.createAppointment(request);

    apiCall.subscribe({
      next: (res) => {
        this.notificationService.showSnackBar(res.message, 'success');
        this.isLoading.set(false);
        this.gotoAppointmentView();
      },
      error: (err) => {
        this.notificationService.showSnackBar(err?.error || 'Something went wrong', 'failure');
        this.isLoading.set(false);
      }
    });
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  private loadAppointmentForEdit(id: string): void {
    this.isLoading.set(true);
    this.appointmentService.getAppointmentById(id).subscribe({
      next: (appointment) => {
        this.patchAppointmentForm(appointment);
        this.isLoading.set(false);
      },
      error: (err) => console.error("Failed to load appointment:", err)
    });
  }

  private patchAppointmentForm(appointment: AppointmentGetResponseDto) {
    const selectedService = this.clinicAppointmentServices().find(s => s.id === appointment.serviceId);
    this.availableProviders.set(selectedService?.users ?? []);

    this.selectedServiceId.set(appointment.serviceId);

    const appointmentDate = new Date(appointment.appointmentDate);

    this.appointmentForm.patchValue({
      clinicService: appointment.serviceId,
      doctorId: appointment.doctorId,
      slotId: appointment.slotId,
      appointmentDate: appointmentDate,
      patientId: appointment.patientId,
      mode: appointment.modeId,
      description: appointment.description
    });
    if (this.isEditMode()) {
      this.appointmentForm.get('clinicService')?.disable();
      this.appointmentForm.get('appointmentDate')?.setValue(appointmentDate);
    }
    this.fetchAppointments(new Date(appointment.appointmentDate));
  }

  resetAppointmentDateAndSlots() {
    this.appointmentForm.patchValue({
      appointmentDate: null,
      slotId: null
    });
    this.appointmentSlots.set([]);
  }

  onDateSelectionChange(e: any) {
    console.log("event", e)
  }

  isPastSlot(slot: any): boolean {
    const selectedDate: Date = this.appointmentForm.get('appointmentDate')?.value;
    const now = new Date();

    if (!selectedDate) return false;

    const isToday =
      selectedDate.getFullYear() === now.getFullYear() &&
      selectedDate.getMonth() === now.getMonth() &&
      selectedDate.getDate() === now.getDate();

    if (!isToday) {
      return false;
    }

    const [time, modifier] = slot.startTime.split(" ");
    let [hours, minutes] = time.split(":").map(Number);

    if (modifier === "PM" && hours < 12) hours += 12;
    if (modifier === "AM" && hours === 12) hours = 0;

    const slotDate = new Date(selectedDate);
    slotDate.setHours(hours, minutes, 0, 0);

    return slotDate < now;
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;
    const day = date.getDay();
    return day !== 0 && day !== 6;
  };
}
