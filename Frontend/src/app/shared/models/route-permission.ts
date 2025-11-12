import { PermissionAction, PermissionResource } from "../constants/permissions.enums";

export class RoutePermission {
  permissionResource: PermissionResource;
  permissionAction: PermissionAction;
  constructor(resource: PermissionResource, action: PermissionAction) {
    this.permissionResource = resource;
    this.permissionAction = action;
  }
}
