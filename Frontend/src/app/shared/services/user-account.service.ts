import { Injectable } from '@angular/core';
import { UserAccount } from '../models/user-account.model';
import {jwtDecode} from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class UserAccountService {
  private user: Readonly<UserAccount> | null = null;

  setUser(token: string): void {
    const decoded: any = jwtDecode(token);

    const decodedRoles =
      decoded.role ??
      decoded.Role ??
      decoded.roles ??
      decoded.Roles ??
      [];
    const roles = Array.isArray(decodedRoles)
      ? decodedRoles
      : decodedRoles
        ? [decodedRoles]
        : [];
    const user: UserAccount = {
      id: Number(decoded.nameid),
      email: decoded.email,
      fullname: decoded.fullname,
      tenant: decoded.tenant,
      roles,
      permissions: decoded.permission ?? [],
      issuer: decoded.iss,
      audience: decoded.aud ?? [],
      jwtId: decoded.jti,
      issuedAt: decoded.iat,
      expiresAt: decoded.exp,
      notBefore: decoded.nbf
    };

    this.user = Object.freeze(user);
  }

  getUserAccount(): Readonly<UserAccount> | null {
    return this.user;
  }


  clear(): void {
    this.user = null;
  }



}
