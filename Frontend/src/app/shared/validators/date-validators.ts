import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
export class DateValidators {

  static minDateValidator(customMinDate?: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const selectedDate = new Date(control.value);
      const minDate = customMinDate ? new Date(customMinDate) : new Date();
      
      selectedDate.setHours(0, 0, 0, 0);
      minDate.setHours(0, 0, 0, 0);
      
      return selectedDate >= minDate ? null : { 
        pastDate: { 
          actualDate: selectedDate.toISOString().split('T')[0],
          requiredMinDate: minDate.toISOString().split('T')[0],
          message: `Date cannot be before ${minDate.toLocaleDateString()}`
        }
      };
    };
  }

  static maxDateValidator(customMaxDate?: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const selectedDate = new Date(control.value);
      const maxDate = customMaxDate ? new Date(customMaxDate) : new Date();
      
      selectedDate.setHours(0, 0, 0, 0);
      maxDate.setHours(23, 59, 59, 999); 
      
      return selectedDate <= maxDate ? null : { 
        futureDate: { 
          actualDate: selectedDate.toISOString().split('T')[0],
          requiredMaxDate: maxDate.toISOString().split('T')[0],
          message: `Date cannot be after ${maxDate.toLocaleDateString()}`
        }
      };
    };
  }

  static dateRangeValidator(minDate?: Date, maxDate?: Date): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const selectedDate = new Date(control.value);
      const min = minDate ? new Date(minDate) : new Date();
      const max = maxDate ? new Date(maxDate) : new Date();
      
      selectedDate.setHours(0, 0, 0, 0);
      min.setHours(0, 0, 0, 0);
      max.setHours(23, 59, 59, 999);
      
      if (selectedDate < min || selectedDate > max) {
        return {
          dateRange: {
            actualDate: selectedDate.toISOString().split('T')[0],
            minDate: min.toISOString().split('T')[0],
            maxDate: max.toISOString().split('T')[0],
            message: `Date must be between ${min.toLocaleDateString()} and ${max.toLocaleDateString()}`
          }
        };
      }
      
      return null;
    };
  }

  static getToday(): Date {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return today;
  }


  static getDateFromToday(days: number): Date {
    const date = DateValidators.getToday();
    date.setDate(date.getDate() + days);
    return date;
  }

  static getDateErrorMessage(control: AbstractControl | null, fieldName: string = 'Date'): string | null {
    if (!control || !control.errors) return null;

    if (control.errors['pastDate']) {
      return control.errors['pastDate'].message || `${fieldName} cannot be in the past`;
    }

    if (control.errors['futureDate']) {
      return control.errors['futureDate'].message || `${fieldName} cannot be in the future`;
    }

    if (control.errors['dateRange']) {
      return control.errors['dateRange'].message || `${fieldName} is not within allowed range`;
    }

    // Handle Angular Material built-in date errors
    if (control.errors['matDatepickerMin']) {
      return `${fieldName} is before minimum allowed date`;
    }

    if (control.errors['matDatepickerMax']) {
      return `${fieldName} is after maximum allowed date`;
    }

    return null;
  }
}
