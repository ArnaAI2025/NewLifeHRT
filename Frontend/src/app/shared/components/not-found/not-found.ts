import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterModule, MatButtonModule],
  templateUrl: './not-found.html',
  styleUrl: './not-found.scss'
})

export class NotFoundComponent {
  private router: Router = inject(Router);

  constructor() {}

  goToHomePage() {
    this.router.navigate(['/']);
  }
}
