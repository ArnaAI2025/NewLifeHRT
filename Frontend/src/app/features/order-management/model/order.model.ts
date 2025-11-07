import { OrderDetailResponseDto } from "./order-detail.model";

export interface OrderResponseDto {
  id: string;
  name: string;
  patientId: string;
  pharmacyId: string;
  physicianId?: number | null;
  counselorId: number;
  couponId?: string | null;
  patientCreditCardId?: string | null;
  pharmacyShippingMethodId: string;
  shippingAddressId?: string | null;
  therapyExpiration?: string | null;
  lastOfficeVisit?: string | null;
  subtotal: number;
  totalAmount: number;
  surcharge?: number | null;
  couponDiscount?: number | null;
  commission?: number | null;
  totalOnCommissionApplied?: number | null;
  deliveryCharge?: number | null;
  isOrderPaid?: boolean | null;
  isCashPayment?: boolean | null;
  isGenrateCommission?: boolean | null;
  isReadyForLifeFile?: boolean | null;
  orderPaidDate?: string | null;
  orderFulfilled?: string | null;
  rejectionReason?: string | null;
  description?: string | null;
  orderStatus?: number | null;
  status? : number | null;
  createdAt: string;
  isPharmacyActive : boolean;
  orderDetails: OrderDetailResponseDto[];
  isActive? : boolean;
  refundAmount?: number | null;
}
