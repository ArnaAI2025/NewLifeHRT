import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const OrderRoutes: Routes = [
  // View Orders
  {
    path: 'orders/view',
    loadComponent: () =>
      import('./orders-view/orders-view').then(m => m.OrdersView),
    canActivate: [AuthGuard]
  },
  {
    path: 'orders/view/:patientId',
    loadComponent: () =>
      import('./orders-view/orders-view').then(m => m.OrdersView),
    canActivate: [AuthGuard]
  },

  // Add Order

  //  currently on hold from client side
  
  // {
  //   path: 'order/add',
  //   loadComponent: () =>
  //     import('./order-add/order-add').then(m => m.OrderAddComponent),
  //   canActivate: [AuthGuard]
  // },
  {
    path: 'order/add/:patientId',
    loadComponent: () =>
      import('./order-add/order-add').then(m => m.OrderAddComponent),
    canActivate: [AuthGuard]
  },

  // Edit Order
  {
    path: 'order/edit/:id',
    loadComponent: () =>
      import('./order-add/order-add').then(m => m.OrderAddComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'order/edit/:id/:patientId',
    loadComponent: () =>
      import('./order-add/order-add').then(m => m.OrderAddComponent),
    canActivate: [AuthGuard]
  },

  // Full Prescription
  {
    path: 'order-full',
    loadComponent: () =>
      import('./prescription/prescription').then(m => m.Prescription),
    canActivate: [AuthGuard]
  }
];
