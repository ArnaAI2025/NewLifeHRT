import { Component, DestroyRef, inject, Inject, OnDestroy, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-session-expiry-dialog',
  standalone: true,
  imports: [MatDialogModule,MatButtonModule],
  templateUrl: './session-expiry-dialog.html',
  styleUrls: ['./session-expiry-dialog.scss']
})
export class SessionExpiryDialogComponent implements OnInit, OnDestroy {
  countdown = signal<number>(0);
  private timerId?: number;
  private destroyRef = inject(DestroyRef);

  constructor(
    private dialogRef: MatDialogRef<SessionExpiryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { countdown: number }
  ) {
    this.countdown.set(data.countdown);
  }

  ngOnInit() {
    this.timerId = setInterval(() => {
      this.countdown.update(val => {
        const next = val - 1;
        if (next <= 0) {
          this.logout();
          return 0;
        }
        return next;
      });
    }, 1000);

    this.destroyRef.onDestroy(() => clearInterval(this.timerId));
  }

  continue() {
    this.dialogRef.close('continue');
  }

  logout() {
    if (this.timerId) clearInterval(this.timerId);
    this.dialogRef.close('logout');
  }

  ngOnDestroy() {
    if (this.timerId) clearInterval(this.timerId);
  }
}
