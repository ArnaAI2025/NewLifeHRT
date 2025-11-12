import { Routes } from '@angular/router';
import { loginRoutes } from './features/login/login.routes';
import { LayoutComponent } from './layout/layout.component';
import { dashboardRoute } from './features/dashboard/dashboard.routes';
import { userManagementRoutes } from './features/user-management/user-management.routes';
import { productRoutes } from './features/product/product.routes';
import { patientsRoutes } from './features/patient/patient.routes';
import { pharmacyRoutes } from './features/pharmacy/pharmacy.routes';
import { LeadManagementRoutes } from './features/lead-management/lead-management.routes';
import { priceListItemRoutes } from './features/price-list-items/price-list-item.routes';
import { commisionRateRoutes } from './features/commision-rates/commision-rate.routes';
import {DoctorManagementAndMedicalRecommandationRoutes} from './features/doctor-management-and-medical-recommandation/doctor-management-and-medical-recommandation.routes'
import { ProposalRoutes } from './features/proposal/proposal.routes';
import { AppointmentRoutes } from './features/appointment/appointment.routes';
import { HolidayRoutes } from './features/holiday/holiday.routes';
import { OrderRoutes } from './features/order-management/order.routes';
import { emailAndSmsRoutes } from './features/email-sms/email-sms.routes';
import { pharmacyConfigurationRoutes } from './features/pharmacy-configuration/pharmacy-configuration.routes';
import { lifefileDashboardRoutes } from './features/lifefile-dashboard/lifefile-dashboard.routes';
import { CommissionRoutes } from './features/commission/commission.routes';
import { reminderRoutes } from './features/reminder/reminder.routes';
import { orderProductRefillRoutes } from './features/order-product-refill/order-product-refill.routes';
import { orderProductScheduleRoutes } from './features/order-product-schedule/order-product-schedule.routes';

export const routes: Routes = [
  ...loginRoutes,
  {
    path: '',
    component: LayoutComponent,
    children: [
      ...dashboardRoute,
      ...userManagementRoutes,
      ...productRoutes,
      ...patientsRoutes,
      ...pharmacyRoutes,
      ...priceListItemRoutes,
      ...commisionRateRoutes,
      ...LeadManagementRoutes,
      ...DoctorManagementAndMedicalRecommandationRoutes,
      ...ProposalRoutes,
      ...AppointmentRoutes,
      ...HolidayRoutes,
      ...OrderRoutes,
      ...emailAndSmsRoutes,
      ...CommissionRoutes,
      ...pharmacyConfigurationRoutes,
      ...lifefileDashboardRoutes,
      ...reminderRoutes,
      ...orderProductRefillRoutes,
      ...orderProductScheduleRoutes
    ]
  },
  {
    path: 'unauthorized',
    loadComponent: () => import('./shared/components/unauthorized/unauthorized').then(m => m.UnauthorizedComponent)
  },
  {
    path: '**',
    loadComponent: () => import('./shared/components/not-found/not-found').then(m => m.NotFoundComponent)
  }
];
