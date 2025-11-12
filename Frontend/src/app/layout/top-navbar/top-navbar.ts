import { Component, EventEmitter, inject, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterModule } from '@angular/router';
import { UserAccountService } from '../../shared/services/user-account.service';
import { UserAccount } from '../../shared/models/user-account.model';

@Component({
  selector: 'app-top-navbar',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    RouterModule
  ],
  templateUrl: './top-navbar.html',
  styleUrls: ['./top-navbar.scss']
})
export class TopNavbarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  private userAccountService = inject(UserAccountService);
  userAccount! : UserAccount;
  private router = inject(Router);

  onToggleSidebar() {
    this.toggleSidebar.emit();
  }

  async fetchUserAccount(): Promise<void> {
    const user = this.userAccountService.getUserAccount();
    if (!user) {
      await this.router.navigate(['/login']);
      return;
    }
    this.userAccount = user;
  }

  ngOnInit() {
    this.fetchUserAccount();
  }

  get initials(): string {
    if (!this.userAccount || !this.userAccount.fullname) return '';
    const names = this.userAccount.fullname.trim().split(' ');
    if (names.length === 1) return names[0].slice(0, 2).toUpperCase();
    const first = names[0].slice(0, 1).toUpperCase();
    const last = names[names.length - 1].slice(0, 1).toUpperCase();
    return first + last;
  }
}

