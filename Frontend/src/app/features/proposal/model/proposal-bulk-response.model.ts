export interface ProposalBulkResponseDto {
  id: string;                    
  name: string;
  patientName: string;
  assignedUserName : string;
  pharmacyName: string;
  isActive: boolean;
  counselorName: string;
  therapyExpiration?: string;    
  createdAt: string;             
  status?: number;
}
