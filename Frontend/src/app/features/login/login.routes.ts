import { Routes } from '@angular/router';
import { NoAuthGuard } from '../../core/guards/no-auth.guard';
export const loginRoutes: Routes = [
  {
    path : '',
    canActivate: [NoAuthGuard],
    loadComponent: () =>
      import('./home-page/home-page').then(m => m.HomePageComponent)
  },
  {
    path: 'login',
    canActivate: [NoAuthGuard],
    loadComponent: () =>
      import('./login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'logout',
    loadComponent: () =>
      import('./logout/logout.component').then(m => m.LogoutComponent)
  },
  {
    path: 'reset-password',
    canActivate: [NoAuthGuard],
    loadComponent: () =>
      import('./reset-password/reset-password').then(m => m.ResetPassword)
  }
];
