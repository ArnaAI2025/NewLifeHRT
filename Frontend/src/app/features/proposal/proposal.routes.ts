import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
export const ProposalRoutes: Routes = [
   {
    path: 'proposals/view',
    loadComponent: () =>
      import('./proposal-view/proposal-view').then(m => m.ProposalView),canActivate: [AuthGuard]
  },
           {
    path: 'proposals/view/:patientId',
    loadComponent: () =>
      import('./proposal-view/proposal-view').then(m => m.ProposalView),canActivate: [AuthGuard]
  },
     {
    path: 'proposal/add',
    loadComponent: () =>
      import('./proposal-add/proposal-add').then(m => m.ProposalAdd),canActivate: [AuthGuard]
  },
         {
    path: 'proposal/add/:patientId',
    loadComponent: () =>
      import('./proposal-add/proposal-add').then(m => m.ProposalAdd),canActivate: [AuthGuard]
  },
       {
    path: 'proposal/edit/:proposalId',
    loadComponent: () =>
      import('./proposal-add/proposal-add').then(m => m.ProposalAdd),canActivate: [AuthGuard]
  },
  {
  path: 'patient/:patientId/proposal/edit/:proposalId',
  loadComponent: () =>
    import('./proposal-add/proposal-add').then(m => m.ProposalAdd),
  canActivate: [AuthGuard]
},

    {
    path: 'coupon/add',
    loadComponent: () =>
      import('./coupon-add/coupon-add').then(m => m.CouponAddComponent),
    canActivate: [AuthGuard]
  },
  {
  path: 'coupon/edit/:id',
  loadComponent: () =>
    import('./coupon-add/coupon-add').then(m => m.CouponAddComponent),
  canActivate: [AuthGuard]
},
{
  path: 'coupons/view',
  loadComponent: () =>
    import('./coupon-view/coupon-view').then(m => m.CouponView),
  canActivate: [AuthGuard]
}
]