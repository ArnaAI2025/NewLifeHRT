import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.PharmacyConfiguration;
export const pharmacyConfigurationRoutes: Routes = [
  {
    path: 'pharmacyconfiguration/view',
    loadComponent: () =>
      import('./pharmacy-configuration-view/pharmacy-configuration-view').then(m => m.PharmacyConfigurationViewComponent),
      canActivate: [AuthGuard],
      data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'pharmacyconfiguration/add',
    loadComponent: () =>
      import('./pharmacy-configuration-add/pharmacy-configuration-add').then(m => m.PharmacyConfigurationAddComponent),
      canActivate: [AuthGuard],
      data: new RoutePermission(resource, PermissionAction.Create)
  },
  {
    path: 'pharmacyconfiguration/edit/:id',
    loadComponent: () =>
      import('./pharmacy-configuration-add/pharmacy-configuration-add').then(m => m.PharmacyConfigurationAddComponent),
      canActivate: [AuthGuard],
      data: new RoutePermission(resource, PermissionAction.Update)
  }
];
