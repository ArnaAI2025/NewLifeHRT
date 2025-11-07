export class PharmacyConfigurationGetAllResponseDto {
  id!: string;
  pharmacyName!: string;
  typeName!: string;
  status?: string;
  modifiedOn!: Date;
  selected?: boolean;
}
