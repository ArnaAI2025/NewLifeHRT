import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.User;
export const userManagementRoutes: Routes = [
  {
    path: 'admin/view',
    loadComponent: () =>
      import('./admin/admin-view/admin-view').then(m => m.AdminViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'doctor/view',
    loadComponent: () =>
      import('./doctor/doctor-view/doctor-view').then(m => m.DoctorViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'nurse/view',
    loadComponent: () =>
      import('./nurse/nurse-view/nurse-view').then(m => m.NurseViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
    {
    path: 'receptionist/view',
    loadComponent: () =>
      import('./receptionist/receptionist-view/receptionist-view').then(m => m.ReceptionistViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'sales-person/view',
    loadComponent: () =>
      import('./sales-person/sales-person-view/sales-person-view').then(m => m.SalesPersonViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'edit/:fromPage/:userId',
    loadComponent: () =>
      import('./user-management-form/user-management-form').then(m => m.UserManagementFormComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Update)
  },
  {
    path: 'add/:fromPage',
    loadComponent: () =>
      import('./user-management-form/user-management-form').then(m => m.UserManagementFormComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Create)
  }
];
