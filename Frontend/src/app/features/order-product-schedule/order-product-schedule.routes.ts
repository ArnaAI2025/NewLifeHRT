import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

export const orderProductScheduleRoutes: Routes = [
  {
    path: 'order-product-schedule/view',
    loadComponent: () =>
      import('./order-product-schedule-summary-view/order-product-schedule-summary-view').then(m => m.OrderProductScheduleSummaryViewComponent),
    canActivate: [AuthGuard],
  },
  {
    path: 'order-product-schedule/calendar',
    loadComponent: () =>
      import('./order-product-schedule-calendar/order-product-schedule-calendar').then(m => m.OrderProductScheduleCalendarComponent),
    canActivate: [AuthGuard],
  }
];
