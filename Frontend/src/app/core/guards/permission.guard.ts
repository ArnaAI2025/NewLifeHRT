import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate ,Router, UrlTree} from '@angular/router';
import { PermissionService } from '../../shared/services/permission.service';
import { RoutePermission } from '../../shared/models/route-permission';
import { AppSettingsService } from '../../shared/services/app-settings.service';

@Injectable({
  providedIn: 'root'
})

export class PermissionGuard implements CanActivate {

  constructor(private router: Router, private permissionService: PermissionService,private appSettingsService: AppSettingsService) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const routePermission = route.data as RoutePermission;
    const resource = routePermission.permissionResource;
    const action = routePermission.permissionAction;
    const isUserPatient = this.appSettingsService.isUserPatient();
    if (this.permissionService.hasPermission(resource, action) || isUserPatient) {
      return true;
    }
    else {
      this.router.navigate(['/unauthorized']);
      return false;
    }
  }
}
