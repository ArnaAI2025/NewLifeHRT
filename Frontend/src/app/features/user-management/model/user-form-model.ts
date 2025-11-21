import { Address } from "../../../shared/models/address.model";

export interface UserFormModel {
  userName: string;
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  phoneNumber: string;
  roleIds: number[];
  address: Address;
  dea: string;
  npi: string;
  commisionInPercentage: number | null;
  signatureUrl?: string;
  matchAsCommisionRate: boolean;
  replaceCommisionRate: string;
  isVacationApplicable?: boolean;
  serviceIds: string[];
  timezoneId?: number | null;
  color?: string | null;
  licenseInformations : any[];
}
