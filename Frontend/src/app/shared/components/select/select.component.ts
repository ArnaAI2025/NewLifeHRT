import { Component, Input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule, MatFormFieldAppearance } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-mat-dropdown',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './select.component.html',
})
export class DropdownComponent {
  @Input() label: string = '';
  @Input() control!: FormControl;
  @Input() options: { label: string; value: any }[] = [];
  @Input() placeholder: string = '';
  @Input() appearance: MatFormFieldAppearance = 'outline';
  @Input() multiple: boolean = false;
}
