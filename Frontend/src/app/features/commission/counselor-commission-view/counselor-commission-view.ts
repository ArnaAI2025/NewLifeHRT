import { Component, OnInit, AfterViewInit, ViewChild, signal, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs/operators';

import { CommissionsPayableResponse } from '../model/commissions-payable-response.model';
import { CommissionService } from '../commission-service';

@Component({
  selector: 'app-counselor-commission-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatMenuModule
  ],
  templateUrl: './counselor-commission-view.html',
  styleUrls: ['./counselor-commission-view.scss']
})
export class CounselorCommissionView implements OnInit, AfterViewInit {
  private readonly route = inject(ActivatedRoute);
  private readonly commissionService = inject(CommissionService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  poolDetailId?: string;

  isLoading = signal(false);
  filterKeyword = '';
  pageSize = 10;
  pageIndex = 0;

  dataSource = new MatTableDataSource<CommissionsPayableResponse>([]);
  pagedData: CommissionsPayableResponse[] = [];

  displayedColumns: string[] = [
    'patientName',
    'pharmacyName',
    'totalSales',
    'commissionsPayableAmount',
    'entryType',
    'isActive',
    'actions'
  ];

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(pm => pm.get('poolDetailId')),
        filter((id): id is string => !!id)
      )
      .subscribe(id => {
        this.poolDetailId = id;
        this.loadData();
      });
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (data: CommissionsPayableResponse, filter: string) => {
      if (!filter) return true;
      const keyword = filter.trim().toLowerCase();

      return (
        (data.patientName || '').toLowerCase().includes(keyword) ||
        (data.pharmacyName || '').toLowerCase().includes(keyword) ||
        (data.entryType || '').toLowerCase().includes(keyword) ||
        (data.commissionsPayableAmount?.toString() || '').includes(keyword) ||
        (data.totalSales?.toString() || '').includes(keyword)
      );
    };

    this.paginator.page
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((event: PageEvent) => {
        this.pageSize = event.pageSize;
        this.pageIndex = event.pageIndex;
        this.updatePagedData();
      });

    this.updatePagedData();
  }

  applyFilter(): void {
    this.dataSource.filter = this.filterKeyword.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
    this.updatePagedData();
  }

  private loadData(): void {
    if (!this.poolDetailId) {
      this.dataSource.data = [];
      this.updatePagedData();
      return;
    }

    this.isLoading.set(true);

    this.commissionService.getCommissionsByPoolDetailId(this.poolDetailId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res: CommissionsPayableResponse[]) => {
          this.dataSource.data = res ?? [];
          this.filterKeyword = '';
          this.updatePagedData();
        },
        error: (err) => {
          console.error('Failed to load counselor commissions', err);
        },
        complete: () => this.isLoading.set(false)
      });
  }

  private updatePagedData(): void {
    const data = this.dataSource.filteredData.length ? this.dataSource.filteredData : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    this.pagedData = data.slice(startIndex, startIndex + this.pageSize);
  }
    onEdit(element: CommissionsPayableResponse): void {
      if (!element?.commissionsPayableId) return;
      this.router.navigateByUrl(`/counselor-order-wise-detail/view/${element.commissionsPayableId}`);
    }
back = (): void => {
  this.router.navigate(['/commission-pool/view']);
};

}
