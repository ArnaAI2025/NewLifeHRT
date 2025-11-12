import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';

const resource = PermissionResource.Lead;
export const CommissionRoutes: Routes = [
  {
    path: 'commission-pool/view',
    loadComponent: () =>
      import('./commission-pool-view/commission-pool-view').then(m => m.CommissionPoolViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
  path: 'counselor-order-wise-commissions/view/:poolDetailId',
  loadComponent: () =>
    import('./counselor-commission-view/counselor-commission-view').then(m => m.CounselorCommissionView),
  canActivate: [AuthGuard, PermissionGuard],
  data: new RoutePermission(resource, PermissionAction.Read)
},
{
  path: 'counselor-order-wise-detail/view/:commissionsPayableId',
  loadComponent: () =>
    import('./counselor-commission-detail-view/counselor-commission-detail-view').then(m => m.CounselorCommissionDetailView),
  canActivate: [AuthGuard, PermissionGuard],
  data: new RoutePermission(resource, PermissionAction.Read)
}

];