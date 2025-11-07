import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { OrderService } from '../order-service';
import { firstValueFrom } from 'rxjs';
import { OrderReceiptResponse } from '../model/order-receipt-response.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'order-receipt',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  templateUrl: './order-receipt.html',
  styleUrls: ['./order-receipt.scss'],
})
export class OrderReceipt implements OnInit {
  
constructor(
  @Inject(MAT_DIALOG_DATA) public data: { orderId: string | null, isPaidLogo: boolean },
  private dialogRef: MatDialogRef<OrderReceipt>
) {}


  private readonly orderService = inject(OrderService);

  order = signal<OrderReceiptResponse | null>(null);
  isLoading = signal(true);

  async ngOnInit(): Promise<void> {
    if (!this.data?.orderId) {
      this.isLoading.set(false);
      return;
    }

    try {
      const result = await firstValueFrom(
        this.orderService.getReceiptById(this.data.orderId)
      );
      this.order.set(result);
    } catch (err) {
      console.error('Failed to load receipt:', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  onClose(): void {
    this.dialogRef.close();   
  }
}
