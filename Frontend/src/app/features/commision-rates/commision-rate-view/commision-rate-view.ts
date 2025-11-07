import { Component, computed, ViewChild } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { Router } from '@angular/router';
import { CommisionRateTableComponent } from '../commision-rate-table/commision-rate-table';


@Component({
  selector: 'app-commision-rate-view',
  imports: [CommisionRateTableComponent, MatTableModule, FormsModule, MatButtonModule, MatMenuModule, MatIconModule, MatCheckboxModule, MatProgressSpinnerModule, MatInputModule, MatSortModule, MatPaginatorModule, CommonModule],
  templateUrl: './commision-rate-view.html',
  styleUrl: './commision-rate-view.scss'
})
export class CommisionRateViewComponent {
  @ViewChild('commisionRateTable') commisionRateComponent!: CommisionRateTableComponent;
  searchKeyword = '';

  constructor(
    private router: Router,
  ) { }

  get selectedCount(): number {
    return this.commisionRateComponent?.selectedCount ?? 0;
  }

  // 🔹 Angular will re-run computed only when its dependencies change
  isLoading = computed(() => this.commisionRateComponent?.isLoading() ?? false);

  // 🔹 Proxy Method for hasSelectedRows
  hasSelectedRows(): boolean {
    return this.commisionRateComponent?.hasSelectedRows?.() ?? false;
  }

  addCommisionRate() {
    this.router.navigate(['/commissionrate/add']);
  }

  applyFilter() {
    this.commisionRateComponent.searchKeyword = this.searchKeyword;
    this.commisionRateComponent.applyFilter();
  }

  triggerBulkAction(action: 'activate' | 'deactivate' | 'delete') {
    if (this.commisionRateComponent) {
      this.commisionRateComponent.handleAction(action, true);
    }
  }

}
