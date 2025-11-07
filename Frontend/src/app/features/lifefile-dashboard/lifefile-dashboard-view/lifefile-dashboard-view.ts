import { ChangeDetectorRef, Component, OnInit, AfterViewInit, ViewChild, Input, signal } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { LifeFileDashboardService } from '../lifefile-dashboard-services';
import { MatDialog } from '@angular/material/dialog';
import { ApiErrorDialogComponent } from '../api-error-dialog/api-error-dialog';
import { ApiTransactionResponseDto } from '../model/api-transaction-response.model';
import { OrderService } from '../../order-management/order-service';

interface LifeFileApiTracking {
  orderId: string;
  orderName: string;
  pharmacyName: string;
  integrationType: string;
  status: string;
  transactions: ApiTransactionResponseDto[];
}

@Component({
  selector: 'app-lifefile-dashboard-view',
  imports: [MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule, CommonModule],
  templateUrl: './lifefile-dashboard-view.html',
  styleUrl: './lifefile-dashboard-view.scss'
})
export class LifefileDashboardViewComponent {
  dataSource = new MatTableDataSource<LifeFileApiTracking>([]);
  displayedColumns: string[] = ['orderName',
    'pharmacyName',
    'integrationType',
    'orderStatus',
    'actions'];
  isLoading = signal(false);
  searchKeyword = '';
  pagedData: LifeFileApiTracking[] = [];
  pageSize = 5;
  pageIndex = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private router: Router,
    private confirmationDialog: ConfirmationDialogService,
    private notificationService: NotificationService,
    private lifefileDashboardService: LifeFileDashboardService,
    private orderService: OrderService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    this.isLoading.set(true);
    try {
      const data = await firstValueFrom(
        this.lifefileDashboardService.getAllOrderprocessingApiTrackingErrors()
      );
      this.dataSource.data = data;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    } catch (error) {
      this.notificationService.showSnackBar('Failed to load data', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
    if (!this.dataSource.filter) {
      this.dataSource.filter = '';
    }
    this.resetPaginator();
  }

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length ? this.dataSource.filteredData : this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }

  navigateToEdit(orderId: string): void {
    this.router.navigate(['/order/edit', orderId]);
  }

  openViewDialog(element: any): void {
    console.log('element, ', element);
    this.dialog.open(ApiErrorDialogComponent, {
      width: '800px',
      data: { transactions: element.transactions }
    });
  }

  async onStartLifeFile(orderId: string): Promise<void> {
    try {
      const res = await firstValueFrom(
        this.orderService.markReadyToLifeFile(orderId)
      );
      if (res.isSuccess) {
        this.notificationService.showSnackBar(res.message, 'success');
      }
      else {
        this.notificationService.showSnackBar(res.message, 'failure');
      }

    } catch (error) {
      this.notificationService.showSnackBar(
        'Failed to mark ready for LifeFile.',
        'failure'
      );
    }
  }

  confirmStartLifeFile(orderId: string): void {
    this.confirmationDialog.openConfirmation({
      title: 'Confirm Action',
      message: 'Are you sure you want to start life file?',
      confirmButtonText: 'Yes',
      cancelButtonText: 'No',
      showCancelButton: true
    }).subscribe(async (confirmed) => {
      if (confirmed) {
        await this.onStartLifeFile(orderId);
        this.loadData(); // refresh the list after success
      }
    });
  }
}
