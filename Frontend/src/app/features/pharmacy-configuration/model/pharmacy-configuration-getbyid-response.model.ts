export class PharmacyConfigurationGetByIdResponseModel {
  pharmacyId!: string;
  typeId!: number;
  status!: string;
  configData!: { keyId: number; value: string }[];
}
