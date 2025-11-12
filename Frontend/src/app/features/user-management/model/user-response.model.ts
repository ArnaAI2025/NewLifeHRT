import { Address } from "../../../shared/models/address.model";

export interface UserResponseDto {
  id: number;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  roleIds: number[];
  dea?: string;
  npi?: string;
  commisionInPercentage?: number;
  matchAsCommisionRate?: boolean;
  replaceCommisionRate?: string;
  signatureUrl?: string;
  address?: Address;
  createdAt: string;
  serviceIds: string[];
  password : string;
  isVacationApplicable : boolean;
  isDeleted?: boolean;
  timezoneId?: number;
  color?: string;
  licenseInformation : any[];
}


