export interface CouponResponse {
  id?: string; 
  couponName: string;
  expiryDate: string; 
  //counselorId: number;
  counselorName?: string;
  amount: number;
  percentage?: number | null;
  buget?: number | null;
  isActive? : boolean;
}
