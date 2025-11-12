export class CommisionRateRequestDto {
  productId!: string;
  fromAmount!: number;
  toAmount!: number;
  ratePercentage?: number;
}
