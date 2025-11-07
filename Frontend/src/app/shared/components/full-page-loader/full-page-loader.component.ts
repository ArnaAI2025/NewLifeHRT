import { Component, Input, signal } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-full-page-loader',
  standalone: true,
  imports: [NgIf, MatProgressSpinnerModule],
  template: `
    <div *ngIf="show()" class="fixed inset-0 z-50 flex items-center justify-center bg-white/80">
      <mat-progress-spinner
        color="primary"
        mode="indeterminate"
        diameter="60">
      </mat-progress-spinner>
    </div>
  `
})
export class FullPageLoaderComponent {
  show = signal(false);

  @Input({ required: true })
  set visible(value: boolean) {
    this.show.set(value);
  }
}
