import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const DoctorManagementAndMedicalRecommandationRoutes: Routes = [
  {
    path: 'counselor-view/:patientId',
    loadComponent: () =>
      import('./counselor-notes/counselor-notes')
        .then(m => m.CounselorNotesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'medication-recommendation-view/:patientId',
    loadComponent: () =>
      import('./medical-recommandation-view/medical-recommandation-view')
        .then(m => m.MedicalRecommandationView),
    canActivate: [AuthGuard]
  },
  {
    path: 'medication-recommendation-edit/:patientId/:medicalRecommendationId',
    loadComponent: () =>
      import('./medical-recommendation-add/medical-recommendation-add')
        .then(m => m.MedicalRecommendationAddComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'medication-recommendation-add/:patientId',
    loadComponent: () =>
      import('./medical-recommendation-add/medical-recommendation-add')
        .then(m => m.MedicalRecommendationAddComponent),
    canActivate: [AuthGuard]
  }
];
