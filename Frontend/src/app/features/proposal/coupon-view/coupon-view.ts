import { SelectionModel } from "@angular/cdk/collections";
import { CommonModule } from "@angular/common";
import { Component, OnInit, AfterViewInit, ViewChild, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatMenuModule } from "@angular/material/menu";
import { MatPaginatorModule, MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSortModule, MatSort } from "@angular/material/sort";
import { MatTableModule, MatTableDataSource } from "@angular/material/table";
import { Router } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { ConfirmationDialogService } from "../../../shared/components/confirmation-dialog/confirmation-dialog.services";
import { NotificationService } from "../../../shared/services/notification.service";
import { CouponResponse } from "../model/coupon-response.model";
import { ProposalService } from "../proposal.service";

@Component({
  selector: 'app-coupon-view',
  templateUrl: './coupon-view.html',
  styleUrl: './coupon-view.scss',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatFormFieldModule, MatInputModule, MatIconModule,
    MatButtonModule, MatCheckboxModule, MatPaginatorModule,
    MatSortModule, MatMenuModule, MatTableModule
  ]
})
export class CouponView implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns = [
    'select', 'couponName', 'counselorName', 'expiryDate', 'amount', 'percentage', 'isActive', 'actions'
  ];

  dataSource = new MatTableDataSource<CouponResponse>([]);
  selection = new SelectionModel<CouponResponse>(true, []);
  selectedRow = signal<CouponResponse | null>(null);
  searchKeyword = '';
  isLoading = signal(false);
  pagedData: CouponResponse[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private proposalService: ProposalService,
    private notificationService: NotificationService,
    private confirmationDialog: ConfirmationDialogService,
    private router: Router
  ) {}

  ngOnInit() { this.loadCoupons(); }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (data, filter) => {
      const val = filter.toLowerCase();
      return (data.couponName?.toLowerCase().includes(val) ||
              data.counselorName?.toLowerCase().includes(val) ||
              (data.isActive ? 'active' : 'inactive').includes(val));
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });
    this.updatePagedData();
  }

  async loadCoupons(): Promise<void> {
    this.isLoading.set(true);
    try {
      const coupons = await firstValueFrom(this.proposalService.getAllCoupon());
      this.dataSource.data = coupons || [];
      this.updatePagedData();
    } catch {
      this.notificationService.showSnackBar('Failed to load coupons', 'failure');
    } finally {
      this.isLoading.set(false);
    }
  }

  applyFilter() {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.selection.clear();
    this.resetPaginator();
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    return numSelected === this.dataSource.filteredData.length;
  }
  masterToggle() { this.isAllSelected() ? this.selection.clear() : this.dataSource.filteredData.forEach(r => this.selection.select(r)); }
  toggleRowSelection(row: CouponResponse) { this.selection.toggle(row); }
  isSelected(row: CouponResponse) { return this.selection.isSelected(row); }
  getSelectedCount() { return this.selection.selected.length; }

  onAddCoupon() { this.router.navigate(['/coupon/add']); }
  onEdit(row: CouponResponse | null){
    if(row == null) return;
    this.router.navigate(['/coupon/edit', row.id]); 
  }

  onDelete(row: CouponResponse | null) {
    if(row == null) return;
    this.confirmationDialog.openConfirmation({
      title: 'Delete Coupon',
      message: 'Are you sure you want to delete this coupon?'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.proposalService.deleteCoupon([row.id!]).subscribe(() => {
          this.notificationService.showSnackBar('Deleted successfully', 'success');
          this.loadCoupons();
        });
      }
    });
  }

  onToggleActive(row: CouponResponse | null, active: boolean | null) {
    if (!row || active == null) return;

    this.proposalService.bulkToggleActive([row.id!], active).subscribe(() => {
      this.selectedRow.update(curr => curr && curr.id === row.id ? { ...curr, isActive: active } : curr);
      row.isActive = active;
      this.notificationService.showSnackBar(
        `Coupon ${active ? 'activated' : 'deactivated'} successfully`,
        'success'
      );
    });
  }

  onBulkToggleActive(active: boolean) {
    const ids = this.selection.selected.map(s => s.id!);
    this.proposalService.bulkToggleActive(ids, active).subscribe(() => {
      this.loadCoupons();
      this.selection.clear();
      this.notificationService.showSnackBar(`Selected coupons ${active ? 'activated' : 'deactivated'}`, 'success');
    });
  }
  onBulkDelete() {
    const ids = this.selection.selected.map(s => s.id!);
    this.proposalService.deleteCoupon(ids).subscribe(() => {
      this.loadCoupons();
      this.selection.clear();
      this.notificationService.showSnackBar('Selected coupons deleted', 'success');
    });
  }

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length? this.dataSource.filteredData: this.dataSource.data;
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedData = filtered.slice(startIndex, endIndex);
  }

  private resetPaginator(): void {
    this.paginator.firstPage();
    this.pageIndex = 0;
    this.updatePagedData();
  }
}
