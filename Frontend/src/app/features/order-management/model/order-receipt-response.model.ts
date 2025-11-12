import { ShippingAddressResponseDto } from "../../patient/model/shipping-address-response.model";
import { OrderDetailResponseDto } from "./order-detail.model";
export interface OrderReceiptResponse {
  id: string;
  name?: string;
  dateOfBirth?: string;          
  phoneNumber?: string;
  lastOfficeVisit?: string;      
  drivingLicence?: string;
  allergies?: string;
  patientName?: string;
  doctorName?: string;
  description?: string;
  shippingMethodName?: string;
  shippingMethodAmount?: number;

  patientShippingAddress?: ShippingAddressResponseDto;
  doctorShippingAddress?: ShippingAddressResponseDto;

  signed?: boolean;
  subtotal: number;
  totalAmount: number;
  surcharge?: number;
  createdAt: string;             
  orderDetails: OrderDetailResponseDto[];
}
