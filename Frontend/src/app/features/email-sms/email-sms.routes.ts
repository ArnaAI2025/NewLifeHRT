import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const emailAndSmsRoutes: Routes = [
  {
    path: 'patient/:patientId/sms',   
    loadComponent: () =>
      import('./sms/sms').then(m => m.Chat),
    canActivate: [AuthGuard]
  },
    {
    path: 'lead-management/:leadId/sms',   
    loadComponent: () =>
      import('./sms/sms').then(m => m.Chat),
    canActivate: [AuthGuard]
  },
  {
    path: 'sms',   
    loadComponent: () =>
      import('./sms/sms').then(m => m.Chat),
    canActivate: [AuthGuard]
  },
  {
    path: 'bulk-sms',   
    loadComponent: () =>
      import('./bulk-sms/bulk-sms').then(m => m.BulkSms),
    canActivate: [AuthGuard]
  },
  {
    path: 'unseen-sms',   
    loadComponent: () =>
      import('./unseen-sms/unseen-sms').then(m => m.UnSeenSms),
    canActivate: [AuthGuard]
  },
    {
    path: 'bulk-sms-approval',   
    loadComponent: () =>
      import('./bulk-sms-approval/bulk-sms-approval').then(m => m.BulkSmsApproval),
    canActivate: [AuthGuard]
  },
{
  path: 'edit-bulk/sms/:batchMessageId',   
  loadComponent: () =>
    import('./bulk-sms/bulk-sms').then(m => m.BulkSms),
  canActivate: [AuthGuard]
}

];
