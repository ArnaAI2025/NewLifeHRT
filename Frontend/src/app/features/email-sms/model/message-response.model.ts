import { MessageContentResponseDto } from "./message-content-response.model";

export interface MessageResponseDto {
  messageId: string;       
  id ?: string | null;                
  counselorId?: number | null;             
  counselorName?: string | null;           
  twilioId?: string | null; 
  name?: string | null;               
  direction: string;                       
  isRead: boolean;       
  isPatient : boolean;                    
  timestamp: string;                       
  isSent: boolean;                         
  messageContents: MessageContentResponseDto[];
}