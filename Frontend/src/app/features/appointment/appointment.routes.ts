import { Routes } from '@angular/router';
export const AppointmentRoutes: Routes = [
  {
    path: 'appointment/view',
    loadComponent: () =>
      import('./appointment-view/appointment-view').then(m => m.AppointmentViewComponent)
  },
  {
        path: 'appointment/add',
        loadComponent: () =>
          import('./appointment-add/appointment-add').then(m => m.AppointmentAddComponent),
  },
  {
        path: 'appointment/edit/:id',
        loadComponent: () =>
          import('./appointment-add/appointment-add').then(m => m.AppointmentAddComponent),
  },
  {
        path: 'appointment/view/:patientId',
        loadComponent: () =>
          import('./patient-appointment-view/patient-appointment-view').then(m => m.PatientAppointmentViewComponent),
  },
];
