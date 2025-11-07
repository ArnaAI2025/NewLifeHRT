import { PharmacyShippingMethodDto } from "./shipping-methods.model";


export class PharmacyGetByIdResponseDto {
  id!: string;
  name!: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  currencyId!: number;
  isLab!: boolean;
  hasFixedCommission!: boolean;
  commissionPercentage!: number;
  isActive!: boolean;
  shippingMethods: PharmacyShippingMethodDto[] = []; 

  constructor(init?: Partial<PharmacyGetByIdResponseDto>) {
    Object.assign(this, init);
  }
}
