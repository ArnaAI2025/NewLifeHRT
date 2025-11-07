import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const reminderRoutes: Routes = [
  {
    path: 'patient/:patientId/reminder',
    loadComponent: () =>
      import('./reminder-view/reminder-view').then(m => m.ReminderViewComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'lead-management/:leadId/reminder',
    loadComponent: () =>
      import('./reminder-view/reminder-view').then(m => m.ReminderViewComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'reminderdashboard/view',
    loadComponent: () =>
      import('./reminder-dashboard/reminder-dashboard').then(m => m.ReminderDashboardComponent),
    canActivate: [AuthGuard]
  }
];
