export interface CommissionsPayableResponse {
  poolId: string;
  commissionsPayableId: string;
  commissionsPayableDetailId: string;
  commissionsPayableAmount: number;
  patientName: string;
  pharmacyName: string;
  totalSales: number;
  entryType: string | null;
  isActive: boolean | null;
}
