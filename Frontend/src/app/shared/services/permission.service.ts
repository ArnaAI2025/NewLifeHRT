import { Injectable } from '@angular/core';
import { UserAccountService } from '../services/user-account.service';
import { PermissionAction, PermissionResource } from '../constants/permissions.enums';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private readonly userAccountService: UserAccountService) {}


  hasPermission(resource: PermissionResource, action: PermissionAction): boolean {
    const permissionKey = `${resource}.${action}`;
    const user = this.userAccountService.getUserAccount();

    if (!user) return false;
    return user.permissions?.includes(permissionKey) ?? false;
  }

  hasAnyPermission(permissionKeys: string[]): boolean {
    const user = this.userAccountService.getUserAccount();

    if (!user || !user.permissions?.length) return false;
    return permissionKeys.some(key => user.permissions.includes(key));
  }

  getPermissions(): string[] {
    return this.userAccountService.getUserAccount()?.permissions ?? [];
  }
}
