export interface PatientCreditCardResponseDto {
  patientId?: string;
  id: string;
  cardType: number;
  month: number;
  year: string;
  cardNumber: string;
  isActive: boolean;
  isDefaultCreditCard?: boolean;
  createdBy?: string;
  updatedBy?: string;
}
