import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.PriceListItem;
export const priceListItemRoutes: Routes = [
  {
    path: 'pricelistitem/view',
    loadComponent: () =>
      import('./price-list-item-view/price-list-item-view').then(m => m.PriceListItemViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'pricelistitem/add',
    loadComponent: () =>
      import('./price-list-item-add/price-list-item-add').then(m => m.PriceListItemAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Create)
  },
  {
    path: 'pricelistitem/edit/:id',
    loadComponent: () =>
      import('./price-list-item-add/price-list-item-add').then(m => m.PriceListItemAddComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Update)
  }
];
