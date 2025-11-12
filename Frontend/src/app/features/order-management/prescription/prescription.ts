import { CommonModule } from '@angular/common';
import { Component, OnInit, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { OrderService } from '../order-service';
import { firstValueFrom } from 'rxjs';
import { OrderReceiptResponse } from '../model/order-receipt-response.model';

@Component({
  selector: 'app-prescription',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './prescription.html',
  styleUrls: ['./prescription.scss'],
})
export class Prescription implements OnInit {
  order = signal<OrderReceiptResponse | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: {
      orderId: string | null;
      isSigned: boolean;
      doctorInfo?: {
        name: string;
        clinic: string;
        address: string;
        phone: string;
        license?: string;
      }
    },
    private dialogRef: MatDialogRef<Prescription>,
    private orderService: OrderService,
    private snackBar: MatSnackBar
  ) {}

  async ngOnInit(): Promise<void> {
    if (!this.data?.orderId) {
      this.error.set('No order ID provided');
      this.isLoading.set(false);
      return;
    }

    try {
      const result = await firstValueFrom(
        this.orderService.getSignedPrescriptionOrderById(this.data.orderId, this.data.isSigned )
      );
      this.order.set(result); 
      this.error.set(null);
    } catch (err) {
      console.error('Failed to load prescription:', err);
      this.error.set('Failed to load prescription data');
      this.snackBar.open('Failed to load prescription data', 'Close', {
        duration: 5000,
        panelClass: ['error-snackbar']
      });
    } finally {
      this.isLoading.set(false);
    }
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onPrint(): void {
    window.print();
  }

  onDownload(): void {
    this.snackBar.open('Download functionality not implemented yet', 'Close', {
      duration: 3000
    });
  }

  getDoctorInfo() {
  const o = this.order();
  if (!o) {
    return {
      name: '-',
      clinic: '-',
      address: '-',
      phone: '-',
      license: '-'
    };
  }

  return {
    name: o.doctorName || '-',
    clinic: o.description || '-', 
    address: o.doctorShippingAddress?.addressLine1
      ? `${o.doctorShippingAddress.addressLine1}, ${o.doctorShippingAddress.city}, ${o.doctorShippingAddress.stateOrProvince} ${o.doctorShippingAddress.postalCode}`
      : '-',
    phone: o.phoneNumber || '-',  
    license: o.drivingLicence || '-' 
  };
}


  hasValidOrderDetails(): boolean {
    const o = this.order();
    return !!(o?.orderDetails && o.orderDetails.length > 0);
  }

  getProductDisplay(detail: any): string {
    if (detail?.protocol) return detail.protocol;
    if (detail?.productType) return detail.productType;
    return 'No instructions provided';
  }

  getTotalQuantity(): number {
    const o = this.order();
    if (!o?.orderDetails?.length) return 0;
    return o.orderDetails.reduce((total, item) => total + item.quantity, 0);
  }
}
