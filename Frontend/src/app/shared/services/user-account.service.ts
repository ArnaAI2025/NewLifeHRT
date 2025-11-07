import { Injectable } from '@angular/core';
import { UserAccount } from '../models/user-account.model';
import {jwtDecode} from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class UserAccountService {
  private user: Readonly<UserAccount> | null = null;

  setUser(token: string): void {
    const decoded: any = jwtDecode(token);

    const user: UserAccount = {
      id: Number(decoded.nameid),
      email: decoded.email,
      fullname: decoded.fullname,
      tenant: decoded.tenant,
      role: decoded.role,
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
