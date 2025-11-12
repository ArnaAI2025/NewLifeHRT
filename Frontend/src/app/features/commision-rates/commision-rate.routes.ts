import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

const resource = PermissionResource.CommissionRatePerProduct;
export const commisionRateRoutes: Routes = [
  {
    path: 'commissionrate/view',
    loadComponent: () =>
      import('./commision-rate-view/commision-rate-view').then(m => m.CommisionRateViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'commissionrate/add',
    loadComponent: () =>
      import('./commision-rate-add/commision-rate-add').then(m => m.CommisionRateAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Create)
  },
  {
    path: 'commissionrate/edit/:id',
    loadComponent: () =>
      import('./commision-rate-add/commision-rate-add').then(m => m.CommisionRateAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Update)
  }
];
