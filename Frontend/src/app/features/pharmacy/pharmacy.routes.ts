import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.Pharmacy;
export const pharmacyRoutes: Routes = [
  {
    path: 'pharmacy/view',
    loadComponent: () =>
      import('./pharmacy-view/pharmacy-view').then(m => m.PharmacyViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'pharmacy/add',
    loadComponent: () =>
      import('./pharmacy-add/pharmacy-add').then(m => m.PharmacyAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Create)
  },
  {
    path: 'pharmacy/edit/:id',
    loadComponent: () =>
      import('./pharmacy-add/pharmacy-add').then(m => m.PharmacyAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Update)
  }
];
