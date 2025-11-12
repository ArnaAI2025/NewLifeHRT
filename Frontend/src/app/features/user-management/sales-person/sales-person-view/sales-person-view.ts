import { Component } from '@angular/core';
import { UserListViewComponent } from '../../user-list-view/user-list-view';
import { UserRole } from '../../../../shared/enums/user-role.enum';


@Component({
selector: 'app-sales-person-view',
  templateUrl: './sales-person-view.html',
  styleUrl: './sales-person-view.scss',
  standalone : true,
  imports: [
    UserListViewComponent
  ]
})
export class SalesPersonViewComponent {
  public UserRole = UserRole;
  
}
