import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CheckboxComponent } from '../shared/components/checkbox/checkbox.component';
import { DatePickerComponent } from '../shared/components/datepicker/datepicker.component';
import { InputComponent } from '../shared/components/input-control/input-control.component';
import { LabelComponent } from '../shared/components/label/label.component';
import { RadioGroupComponent } from '../shared/components/radio/radio.component';
import { DropdownComponent } from '../shared/components/select/select.component';
import { TextAreaComponent } from '../shared/components/textarea/textarea.component';

@Component({
  selector: 'app-check',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CheckboxComponent,
    DatePickerComponent,
    InputComponent,
    LabelComponent,
    RadioGroupComponent,DropdownComponent,TextAreaComponent
  ],
  templateUrl: './check.html',
  styleUrl: './check.scss'
})
export class Check {
  termsControl = new FormControl(false);
   dateControl = new FormControl(null);
   nameControl = new FormControl('');
   namerControl = new FormControl('');
     genderControl = new FormControl('male');
   cityControl = new FormControl('');
descriptionControl = new FormControl('');
  cityOptions = [
    { label: 'New York', value: 'NY' },
    { label: 'Los Angeles', value: 'LA' },
    { label: 'Chicago', value: 'CHI' },
    { label: 'Houston', value: 'HOU' }
  ];
  genderOptions = [
    { label: 'Male', value: 'male' },
    { label: 'Female', value: 'female' },
    { label: 'Other', value: 'other' },
  ];
  // Optional config
  minDate = new Date(2023, 0, 1);  // Jan 1, 2023
  maxDate = new Date(2025, 11, 31); // Dec 31, 2025
}
