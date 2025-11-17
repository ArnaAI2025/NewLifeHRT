export interface UserAccount {
  id: number;
  email: string;
  fullname: string;
  tenant: string;
  role: string;
  permissions: string[];
  issuer: string;
  audience: string[];
  jwtId: string;
  issuedAt: number;
  expiresAt: number;
  notBefore: number;
}
