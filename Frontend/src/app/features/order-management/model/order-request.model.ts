export interface OrderDetailRequestDto {
  productId: string;
  quantity: number;
  amount: number;
  protocol : string;
  perUnitAmount: number;
  totalAmount: number;
}

export interface OrderRequestDto {
  name: string;
  patientId: string;
  proposalId?: string | null;
  pharmacyId: string;
  physicianId?: number | null;
  counselorId: number;
  couponId?: string | null;
  patientCreditCardId?: string | null;
  pharmacyShippingMethodId: string;
  shippingAddressId?: string | null;
  therapyExpiration?: Date | null;
  lastOfficeVisit?: Date | null;
  status?: number | null;
  subtotal: number;
  totalAmount: number;
  surcharge?: number | null;
  couponDiscount?: number | null;
  commission?: number | null;
  totalOnCommissionApplied?: number | null;
  deliveryCharge?: number | null;
  isOrderPaid?: boolean | null;
  isCashPayment?: boolean | null;
  isGenrateCommision?: boolean | null;
  isReadyForLifeFile?: boolean | null;
  rejectionResaon?: string | null;
  description?: string | null;
  orderPaidDate?: Date | null;
  orderFulFilled?: Date | null;
  signed? : boolean;
  courierServiceId? : number | null;
  orderDetails: OrderDetailRequestDto[];
  pharmacyOrderNumber? : string | null;
}