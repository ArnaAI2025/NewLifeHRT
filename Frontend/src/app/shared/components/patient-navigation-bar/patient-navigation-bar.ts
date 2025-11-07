import { Component, EventEmitter, inject, Input, Output, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import {Router} from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AppSettingsService } from '../../services/app-settings.service';
@Component({
  selector: 'app-patient-navigation-bar',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatButtonToggleModule,
    FormsModule
  ],
  templateUrl: './patient-navigation-bar.html',
  styleUrls: ['./patient-navigation-bar.scss']
})
export class PatientNavigationBarComponent {
  @Input() isEditMode = false;
  @Input() isSaving = false;
  @Input() isActive: boolean | undefined ;
  @Input() patientId: string | null = null;
  @Input() activeView: 'form' | 'medical-recommandation' | 'appointments' |'shipping-address' | 'counselor' | 'proposals' | 'orders' | 'sms' | 'reminders' = 'form';

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();
  @Output() addPatient = new EventEmitter<void>();
  @Output() saveAndClose = new EventEmitter<void>();
  @Output() toggleActiveStatus = new EventEmitter<boolean>();
  @Output() delete = new EventEmitter<void>();
  private readonly router = inject(Router);
  private appSettingService = inject(AppSettingsService);


  onClose(): void {
    this.close.emit();
  }

  onSubmit(): void {
    this.save.emit();
  }

  onClickAddPatient(): void {
    this.addPatient.emit();
  }

  onSaveAndCloseClick(): void {
    this.saveAndClose.emit();
  }

  onToggleActive(status: boolean): void {
    this.toggleActiveStatus.emit(status);
  }
onViewToggle(event: MatButtonToggleChange): void {
  const activeView = event.value as 'form' | 'medical-recommandation' | 'sms' | 'counselor' | 'appointments' | 'shipping-address' | 'proposals' | 'orders' | 'reminders';

  if (activeView === 'form') {
    if (this.isEditMode && this.patientId) {
      this.router.navigateByUrl(`/patient/edit/${this.patientId}`);
    }
  } else if (this.isEditMode && activeView === 'counselor') {
    this.router.navigateByUrl(`/counselor-view/${this.patientId}`);
  }
  else if (this.isEditMode && activeView === 'medical-recommandation') {
    this.router.navigateByUrl(`/medication-recommendation-view/${this.patientId}`);
  }
  else if (this.isEditMode && activeView === 'shipping-address') {
    this.router.navigateByUrl(`/shipping-address/view/${this.patientId}`);
  }
  else if (this.isEditMode && activeView === 'proposals') {
    this.router.navigateByUrl(`/proposals/view/${this.patientId}`);
  }
  else if (this.isEditMode && activeView === 'orders') {
    this.router.navigateByUrl(`/orders/view/${this.patientId}`);
  }
    else if (this.isEditMode && activeView === 'sms') {
    this.router.navigateByUrl(`/patient/${this.patientId}/sms`);
  }
    else if (this.isEditMode && activeView === 'appointments') {
    this.router.navigateByUrl(`/appointment/view/${this.patientId}`);
  }
  else if (this.isEditMode && activeView === 'reminders') {
    this.router.navigateByUrl(`/patient/${this.patientId}/reminder`);
  }
}

get isSavingValue(): boolean {
  return typeof this.isSaving === 'function' ? this.isSaving : this.isSaving;
}

onDeletePatient(){
  this.delete.emit();
}

isPatientLoggedIn(){
  return this.appSettingService.isUserPatient();
}

}
