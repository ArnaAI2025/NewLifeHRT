export interface CommissionsPayableDetailResponse {
  counselorName: string;
  commissionPayStatus?: boolean | null;
  patientName?: string | null;
  weekSummary?: string | null;
  ordersName?: string | null;
  subTotalAmount?: number | null;
  shipping?: number | null;
  surcharge?: number | null;
  syringe?: number | null;
  commissionAppliedTotal?: number | null;
  commissionPayable?: number | null;
  discount?: number | null;
  totalAmount?: number | null;
  pharmacyName?: string | null;
  commissionCalculationDetails?: string | null;
  ctcCalculationDetails?: string | null;
  ctc?: number | null;
  ctcPlusCommission?: number | null;
  profitAmount?: number | null;
  netAmount?: number | null;
  poolDetailId?: string | null; 
  isPriceOverRidden? : boolean | null;
  isMissingProductPrice? : boolean | null;
}
