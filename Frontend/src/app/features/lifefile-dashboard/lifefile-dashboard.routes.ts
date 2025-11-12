import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

const resource = PermissionResource.CommissionRatePerProduct;
export const lifefileDashboardRoutes: Routes = [
  {
    path: 'lifefiledashboard/view',
    loadComponent: () =>
      import('./lifefile-dashboard-view/lifefile-dashboard-view').then(m => m.LifefileDashboardViewComponent),
    canActivate: [AuthGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  }
];
