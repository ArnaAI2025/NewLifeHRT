export class OrderProductScheduleResponseDto {
  orderProductScheduleId!: string;
  orderId!: string;
  orderName!: string;
  productId!: string;
  productName!: string;
  protocol?: string;
  timeZone!: string;
  occurrenceDateAndTime!: Date | null;
}
