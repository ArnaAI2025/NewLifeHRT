export interface ShippingAddressResponseDto {
  addressLine1?: string;
  addressType?:string;
  countryId? : number;
  city?: string;
  stateOrProvince?: string;
  isDefaultAddress? : boolean;
  postalCode?: string;
  country?: string;
  stateId?: number;
  stateName?: string;
  countryName?: string;
  id: string;         
  patientId: string;  
  isActive : boolean;
}
