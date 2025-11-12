import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

export const orderProductRefillRoutes: Routes = [
  {
    path: 'order-product-refill/view',
    loadComponent: () =>
      import('./order-product-refill-view/order-product-refill-view').then(m => m.OrderProductRefillViewComponent),
    canActivate: [AuthGuard],
  },
  {
    path: 'order-product-refill/edit/:id',
    loadComponent: () =>
      import('./order-product-refill-add/order-product-refill-add').then(m => m.OrderProductRefillAddComponent),
    canActivate: [AuthGuard],
  }
];
