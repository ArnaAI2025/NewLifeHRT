import { ProposalDetailRequestDto } from "./proposal-detail-request.model";

export interface ProposalRequestDto {
  id?: string;
  name: string;
  patientId: string;
  pharmacyId: string;
  counselorId: number;
  phyisianId : number;
  couponId?: string;
  patientCreditCardId?: string;
  pharmacyShippingMethodId: string | null;
  shippingAddressId?: string;
  therapyExpiration?: Date;
  subtotal: number;
  couponDiscount? : number;
  totalAmount: number;
  surcharge?: number;
  deliveryCharge?: number;
  status: number;
  description?: string;
  proposalDetails: ProposalDetailRequestDto[];
  isAddressVerified? : boolean;
}

