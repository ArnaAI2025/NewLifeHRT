import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListViewComponent } from '../../user-list-view/user-list-view'; 
import { UserRole } from '../../../../shared/enums/user-role.enum'; 

@Component({
  selector: 'app-admin-view',
  standalone: true,
  templateUrl: './admin-view.html',
  styleUrls: ['./admin-view.scss'], 
  imports: [
    CommonModule,
    UserListViewComponent
  ]
})
export class AdminViewComponent {
  public UserRole = UserRole; 
}
