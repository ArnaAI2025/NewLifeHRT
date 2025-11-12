import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { HttpService } from '../services/http.service';
import { ApiResource } from '../constants/api-resource';
import { ApiHelper } from '../constants/api.helper';
import { HttpRequestOptions } from '../models/http-request-options.model';
import { catchError, Observable, of } from 'rxjs';
import { LoginResponseDto } from '../../features/login/model/login-response.model';
import { LoginRequestDto } from '../../features/login/model/login-request.model';
import { LoginVerifyOtpRequestDto } from '../../features/login/model/login-verify-otp';
import { VerifyOtpResponseDto } from '../../features/login/model/verify-otp-response.model';
import { ResetPasswordRequestDto } from '../../features/login/model/reset-password-request.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    private baseHttp: HttpService,
    private appSettingsService: AppSettingsService
  ) {}

  login(loginRequest: LoginRequestDto): Observable<LoginResponseDto | null> {
    const url = ApiResource.getURI(ApiHelper.auth.base, ApiHelper.auth.login);

    const options: HttpRequestOptions = {
      attachToken: false,
      skipEncryption: false,
      timeout: 8000,
      headers: {
        'Content-Type': 'application/json',
        'X-No-Auth': 'true'
      }
    };

    return this.baseHttp.post<LoginResponseDto>(url, loginRequest, options).pipe(
      catchError(() => of(null))
    );
  }

  verifyOtp(payload: LoginVerifyOtpRequestDto): Observable<VerifyOtpResponseDto | null> {
    const url = ApiResource.getURI(ApiHelper.auth.base, ApiHelper.auth.verifyOtp);

    const options: HttpRequestOptions = {
      attachToken: false,
      skipEncryption: true,
      timeout: 8000,
      headers: {
        'Content-Type': 'application/json',
        'X-No-Auth': 'true'
      }
    };

    return this.baseHttp.post<VerifyOtpResponseDto>(url, payload, options).pipe(
      catchError(() => of(null))
    );
  }

  persistSession(token: string, refreshToken: string, user: any): void {
    this.appSettingsService.setTokens(token, refreshToken);    
  }

  resetPassword(payload: ResetPasswordRequestDto): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.auth.base, ApiHelper.auth.resetPassword);

    const options: HttpRequestOptions = {
      attachToken: false, 
      skipEncryption: true,
      timeout: 8000,
      headers: {
        'Content-Type': 'application/json',
        'X-No-Auth': 'true'
      }
    };

    return this.baseHttp.post<any>(url, payload, options).pipe(
      catchError((error) => {
        console.error("Reset password error:", error);
        return of(null); 
      })
    );
  }

  getPatientUserId(): string {
    return this.appSettingsService.getPatientUserId();
  }



}
