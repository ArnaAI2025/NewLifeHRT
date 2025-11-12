import { PharmacyShippingMethodDto } from "./shipping-methods.model";

export class PharmacyCreateRequestDto {
  name!: string;
  startDate?: string | null;
  endDate?: string | null;
  currencyId!: number;
  isLab! : boolean;
  description?: string | null;
  shippingMethods? : PharmacyShippingMethodDto;
}
