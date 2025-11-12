export interface PatientAttachmentResponseDto {
  id?: string;              
  attachmentId?: string;    
  attachmentName: string;   
  fileType?: string;   
  fileUrl?: string;     
  extension: string;        
  createdAt: string;        
  createdBy: string;        
  updatedAt?: string;       
  updatedBy?: string;       
  categoryName?: string;    
  file: File;
  fileCategory: string;
  fileCategoryId: number;
  dummyId : number;
}
