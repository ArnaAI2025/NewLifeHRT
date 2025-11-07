import { Address } from "../../../shared/models/address.model";

export interface LeadDisplay {
  id: string;
  subject: string;
  firstName: string;
  lastName: string;
  fullName: string; 
  phoneNumber?: string;
  email?: string;
  dateOfBirth?: Date;
  gender?: string;
  highLevelOwner?: string;
  description?: string;
  tags?: string;
  isActive?: boolean;
  addressId?: string;
  address?: Address
  isQualified?: boolean;
  ownerId: number;
  ownerFullName?: string;
  createdAt? :  Date;
  createdBy : string;
  
} 
