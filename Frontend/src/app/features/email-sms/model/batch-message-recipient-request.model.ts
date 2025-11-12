import { Status } from '../../../shared/enums/status.enum';
export interface BatchMessageRecipientRequestDto {
  batchMessageRecipientId?: string; 
  patientId?: string | null; 
  leadId?: string | null;
  status?: Status; 
  errorReason?: string | null;
}