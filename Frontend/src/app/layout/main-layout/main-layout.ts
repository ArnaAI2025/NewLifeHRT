import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavContent, MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { TopNavbarComponent } from '../top-navbar/top-navbar';
import { SidebarComponent } from '../sidebar/sidebar';
import { filter } from 'rxjs';


@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    RouterModule,
    TopNavbarComponent,
    SidebarComponent
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent implements OnInit,AfterViewInit  {
  sidenavOpen = true;
  isCollapsed = false;
  hasActiveRoute = false;
  @ViewChild(MatSidenavContent) sidenavContent!: MatSidenavContent;

  constructor(private router: Router) {}

  ngOnInit() {
    this.hasActiveRoute = this.router.url !== '/';
  }

  ngAfterViewInit() {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => {
        this.sidenavContent.scrollTo({ top: 0, behavior: 'smooth' });
      });
  }

  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }
}
