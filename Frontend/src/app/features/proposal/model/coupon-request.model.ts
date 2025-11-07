export interface CouponRequest {
  couponName: string;
  expiryDate: string; 
 // counselorId: number;
  amount: number;
  percentage?: number | null;
  buget?: number | null;
}
