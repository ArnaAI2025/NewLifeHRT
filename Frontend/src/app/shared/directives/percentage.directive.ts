import { Directive, HostListener, Input } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appPercentage]'
})
export class PercentageDirective {
  @Input() minPercentage = 0;
  @Input() maxPercentage = 100;

  constructor(private ngControl: NgControl) {}

  @HostListener('input', ['$event'])
  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    const originalValue = value;

    // Save cursor position
    const cursorPos = input.selectionStart ?? value.length;

    // Allow only digits and one dot
    let clean = value.replace(/[^0-9.]/g, '');
    const dotIndex = clean.indexOf('.');
    if (dotIndex !== -1) {
      const beforeDot = clean.substring(0, dotIndex + 1);
      const afterDot = clean.substring(dotIndex + 1).replace(/\./g, '');
      clean = beforeDot + afterDot;
    }

    // Limit to 2 decimal places
    if (clean.includes('.')) {
      const [intPart, decPart] = clean.split('.');
      clean = intPart + '.' + decPart.slice(0, 2);
    }

    // Clamp value only when valid
    if (!clean.endsWith('.') && clean !== '') {
      const num = parseFloat(clean);
      if (!isNaN(num)) {
        if (num > this.maxPercentage) clean = this.maxPercentage.toString();
        if (num < this.minPercentage) clean = this.minPercentage.toString();
      }
    }

    // Update DOM only if changed
    if (clean !== originalValue) {
      input.value = clean;

      // Restore cursor safely
      const diff = clean.length - originalValue.length;
      const newPos = Math.max(0, Math.min(clean.length, cursorPos + diff));
      input.setSelectionRange(newPos, newPos);
    }

    // Update Angular control silently
    if (this.ngControl?.control) {
      this.ngControl.control.setValue(clean, { emitEvent: false });
    }
  }

  @HostListener('blur', ['$event'])
  onBlur(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    if (!value) return;

    let num = parseFloat(value);
    if (isNaN(num)) num = this.minPercentage;
    if (num > this.maxPercentage) num = this.maxPercentage;
    if (num < this.minPercentage) num = this.minPercentage;

    // Format to 2 decimals
    const formatted = num.toFixed(2);
    input.value = formatted;

    if (this.ngControl?.control) {
      this.ngControl.control.setValue(formatted, { emitEvent: true });
    }
  }
}
