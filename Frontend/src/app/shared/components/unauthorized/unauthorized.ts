import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterModule, MatButtonModule],
  templateUrl: './unauthorized.html',
  styleUrls: ['./unauthorized.scss']
})
export class UnauthorizedComponent {

  goBack() {
    window.history.back();
  }
}
