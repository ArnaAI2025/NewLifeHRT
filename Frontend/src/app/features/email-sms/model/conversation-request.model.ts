import { MessageContentRequest } from "./message-content-request";
import { MessageRequest } from "./message-request.model";

export interface ConversationRequest {
  patientId?: string;
  leadId?: string;
  to: string; 
  message: MessageRequest;
  messageContent: MessageContentRequest;
}