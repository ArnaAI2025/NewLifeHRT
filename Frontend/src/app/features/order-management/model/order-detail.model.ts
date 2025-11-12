export interface OrderDetailResponseDto {
  id: string | null;          
  productId: string;   
  productPharmacyPriceListItemId: string;
  productName : string;
  isColdStorageProduct : boolean;
  protocol: string | null;
  quantity: number;
  perUnitAmount: number;
  amount?: number | null;
}