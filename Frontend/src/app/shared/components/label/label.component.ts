import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-label',
  standalone: true,
  imports: [CommonModule, MatTooltipModule],
  templateUrl: './label.component.html',
  styleUrl: './label.component.scss'})
export class LabelComponent {
  @Input() text: string = '';
  @Input() required: boolean = false;
  @Input() tooltip?: string;
  @Input() tag: 'label' | 'div' | 'span' = 'label';
  @Input() forId?: string;
  @Input() class: string = 'block text-sm font-medium text-gray-700 mb-1';
}
