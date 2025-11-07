import { CommonModule } from '@angular/common';
import { Component, forwardRef, OnInit } from '@angular/core';
import { FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-color-picker',
  imports: [FormsModule, CommonModule],
  templateUrl: './color-picker.html',
  styleUrl: './color-picker.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ColorPickerComponent),
      multi: true
    }
  ]
})
export class ColorPickerComponent implements OnInit {
  value: string = '#d53434';
  disabled = false;

  private onChange: any = () => {};
  private onTouched: any = () => {};

  ngOnInit(): void {}

  writeValue(value: any): void {
    this.value = value || '#d53434';
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onColorInputChange(event: any): void {
    const newValue = event.target.value;
    this.value = newValue;
    this.onChange(newValue);
    this.onTouched();
  }

  onHexInputChange(event: any): void {
    let newValue = event.target.value;

    // Add # if missing
    if (newValue && !newValue.startsWith('#')) {
      newValue = '#' + newValue;
    }

    // Validate hex format
    if (/^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/i.test(newValue)) {
      this.value = newValue;
      this.onChange(newValue);
    }

    this.onTouched();
  }

}
