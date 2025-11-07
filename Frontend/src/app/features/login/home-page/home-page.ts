import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})

export class HomePageComponent {
  private router: Router = inject(Router);

  constructor() {}

  onLoginClick() {
    this.router.navigate(['/login']);
  }
}
