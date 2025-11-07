import { Status } from '../../../shared/enums/status.enum';
import { BatchMessageRecipientRequestDto } from './batch-message-recipient-request.model';
export interface BatchMessageRequestDto {
  batchMessageId?: string; 
  subject?: string | null;
  createdByUserName?: string | null;
  message: string;
  createdByUserId: number;
  createdAt: string; 
  approvedByUserId?: number | null;
  approvedAt?: string | null; 
  status?: Status | null; 
  notes?: string | null;
  isSms?: boolean | null;
  isMail?: boolean | null;
  batchMessageRecipients?: BatchMessageRecipientRequestDto[];
}