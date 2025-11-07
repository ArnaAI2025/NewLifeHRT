export interface OrderBulkResponseDto {
  id: string; 
  name: string;
  patientName: string;
  pharmacyName: string;
  counselorName: string;
  therapyExpiration?: string; 
  createdAt: string;          
  status?: number;
}
