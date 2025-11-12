import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private snackBar: MatSnackBar) {}

 showSnackBar(
  message: string,
  type: 'success' | 'failure' | 'normal' = 'normal',
  action?: string | null,
  duration?: number
): void {
  let panelClass: string;
  switch (type) {
    case 'success':
      panelClass = 'green-snackbar';
      break;
    case 'failure':
      panelClass = 'red-snackbar';
      break;
    default:
      panelClass = 'yellow-snackbar';
      break;
  }

  const displayDuration = duration ?? 3500;
  const actionToUse = action ?? 'Close';

  this.snackBar.open(message, actionToUse, {
    duration: displayDuration,
    panelClass: [panelClass],
    horizontalPosition: 'right',
    verticalPosition: 'top',
  });
}


}
