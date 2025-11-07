export class CommisionRatesGetAllResponseDto
{
  id!: string;
  productId!: string;
  productName!: string;
  fromAmount!: number;
  toAmount!: number;
  ratePercentage?: number;
  status!: string;
}
