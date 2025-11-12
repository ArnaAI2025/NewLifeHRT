export interface MedicalRecommendationRequestDto {
  id?: string; 
  consultationDate: string; 
  patientId: string;
  doctorId: number;
  medicationTypeId: number;
  otherMedicationType : string;
  followUpLabTestId?: number;
  title?: string;
  pMHx?: string;
  pSHx?: string;
  fHx?: string;
  suppliments?: string;
  medication?: string;
  socialHistory?: string;
  allergies?: string;
  hrt?: string;
  subjective?: string;
  objective?: string;
  assessment?: string;
  plan?: string;
  socialPoint?: string;
  notes?: string;
}
