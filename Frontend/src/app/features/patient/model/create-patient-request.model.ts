import { Address } from "../../../shared/models/address.model";
import { CreditCardDto } from "./credit-card.model";

export interface CreatePatientRequestDto {
  visitTypeId?: number | null;
  splitCommission?: boolean | null;
  patientGoal?: string | null;
  patientNumber: string;
  referralId?: string | null;
  agendaId? : number | null;
  firstName: string;
  lastName: string;
  gender: number;
  phoneNumber?: string | null;
  email: string;
  dateOfBirth?: string | null;
  drivingLicence?: string | null;
  addressId?: string | null;
  assignPhysicianId?: number | null;
  counselorId?: number | null;
  previousCounselorId?: number | null;
  allergies: string;
  status: boolean;
  isAllowMail?: boolean | null;
  labRenewableAlertDate?: string | null;
  address?: Address | null;
  patientCreditCards?: CreditCardDto[] | null;
  outstandingRefundBalance: number;
}
