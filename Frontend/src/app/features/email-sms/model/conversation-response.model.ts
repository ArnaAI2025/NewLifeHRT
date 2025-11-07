import { MessageResponseDto } from "./message-response.model";

export interface ConversationResponseDto {
  conversationId: string;                 
  patientId?: string | null; 
  name?: string | null; 
  phoneNumber?: string | null;              
  leadId?: string | null;                 
  currentCounselorId?: number | null;      
  currentCounselorName?: string | null;    
  messages: MessageResponseDto[];
}

