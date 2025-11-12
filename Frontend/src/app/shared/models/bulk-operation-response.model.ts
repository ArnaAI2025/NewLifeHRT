export interface BulkOperationResponseDto {
  successCount: number;
  failedCount: number;
  message: string;
  failedIds: string[];
  successIds: string[];
}
