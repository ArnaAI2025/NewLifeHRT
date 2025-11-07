export interface MediaContent {
  contentType: string;
  content: string;
}

export interface SmsMessage {
  sender: Sender;
  text?: string;
  html?: string;
  list?: string[];
  mediaContents?: MediaContent[]; 

  mediaUrl?: string;
  content?: string;      
  contentType?: string;  
  time: string;
  status?: 'Delivered' | 'Read';
counselorInitials?: string;
}
export type Sender = 'clinic' | 'patient';