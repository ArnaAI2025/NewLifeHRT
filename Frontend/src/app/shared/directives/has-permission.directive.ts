import {
  Directive,
  Input,
  TemplateRef,
  ViewContainerRef,
  inject,
  OnInit
} from '@angular/core';
import { PermissionService } from '../services/permission.service';
import { PermissionResource, PermissionAction } from '../../shared/constants/permissions.enums';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit {
  private templateRef = inject(TemplateRef<any>);
  private viewContainer = inject(ViewContainerRef);
  private permissionService = inject(PermissionService);

  private resource: PermissionResource | undefined;
  private action: PermissionAction | undefined;

  @Input('hasPermission')
  set setPermissionInput(value: [PermissionResource?, PermissionAction?]) {
    if (Array.isArray(value) && value.length === 2) {
      this.resource = value[0];
      this.action = value[1];
    }
  }

  ngOnInit(): void {
    const hasAccess = !this.resource || !this.action || this.permissionService.hasPermission(this.resource, this.action);
    if (hasAccess) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
