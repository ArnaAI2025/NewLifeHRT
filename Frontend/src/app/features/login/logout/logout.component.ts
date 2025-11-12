import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AppSettingsService } from '../../../shared/services/app-settings.service';
import { TokenRefreshService } from '../../../shared/services/token-refresh.service';

@Component({
  selector: 'app-logout.component',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss']
})
export class LogoutComponent {
  private readonly router =  inject(Router);
  private readonly appSettingsService = inject(AppSettingsService); 
  private readonly tokenRefreshService = inject(TokenRefreshService); 
  ngOnInit(): void {
    this.appSettingsService.clearAll();
    this.tokenRefreshService.stopAll();
  }

  loginAgain(): void {
    this.router.navigate(['/login']);
  }
}
