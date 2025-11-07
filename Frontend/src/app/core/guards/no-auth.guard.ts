// no-auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AppSettingsService } from '../../shared/services/app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(
    private appSettingsService: AppSettingsService,
    private router: Router
  ) {}

  canActivate(): boolean {
    if (this.appSettingsService.isUserLoggedIn() && !this.appSettingsService.isTokenExpired()) {
      this.router.navigate(['/dashboard']);
      return false;
    }
    return true;
  }
}
