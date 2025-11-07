import { Status } from '../../../shared/enums/status.enum';

export interface BatchMessageRecipientResponseDto {
  batchMessageRecipientId?: string;  
  patientId?: string;
  leadId?: string;  
  name?: string;
  status?: Status;
}
