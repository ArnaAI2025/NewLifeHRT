import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
export const dashboardRoute: Routes = [
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./dashboard/dashboard').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'patient-proposal',
    loadComponent: () =>
      import('./patient-proposal/patient-proposal').then(m => m.PatientProposalComponent),
    canActivate: [AuthGuard]
  }

];

