export class OrderProductRefillGetByIdResponseModel {
  id!: string;
  productName: string = '';
  daysSupply?: number;
  doseAmount?: number;
  doseUnit?: string;
  frequencyPerDay?: number;
  bottleSizeML?: number;
  refillDate?: string;
  status?: string;
  assumption?: string;
}
