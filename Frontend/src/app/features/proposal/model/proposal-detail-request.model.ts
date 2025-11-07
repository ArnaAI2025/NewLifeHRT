export interface ProposalDetailRequestDto {
  id?: string;
  productPharmacyPriceListItemId: string;
  productId: string;
  quantity: number;
  amount?: number;
  perUnitAmount: number;
  discount?: number;
  totalAmount?: number;
}