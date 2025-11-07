import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

const resource = PermissionResource.Lead;
export const LeadManagementRoutes: Routes = [
  {
    path: 'lead-management/view',
    loadComponent: () =>
      import('./lead-management-view/lead-management-view').then(m => m.LeadManagementView),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
    {
    path: 'lead-management/add',
    loadComponent: () =>
      import('./lead-management-add/lead-management-add').then(m => m.LeadManagementAddComponent),
    canActivate: [AuthGuard],
  },
  {
    path: 'lead-management/edit/:id',
    loadComponent: () =>
      import('./lead-management-add/lead-management-add').then(m => m.LeadManagementAddComponent),
    canActivate: [AuthGuard],
  }
];
