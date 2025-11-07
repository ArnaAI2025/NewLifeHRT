import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionAction, PermissionResource } from '../../shared/constants/permissions.enums';
import { RoutePermission } from '../../shared/models/route-permission';
import { PermissionGuard } from '../../core/guards/permission.guard';

const resource = PermissionResource.Patient;
export const patientsRoutes: Routes = [
  {
    path: 'patients/view',
    loadComponent: () =>
      import('./patient-view/patient-view').then(m => m.PatientViewComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: new RoutePermission(resource, PermissionAction.Read)
  },
  {
    path: 'patient/add',
    loadComponent: () =>
      import('./patient-add/patient-add').then(m => m.PatientAddComponent),
    canActivate: [AuthGuard],
  },
  {
    path: 'patient/edit/:id',
    loadComponent: () =>
      import('./patient-add/patient-add').then(m => m.PatientAddComponent),
    canActivate: [AuthGuard],
  },
  {
  path: 'shipping-address/add/:id',
  loadComponent: () =>
    import('./shipping-address-add/shipping-address-add').then(m => m.ShippingAddressAddComponent),
  canActivate: [AuthGuard]
},
  {
  path: 'shipping-address/edit/:id/:shippingAddressId',
  loadComponent: () =>
    import('./shipping-address-add/shipping-address-add').then(m => m.ShippingAddressAddComponent),
  canActivate: [AuthGuard]
},
  {
  path: 'shipping-address/view/:patientId',
  loadComponent: () =>
    import('./shipping-address-view/shipping-address-view').then(m => m.ShippingAddressViewComponent),
    canActivate: [AuthGuard]
  },
];
