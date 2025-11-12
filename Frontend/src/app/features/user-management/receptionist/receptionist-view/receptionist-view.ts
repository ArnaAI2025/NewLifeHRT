import { Component } from '@angular/core';
import { UserListViewComponent } from '../../user-list-view/user-list-view';
import { UserRole } from '../../../../shared/enums/user-role.enum';

@Component({
selector: 'app-receptionist-view',
  templateUrl: './receptionist-view.html',
  styleUrl: './receptionist-view.scss',
  imports: [
       UserListViewComponent
  ]
})
export class ReceptionistViewComponent  {

  public UserRole = UserRole;

}
