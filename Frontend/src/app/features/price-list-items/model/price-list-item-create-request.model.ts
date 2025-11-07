export class PriceListItemRequestDto {
  currencyId?: number;
  amount!: number;
  costOfProduct?: number;
  lifeFilePharmacyProductId?: string;
  lifeFielForeignPmsId?: string;
  lifeFileDrugFormId?: number;
  lifeFileDrugName?: string;
  lifeFileDrugStrength?: string;
  lifeFileQuantityUnitId?: number;
  lifeFileScheduledCodeId?: number;
  pharmacyId!: string;
  productId!: string;
}
