export class CreateProductRequestDto {
  productID!: string;
  name!: string;
  isLabCorp?: boolean;
  isColdStorageProduct? : boolean;
  labCode?: string | null;
  parentId?: string | null;
  typeId?: number | null;
  category1Id?: number | null;
  category2Id?: number | null;
  category3Id?: number | null;
  productDescription?: string | null;
  protocol?: string | null;
  isScheduled?: boolean | null;
  webProductName?: string | null;
  webProductDescription?: string | null;
  isWebPopularMedicine?: boolean | null;
  webFormId?: number | null;
  webStrength?: string | null;
  webCost?: string | null;
  isEnabledCalculator?: boolean | null;
  isNewEnabledCalculator?: boolean | null;
  isPBPEnabled?: boolean | null;
}
