export class PharmacyGetAllResponseDto {
  id!: string;
  name!: string;
  startDate?: string;
  endDate?: string;
  currencyName!: string;
  isActive!: boolean
  constructor(init?: Partial<PharmacyGetAllResponseDto>) {
    Object.assign(this, init);
  }
}
