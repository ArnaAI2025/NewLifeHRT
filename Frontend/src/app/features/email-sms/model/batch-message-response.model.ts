import { Status } from "../../../shared/enums/status.enum";
import { BatchMessageRecipientResponseDto } from "./batch-message-recipient-response.model";

export interface BatchMessageResponseDto {
  batchMessageId?: string;
  subject?: string;
  message: string;
  createdByUser?: string;
  createdByUserId: number;
  createdAt: string;
  approvedByUserId?: number;
  approvedAt?: string;
  status?: Status;
  notes?: string;
  isPatient?: boolean;

  isMail?: boolean;   
  isSms?: boolean;

  batchMessageRecipient: BatchMessageRecipientResponseDto[];
}
