import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
export const HolidayRoutes: Routes = [
  {
        path: 'holiday/add',
        loadComponent: () =>
          import('./holiday-add/holiday-add').then(m => m.HolidayAddComponent),
        canActivate: [AuthGuard]
  }
];
