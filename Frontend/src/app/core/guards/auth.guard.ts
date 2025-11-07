import { Injectable } from '@angular/core';
import { CanActivate ,Router, UrlTree} from '@angular/router';
import { Observable } from 'rxjs';
import { AppSettingsService } from '../../shared/services/app-settings.service';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {

  constructor(private router: Router, private appSettings: AppSettingsService) {}

  canActivate(): boolean | UrlTree {
    const isLoggedIn = !!this.appSettings.getLocalStorage(`${this.appSettings.tenantName}:access_token`);
    const isTokenExpired = this.appSettings.isTokenExpired();
    if (isLoggedIn && !isTokenExpired) {
      return true;
    }else if(isLoggedIn && isTokenExpired){
      return this.router.parseUrl('/logout');
    } else {
      return this.router.parseUrl('/login');
    }
  }
}
