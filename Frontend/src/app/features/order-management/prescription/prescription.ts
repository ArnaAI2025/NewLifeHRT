import { CommonModule } from '@angular/common';
import { Component, OnInit, Inject, signal, ChangeDetectorRef } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { OrderService } from '../order-service';
import { DomSanitizer, SafeHtml, SafeResourceUrl } from '@angular/platform-browser';
import { FullPageLoaderComponent } from '../../../shared/components/full-page-loader/full-page-loader.component';
import { PrescriptionReceiptDto } from './prescription.model';

@Component({
  selector: 'app-prescription',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    FullPageLoaderComponent
  ],
  templateUrl: './prescription.html',
  styleUrls: ['./prescription.scss'],
})
export class Prescription implements OnInit {
   orderId!: string;
  isScheduleDrug?: boolean;
  safeHtml?: SafeHtml;
  pdfDataUrl?: SafeResourceUrl;
  dto?: PrescriptionReceiptDto;
  loading = signal(false);
  error?: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: {
      orderId: string | null;
      isSigned: boolean;
      isReceipt:boolean;
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
    private snackBar: MatSnackBar,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.fetch();
  }

   fetch() {
    this.loading.set(true);
    if(this.data.isReceipt){
      this.orderService.getReceiptById(this.data.orderId).subscribe({
        next: (dto) => {
          this.dto = dto;
          // sanitize HTML (server generated HTML considered trusted)
          if (dto.renderedHtml) {
            this.safeHtml = this.sanitizer.bypassSecurityTrustHtml(dto.renderedHtml);
          }
          
          // prepare PDF preview (data URL) - only if pdfBase64 exists
          if (dto.pdfBase64) {
            const dataUrl = `data:application/pdf;base64,${dto.pdfBase64}`;
            this.pdfDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(dataUrl);
          }
          
          this.loading.set(false);
          this.cdr.detectChanges();
        },
        error: (err:any) => {
          this.error = 'Failed to load template';
          this.loading.set(false);
          console.error(err);
        }
      });
    }else{
      this.orderService.getOrderTemplate(this.data.orderId, this.data.isSigned).subscribe({
        next: (dto) => {
          this.dto = dto;
          // sanitize HTML (server generated HTML considered trusted)
          if (dto.renderedHtml) {
            this.safeHtml = this.sanitizer.bypassSecurityTrustHtml(dto.renderedHtml);
          }
          
          // prepare PDF preview (data URL) - only if pdfBase64 exists
          if (dto.pdfBase64) {
            const dataUrl = `data:application/pdf;base64,${dto.pdfBase64}`;
            this.pdfDataUrl = this.sanitizer.bypassSecurityTrustResourceUrl(dataUrl);
          }
          
          this.loading.set(false);
          this.cdr.detectChanges();
        },
        error: (err:any) => {
          this.error = 'Failed to load template';
          this.loading.set(false);
          console.error(err);
        }
      });
    }
  }

  onClose(): void {
    this.dialogRef.close();
  }

  downloadFromBase64() {
    if (!this.dto?.pdfBase64) return;
    const byteString = atob(this.dto.pdfBase64);
    const ab = new ArrayBuffer(byteString.length);
    const ia = new Uint8Array(ab);
    for (let i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    const blob = new Blob([ab], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `order_${this.orderId}.pdf`;
    document.body.appendChild(a);
    a.click();
    a.remove();
    window.URL.revokeObjectURL(url);
  }

  // recommended: download from server endpoint (streams)
  downloadFromServer() {
    this.orderService.downloadOrderPdf(this.data.orderId, this.data.isSigned,this.data.isReceipt).subscribe({
      next: (blob:any) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `order_${this.data.orderId}.pdf`;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error(err);
        alert('Failed to download PDF');
      }
    });
  }

}
