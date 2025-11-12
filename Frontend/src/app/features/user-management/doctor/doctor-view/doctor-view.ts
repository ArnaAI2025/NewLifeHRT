import { Component } from '@angular/core';

import {UserListViewComponent} from '../../user-list-view/user-list-view'
import { UserRole } from '../../../../shared/enums/user-role.enum';
@Component({
  selector: 'app-doctor-view',
  standalone: true,
  templateUrl: './doctor-view.html',
  styleUrl: './doctor-view.scss',
  imports: [

    UserListViewComponent
  ]
})
export class DoctorViewComponent  {
  public UserRole = UserRole;
}
