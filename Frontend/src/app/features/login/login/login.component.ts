import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import {  LoginRequestDto } from '../model/login-request.model';
import { AuthService } from '../../../shared/services/auth.service';
import {  LoginVerifyOtpRequestDto } from '../model/login-verify-otp';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { TokenRefreshService } from '../../../shared/services/token-refresh.service';
import { CustomEmailValidator } from '../../../shared/validators/custom-email.validator';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})

export class LoginComponent implements OnInit, OnDestroy {
  // Forms
  loginForm: FormGroup;
  twoFactorForm: FormGroup;

  // UI State
  hidePassword = true;
  showTwoFactor = signal(false);
  countdown = signal(30);
  canResend = signal(false);
  countdownInterval: any;
  isLoading = signal(false);

  // Display
  userId!: number;
  otpId!: string;
  userEmail = '';

  formattedCountdown = computed(() => {
    const val = this.countdown();
    const min = Math.floor(val / 60).toString().padStart(2, '0');
    const sec = (val % 60).toString().padStart(2, '0');
    return `${min}:${sec}`;
  });

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private readonly authService: AuthService,
    private readonly userAccountService: UserAccountService,
    private readonly notificationService : NotificationService,
    private readonly tokenRefreshService : TokenRefreshService,
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email,CustomEmailValidator]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });

    this.twoFactorForm = this.formBuilder.group({
       code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  async ngOnInit(): Promise<void> {
    if (this.showTwoFactor()) {
      this.startCountdown();
    }
  }

  ngOnDestroy(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }

  onLoginSubmit(): void {
  if (!this.loginForm.valid) return;

  const request: LoginRequestDto = {
    email: this.loginForm.value.email,
    password: this.loginForm.value.password
  };

  this.isLoading.set(true);

  this.authService.login(request).subscribe({
    next: (res) => {
      this.userEmail = this.loginForm.value.email;

      if (res?.userId && res?.userId > 0) {
        if (res.tokens?.accessToken) {
          // Case 1: Normal login with tokens
          this.authService.persistSession(
            res.tokens.accessToken,
            res.tokens.refreshToken,
            res.userId
          );
          this.tokenRefreshService.start();
          this.router.navigate(['/dashboard']);
        } else if (res.mustChangePassword) {
          // Case 2: Must change password
          this.router.navigate(['/reset-password'], {
            state: {
              email: request.email,
              oldPassword: request.password
            }
          });
        } else {
          // Case 3: OTP required
          this.showTwoFactor.set(true);
          this.userId = res.userId;
          this.otpId = res.otpId;
          this.startCountdown();

          this.notificationService.showSnackBar(
            'OTP sent successfully! Please verify your code to continue.',
            'success'
          );
        }
      } else {
        // Login failed
        this.notificationService.showSnackBar(
          'Login failed. Verify credentials.',
          'failure'
        );
      }

      this.isLoading.set(false);
    },
    error: () => {
      this.notificationService.showSnackBar(
        'Login failed. Please try again.',
        'failure'
      );
      this.isLoading.set(false);
    }
  });
}

    resendCode(): void {
    if (this.canResend()) {
      this.startCountdown();
      this.onLoginSubmit();
    }
  }
  startCountdown(): void {
    this.canResend.set(false);
    this.countdown.set(30);

    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }

    this.countdownInterval = setInterval(() => {
      const current = this.countdown();
      if (current > 0) {
        this.countdown.set(current - 1);
      } else {
        clearInterval(this.countdownInterval);
        this.canResend.set(true);
      }
    }, 1000);
  }

on2FASubmit(): void {
  if (this.twoFactorForm.valid && this.otpId) {
    const code = Object.values(this.twoFactorForm.value).join('');
    const payload: LoginVerifyOtpRequestDto = {
      email: this.loginForm.value.email,
      otpCode: code,
      otpId: this.otpId
    };
    this.isLoading.set(true);
this.authService.verifyOtp(payload).subscribe({
  next: (res) => {
    if (res?.accessToken) {
      this.authService.persistSession(res.accessToken, res.refreshToken, this.userId);
      this.tokenRefreshService.register();
      var patientUserId = this.authService.getPatientUserId();
      var isUserPatient = !!patientUserId && patientUserId.trim() !== ''
      if(isUserPatient){
        this.router.navigate([`/patient/edit/${patientUserId}`], {
        });
      }else{
        this.router.navigate(['/dashboard']);
      }

      this.notificationService.showSnackBar(
        'OTP verified successfully!',
        'success'
      );
    } else {
      this.notificationService.showSnackBar(
        'Please check your OTP.',
        'failure'
      );
      this.twoFactorForm.reset();
    }
    this.isLoading.set(false);
  },
  error: () => {
    this.notificationService.showSnackBar(
      'Invalid or expired OTP.',
      'failure'
    );
    this.twoFactorForm.reset();
    this.isLoading.set(false);
  }
});

  }
}

onCodeInput(event: any): void {
  const input = event.target as HTMLInputElement;
  input.value = input.value.replace(/\D/g, '').slice(0, 6);
  this.twoFactorForm.get('code')?.setValue(input.value, { emitEvent: false });
}

  forgotPassword(): void {
    // Hook logic to trigger forgot password flow
  }
}
