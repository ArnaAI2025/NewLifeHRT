import { Component, Input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatRadioModule } from '@angular/material/radio';

@Component({
  selector: 'app-mat-radio-group',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatRadioModule],
  templateUrl: './radio.component.html',
  styleUrl: './radio.component.scss'

})
export class RadioGroupComponent {
  @Input() label: string = '';
  @Input() control!: FormControl;
  @Input() options: { label: string; value: any }[] = [];
}
