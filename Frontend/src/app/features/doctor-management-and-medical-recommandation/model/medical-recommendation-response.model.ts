export interface MedicalRecommendationResponseDto {
  consultationDate: string;       
  patientId: string;              
  id: string;                    
  doctorId: number;
  medicationTypeId: number;
  medicationTypeName: string;
  followUpLabTestId?: number | null;
  title?: string | null;
  pMHx?: string | null;
  pSHx?: string | null;
  fHx?: string | null;
  suppliments?: string | null;
  medication?: string | null;
  socialHistory?: string | null;
  allergies?: string | null;
  hRT?: string | null;
  subjective?: string | null;
  objective?: string | null;
  assessment?: string | null;
  plan?: string | null;
  socialPoint?: string | null;
  notes?: string | null;
}
