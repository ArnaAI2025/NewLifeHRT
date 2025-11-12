export class CommisionRateGetByIdResponseDto {
  id!: string;
  fromAmount!: number;
  toAmount!: number;
  ratePercentage?: number;
  productId!: string;
  status!: string;
}
