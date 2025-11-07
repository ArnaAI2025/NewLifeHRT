import { PriceListItemsByPharmacyIdResponseDto } from "./pricelistitem-by-pharmacyid-response.model";

export interface ProductLineItemDto extends PriceListItemsByPharmacyIdResponseDto {
  quantity: number;
  finalAmount: number;
  perUnitPrice: number;
  togglePerUnitPrice: boolean;
}