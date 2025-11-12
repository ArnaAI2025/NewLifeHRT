import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-api-error-dialog',
  standalone: true,   // ✅ important
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './api-error-dialog.html',
  styleUrls: ['./api-error-dialog.scss']
})
export class ApiErrorDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ApiErrorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { transactions: any[] }
  ) {}

  closeDialog(): void {
    this.dialogRef.close();
  }
}
