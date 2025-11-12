export interface MessageRequest {
  conversationId?: string;
  userId?: number; 
  twilioId?: string;
  direction?: string;
  isRead?: boolean;
  isSent?: boolean;
  timestamp?: string;
}