export class ProductStrengthResponseDto
{
  id!: string;
  productID!: string;
  name!: string;
  strengths?: string;
  price?: number;
  isActive!: boolean;
  modifiedOn!: Date;
}
