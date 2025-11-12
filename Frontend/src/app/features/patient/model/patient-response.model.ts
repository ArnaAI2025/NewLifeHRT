import { Address } from "../../../shared/models/address.model";
import { PatientCreditCardResponseDto } from "./patient-credit-card-response";
export interface PatientResponseDto {
  id: string;
  visitTypeId?: number | null;
  splitCommission?: boolean | null;
  patientGoal?: string | null;
  patientNumber: string;
  referralId?: string | null;
  firstName: string;
  lastName: string;
  fullName: string;
  gender: number;
  phoneNumber?: string | null;
  email: string;
  visitTypeName ?: string | null;
  dateOfBirth?: string | null;
  drivingLicence?: string | null;
  addressId?: string | null;
  assignPhysicianId?: number | null;
  counselorId: number;
  previousCounselorId?: number | null;
  previousCounselorFullName: string | null;
  allergies: string;
  status: boolean;
  isAllowMail?: boolean | null;
  labRenewableAlertDate?: string | null;
  isActive: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string | null;
  updatedBy?: string | null;
  address?: Address | null;
  agendaIds?: number[];
  patientCreditCards?: PatientCreditCardResponseDto[];
  profileImageUrl?: string | null;
  outstandingRefundBalance: number;
}

