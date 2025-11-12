import { Component, OnInit, inject, DestroyRef, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { map, filter, switchMap, finalize } from 'rxjs/operators';

import { CommissionService } from '../commission-service';
import { CommissionsPayableDetailResponse } from '../model/commissions-payable-detail-response.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-counselor-commission-detail-view',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './counselor-commission-detail-view.html',
  styleUrls: ['./counselor-commission-detail-view.scss']
})
export class CounselorCommissionDetailView implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private commissionService = inject(CommissionService);
  private destroyRef = inject(DestroyRef);
  private cdr = inject(ChangeDetectorRef);

  isLoading = false;
  detail?: CommissionsPayableDetailResponse;

  // Read-only flag (toggle based on permissions/route)
  readOnlyMode = true;

  ngOnInit(): void {
    this.isLoading = true;

    this.route.paramMap
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(pm => pm.get('commissionsPayableId')),
        filter((id): id is string => !!id),
        switchMap(id => this.commissionService.getCommissionDetailById(id)),
        finalize(() => { this.isLoading = false; })
      )
      .subscribe({
        next: (res: any) => {
          this.detail = {
            counselorName: res?.counselorName ?? '',
            commissionPayStatus: res?.commissionPayStatus ?? null,
            weekSummary: res?.weekSummary ?? '',
            patientName: res?.patientName ?? '',
            ordersName: res?.ordersName ?? '',
            subTotalAmount: res?.subTotalAmount ?? null,
            shipping: res?.shipping ?? null,
            surcharge: res?.surcharge ?? null,
            syringe: res?.syringe ?? null,
            commissionAppliedTotal: res?.commissionAppliedTotal ?? null,
            commissionPayable: res?.commissionPayable ?? null,
            isPriceOverRidden: res?.isPriceOverRidden ?? null,
            discount: res?.discount ?? null,
            totalAmount: res?.totalAmount ?? null,
            pharmacyName: res?.pharmacyName ?? '',
            commissionCalculationDetails: res?.commissionCalculationDetails ?? '',
            ctcPlusCommission: res?.ctcPlusCommission ?? null,
            ctcCalculationDetails: res?.ctcCalculationDetails ?? '',
            ctc: res?.ctc ?? null,
            profitAmount: res?.profitAmount ?? null,
            netAmount: res?.netAmount ?? null,
            poolDetailId: res?.poolDetailId ?? null,
            isMissingProductPrice: res?.isMissingProductPrice ?? null
          };
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Failed to load commission detail', err);
        }
      });
  }

  back = (): void => {
    const id = (this.detail as any)?.poolDetailId;
    if (!id) return;
    this.router.navigate(['/counselor-order-wise-commissions/view', id]);
  };

  // Helpers for template
  fmtMoney(v?: number | null): string {
    return v == null ? '-' : v.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  fmtBool(v?: boolean | null): string {
    return v == null ? '-' : v ? 'Yes' : 'No';
  }

  abs(v?: number | null): number | null {
    return v == null ? null : Math.abs(v);
  }

  get profitOrLossLabel(): string {
    const v = this.detail?.profitAmount;
    return v == null ? '-' : v > 0 ? 'Profit' : v < 0 ? 'Loss' : 'Break-even';
  }

  get profitOrLossAmount(): string {
    const v = this.detail?.profitAmount;
    return v == null ? '-' : this.fmtMoney(Math.abs(v));
  }
  get isPriceOverriddenLabel(): string {
  const v = this.detail?.isPriceOverRidden;
  return v == null ? '-' : v ? 'Yes' : 'No';
}
get isMissingPriceLabel(): string {
  const v = this.detail?.isMissingProductPrice;
  return v == null ? 'No' : v ? 'Yes' : 'No';
}

}
