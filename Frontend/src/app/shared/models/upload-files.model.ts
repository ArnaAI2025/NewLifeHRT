import { UploadFileItemDto } from "./upload-file.model";

export interface UploadFilesRequestDto {
  files: UploadFileItemDto[];
  id: string | number; 
}