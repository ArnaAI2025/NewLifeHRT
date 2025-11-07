import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainLayoutComponent } from "./layout/main-layout/main-layout";
import { TokenRefreshService } from './shared/services/token-refresh.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'frontend';
  private tokenRefreshService = inject(TokenRefreshService);
  ngOnInit() {
    this.tokenRefreshService.register();
  }
}
