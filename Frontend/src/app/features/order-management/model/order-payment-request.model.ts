export interface OrderPaymentRequestDto {
  orderId: string;
  isOrderPaid: boolean | null;
  isCashPayment: boolean | null;
}
