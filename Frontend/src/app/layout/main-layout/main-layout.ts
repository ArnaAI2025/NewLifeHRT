import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { MatIconModule } from '@angular/material/icon';

import { SidebarComponent } from '../sidebar/sidebar';
import { UserAccount } from '../../shared/models/user-account.model';
import { UserAccountService } from '../../shared/services/user-account.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatIconModule,
    SidebarComponent
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent implements OnInit, AfterViewInit {
  isCollapsed = false;
  hasActiveRoute = false;
  userAccount: UserAccount | null = null;
  isMobile = false;
  showSettingsMenu = false;

  @ViewChild('contentWrapper') contentWrapper!: ElementRef<HTMLDivElement>;

  constructor(private router: Router, private readonly userAccountService: UserAccountService) { }

  ngOnInit() {
    this.hasActiveRoute = this.router.url !== '/';
    this.userAccount = this.userAccountService.getUserAccount();
    this.checkViewport();
  }

  ngAfterViewInit() {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => {
        this.scrollToTop();
      });
  }

  @HostListener('window:resize')
  onResize() {
    this.checkViewport();
  }

  private checkViewport() {
    const newIsMobile = window.innerWidth <= 1024;

    // if we just transitioned from desktop -> mobile, force collapse
    if (newIsMobile && !this.isMobile) {
      this.isCollapsed = true;
    }

    this.isMobile = newIsMobile;
  }

  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }

  toggleSettingsMenu(event: MouseEvent) {
    event.stopPropagation();
    this.showSettingsMenu = !this.showSettingsMenu;
  }

  logout(): void {
    this.router.navigate(['/logout']);
  }


  onContentClick() {
    if (this.showSettingsMenu) {
      this.showSettingsMenu = false;
    }
    if (this.isMobile && !this.isCollapsed) {
      this.isCollapsed = true;
    }
  }


  get userInitials(): string {
    if (!this.userAccount?.fullname) {
      return '';
    }

    const names = this.userAccount.fullname.trim().split(' ');
    if (names.length === 1) {
      return names[0].slice(0, 2).toUpperCase();
    }

    const first = names[0].charAt(0).toUpperCase();
    const last = names[names.length - 1].charAt(0).toUpperCase();
    return `${first}${last}`;
  }

  private scrollToTop(): void {
    if (this.contentWrapper) {
      this.contentWrapper.nativeElement.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
}
