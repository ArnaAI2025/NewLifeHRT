export class PharmacyConfigurationRequestModel {
  pharmacyId!: string;
  typeId!: number;
  configData!: { keyId: number; value: string }[];
}
