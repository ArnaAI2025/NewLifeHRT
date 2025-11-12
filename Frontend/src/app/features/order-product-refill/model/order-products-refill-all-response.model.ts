export class OrderProductRefillAllResponseModel  {
  id!: string;
  orderId!: string;
  orderName!: string;
  createdAt!: string;
  productId!: string;
  productName!: string;
  protocol!: string;
  quantity!: number;
  orderFulfilledDate?: string | null;
  productRefillDate?: string | null;
}
