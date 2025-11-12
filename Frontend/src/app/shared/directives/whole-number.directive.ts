import { Directive, ElementRef, HostListener, Input, Inject } from '@angular/core';
import { NotificationService } from '../services/notification.service'; // Adjust path as needed

@Directive({
  selector: '[appWholeNumber]',
  standalone: true,
})
export class WholeNumberDirective {
  @Input() appWholeNumberMin: number = 1; // Default min value
  @Input() appWholeNumberMax: number = 999; // Default max value

  private isUpdating = false; // Flag to prevent recursive updates
  private lastNotificationTime = 0; // Debounce notifications to avoid spam
  private readonly notificationDebounceMs = 2000; // 2-second debounce

  constructor(
    private el: ElementRef<HTMLInputElement>,
    private notificationService: NotificationService
  ) {}

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    const input = event.target as HTMLInputElement;
    const key = event.key;

    // Prevent decimal point, comma, 'e', and negative sign
    if (key === '.' || key === ',' || key === 'e' || key === 'E' || key === '-') {
      event.preventDefault();
      return;
    }

    // Allow control keys (e.g., Backspace, Arrow keys, Tab)
    if (
      event.ctrlKey ||
      event.metaKey ||
      ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'].includes(key)
    ) {
      return;
    }

    // Allow numeric keys, but check if the resulting value would be out of range
    if (/^[0-9]$/.test(key)) {
      const currentValue = input.value;
      const cursorPosition = input.selectionStart ?? 0;
      const newValue = currentValue.slice(0, cursorPosition) + key + currentValue.slice(cursorPosition);
      const numValue = parseInt(newValue, 10);

      if (!isNaN(numValue)) {
        if (numValue < this.appWholeNumberMin) {
          this.showNotification(`Quantity cannot be less than ${this.appWholeNumberMin}`);
          event.preventDefault();
        } else if (numValue > this.appWholeNumberMax) {
          this.showNotification(`Quantity cannot be greater than ${this.appWholeNumberMax}`);
          event.preventDefault();
        }
      }
    } else {
      event.preventDefault();
    }
  }

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    if (this.isUpdating) return; // Prevent recursive calls

    this.isUpdating = true;
    try {
      const input = event.target as HTMLInputElement;
      let value = input.value;

      // Additional validation for pasted input or edge cases
      if (!/^[0-9]*$/.test(value) || value.includes('.')) {
        value = input.value.replace(/[^0-9]/g, '');
        input.value = value || String(this.appWholeNumberMin); // Default to min if invalid
      }

      // Enforce min and max bounds
      const numValue = parseInt(input.value, 10);
      if (isNaN(numValue) || numValue < this.appWholeNumberMin) {
        input.value = String(this.appWholeNumberMin);
        this.showNotification(`Quantity cannot be less than ${this.appWholeNumberMin}`);
      } else if (numValue > this.appWholeNumberMax) {
        input.value = String(this.appWholeNumberMax);
        this.showNotification(`Quantity cannot be greater than ${this.appWholeNumberMax}`);
      }
    } finally {
      this.isUpdating = false; // Reset flag
    }
  }

  @HostListener('blur')
  onBlur(): void {
    if (this.isUpdating) return; // Prevent recursive calls

    this.isUpdating = true;
    try {
      const input = this.el.nativeElement;
      const numValue = parseInt(input.value, 10);
      if (!input.value || isNaN(numValue) || numValue < this.appWholeNumberMin) {
        input.value = String(this.appWholeNumberMin);
        this.showNotification(`Quantity cannot be less than ${this.appWholeNumberMin}`);
      }
    } finally {
      this.isUpdating = false; // Reset flag
    }
  }

  private showNotification(message: string): void {
    const currentTime = Date.now();
    if (currentTime - this.lastNotificationTime >= this.notificationDebounceMs) {
      this.notificationService.showSnackBar(message, 'normal');
      this.lastNotificationTime = currentTime;
    }
  }
}