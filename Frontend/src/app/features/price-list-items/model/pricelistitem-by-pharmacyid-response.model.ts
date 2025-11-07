export class PriceListItemsByPharmacyIdResponseDto
{
  id?: string;
  productId!: string;
  productPharmacyPriceListItemId!: string;
  productName!: string;
  amount!: number;
  status!: string;
  isColdStorageProduct! : boolean;
  protocol! : string;
  productType! : string;
}

