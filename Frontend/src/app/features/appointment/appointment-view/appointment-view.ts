import { Component } from '@angular/core';
import { CommonModule } from "@angular/common";
import { CalendarDateFormatter, CalendarEvent, CalendarView, DateAdapter } from 'angular-calendar';
import { CalendarModule } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';

import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
  AbstractControl
} from "@angular/forms";
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
import { AppointmentCalendarComponent, DoctorOption } from '../appointment-calendar/appointment-calendar';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-appointment-view',
  imports: [
    CalendarModule, CommonModule,
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
    AppointmentCalendarComponent,
    MatDialogModule,
    MatCheckboxModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule
  ],
  templateUrl: './appointment-view.html',
  styleUrl: './appointment-view.scss'
})
export class AppointmentViewComponent {
  view: CalendarView = CalendarView.Day;
  CalendarView = CalendarView;  // to use in template
  viewDate: Date = new Date();

  allDoctors: DoctorOption[] = [];
  doctorSearch = '';
  selectedDoctorIds: (string | number)[] = [];
  filterActive = false;

  constructor(
    private router: Router
  ) { }

  gotoAddAppointment() {
    this.router.navigate(['/appointment/add']);
  }
  onDoctorsChange(list: DoctorOption[]) {
  this.allDoctors = list || [];

  if (!this.filterActive) {
    this.selectedDoctorIds = this.allDoctors.map(d => d.id);
  } else {
    this.selectedDoctorIds = this.selectedDoctorIds.filter(id =>
      this.allDoctors.some(d => d.id === id)
    );
  }
}

  // Helpers
  get filteredDoctors(): DoctorOption[] {
    const q = (this.doctorSearch || '').toLowerCase();
    return this.allDoctors.filter(d => d.name.toLowerCase().includes(q));
  }

  isChecked(id: string | number): boolean {
    return this.selectedDoctorIds.includes(id);
  }

  toggleOne(id: string | number) {
    const i = this.selectedDoctorIds.indexOf(id);
    if (i >= 0) {
      this.selectedDoctorIds.splice(i, 1);
    } else {
      this.selectedDoctorIds.push(id);
    }
    this.filterActive = true;
    this.selectedDoctorIds = [...this.selectedDoctorIds];
  }

  selectAllVisible() {
    this.filterActive = true;
    const ids = this.filteredDoctors.map(d => d.id);
    this.selectedDoctorIds = [...new Set([...this.selectedDoctorIds, ...ids])];
  }

  clearAll() {
    this.filterActive = true;
    this.selectedDoctorIds = [];
  }

  intment() {
    this.router.navigate(['/appointment/add']);
  }

}
