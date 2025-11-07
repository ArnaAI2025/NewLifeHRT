import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { AuthService } from '../../../shared/services/auth.service';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-reset-password',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPassword implements OnInit { 
  resetForm!: FormGroup;
  hideOld = true;
  hideNew = true;
  isLoading = signal(false);
  
  private readonly MIN_LENGTH = 12;

  constructor(
    private fb: FormBuilder, 
    private router: Router, 
    private authService: AuthService, 
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.initializeResetForm();
  }

  initializeResetForm() {
    const state = history.state as { email?: string; oldPassword?: string }; 
    const email = state?.email || '';
    const oldPassword = state?.oldPassword || '';

    this.resetForm = this.fb.group({
      email: [{ value: email, disabled: true }, [Validators.required, Validators.email]],
      oldPassword: [
        { value: oldPassword, disabled: true },
        [Validators.required, Validators.minLength(this.MIN_LENGTH)]
      ], 
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(this.MIN_LENGTH), 
        this.identityPasswordValidator()
      ]]
    }, {
        updateOn: 'change' 
    });
  }

  toggleHideOld(event: MouseEvent) {
    this.hideOld = !this.hideOld;
    event.stopPropagation(); 
    event.preventDefault(); 
  }

  toggleHideNew(event: MouseEvent) {
    this.hideNew = !this.hideNew;
    event.stopPropagation(); 
    event.preventDefault();
  }

  onSubmit() {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }
      this.isLoading.set(true);

      const payload = this.resetForm.getRawValue();

      this.authService.resetPassword(payload).subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res?.id) { 
            this.notificationService.showSnackBar(res.message || "Password changed successfully", 'success');
            this.router.navigate(['/login']);
          } else {
            this.notificationService.showSnackBar(res?.message || "Failed to reset password", 'failure');
          }
        },
        error: (err) => {
          this.isLoading.set(false);
          this.notificationService.showSnackBar(err.error?.message || "Failed to reset password. Please try again.", 'failure');
        }
      });
  }

  identityPasswordValidator(): ValidatorFn {
      return (control: AbstractControl): ValidationErrors | null => {
          const value = control.value;
          if (!value) {
              return null; 
          }
        
          const errors: ValidationErrors = {};

          const hasDigit = /[0-9]+/.test(value);
          if (!hasDigit) {
              errors['requiresDigit'] = true;
          }
        
          const hasNonAlphanumeric = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(value);
          if (!hasNonAlphanumeric) {
              errors['requiresNonAlphanumeric'] = true;
          }
        
          const hasLower = /[a-z]+/.test(value);
          if (!hasLower) {
              errors['requiresLower'] = true;
          }
        
          const hasUpper = /[A-Z]+/.test(value);
          if (!hasUpper) {
              errors['requiresUpper'] = true;
          }
        
          return Object.keys(errors).length ? errors : null;
      };
  }

}

