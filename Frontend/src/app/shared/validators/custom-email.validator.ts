import { AbstractControl, ValidationErrors } from "@angular/forms";

export function CustomEmailValidator(control: AbstractControl): ValidationErrors | null {
  const email = control.value;
  if (!email) return null;

  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$/;
  const invalidPatterns = [/\.\./, /\.{2,}/, /\.[A-Za-z]{2,}\.[A-Za-z]{2,}$/];

  if (!emailRegex.test(email) || invalidPatterns.some(p => p.test(email))) {
    return { invalidEmail: true };
  }

  return null;
}