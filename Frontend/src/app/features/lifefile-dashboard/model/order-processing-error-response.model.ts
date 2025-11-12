import { ApiTransactionResponseDto } from "./api-transaction-response.model";

export interface OrderProcessingErrorResponseDto {
  orderId: string;
  orderName: string;
  pharmacyName: string;
  integrationType: string;
  status: string;
  transactions: ApiTransactionResponseDto[];
}
