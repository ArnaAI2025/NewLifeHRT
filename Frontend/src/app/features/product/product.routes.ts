import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.Product;
export const productRoutes: Routes = [
  {
    path: 'product/view',
    loadComponent: () =>
      import('./product-view/product-view').then(m => m.ProductViewComponent),
      canActivate: [AuthGuard, PermissionGuard],
      data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'product/add',
    loadComponent: () =>
      import('./product-add/product-add').then(m => m.ProductAddComponent),
      canActivate: [AuthGuard, PermissionGuard],
      data: new RoutePermission(resource, PermissionAction.Create)
  },
  {
    path: 'product/edit/:id',
    loadComponent: () =>
      import('./product-add/product-add').then(m => m.ProductAddComponent),
      canActivate: [AuthGuard, PermissionGuard],
      data: new RoutePermission(resource, PermissionAction.Update)
  }
];
