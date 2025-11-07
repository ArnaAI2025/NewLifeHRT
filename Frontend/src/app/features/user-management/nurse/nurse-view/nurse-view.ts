import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListViewComponent } from '../../user-list-view/user-list-view'; 
import { UserRole } from '../../../../shared/enums/user-role.enum'; 

@Component({
selector: 'app-nurse-view',
  templateUrl: './nurse-view.html',
  styleUrl: './nurse-view.scss',
  standalone : true,
  imports: [
    UserListViewComponent
  ]
})
export class NurseViewComponent {
    public UserRole = UserRole; 
}
