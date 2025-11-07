export class ProductFullResponseDto
{
  id!: string;
  productID!: string;
  name!: string;
  labCode?: string;
  labCorp?: boolean;
  isColdStorageProduct? : boolean;
  parentId?: string;
  parentName?: string;

  description?: string;
  protocol?: string;
  scheduled?: boolean;

  webProductName?: string;
  webProductDescription?: string;
  webPopularMedicine?: boolean;

  webFormId?: number;
  webFormName?: string;

  webStrengths?: string;
  webCost?: string;

  enableCalculator?: boolean;
  newEnableCalculator?: boolean;
  pbpEnable?: boolean;

  typeId?: number;
  typeName?: string;

  category1Id?: number;
  category1Name?: string;

  category2Id?: number;
  category2Name?: string;

  category3Id?: number;
  category3Name?: string;

  statusId!: number;
  statusName!: string;

  modifiedOn!: Date;
}
