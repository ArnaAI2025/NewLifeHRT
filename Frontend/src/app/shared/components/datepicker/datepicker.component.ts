import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule, MatFormFieldAppearance } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-mat-datepicker',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './datepicker.component.html',
})
export class DatePickerComponent {
  @Input() label: string = 'Select Date';
  @Input() placeholder: string = 'dd/mm/yyyy';
  @Input() control: FormControl = new FormControl(null);
  @Input() appearance: MatFormFieldAppearance = 'outline';

  @Input() minDate: Date | null = null;
  @Input() maxDate: Date | null = null;
  @Input() startAt: Date | null = null;
  @Input() startView: 'month' | 'year' | 'multi-year' = 'month';
  @Input() touchUi: boolean = false;
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() opened: boolean = false;

  @Input() filterFn: (d: Date | null) => boolean = () => true;
}
