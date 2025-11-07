export interface OrderPaymentResponseDto {
  id: string;
  isSuccess: boolean;
  message: string;
  isOrderPaid: boolean | null;
  isCashPayment: boolean | null;
  orderPaidDate?: string;
}
