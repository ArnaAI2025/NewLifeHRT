import { Address } from "../../../shared/models/address.model";

export interface ShippingAddressRequestDto {
  id?: string | null;      
  patientId: string;       
  address: Address;     
}