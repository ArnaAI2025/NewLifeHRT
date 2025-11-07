import { ChangeDetectorRef, Component, Input, OnInit, signal, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { ProductStrengthService } from '../product-strength.services';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ProductStrengthAddComponent } from '../product-strength-add/product-strength-add';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';

interface ProductStrength {
  id: string;
  productId: string;
  name: string;
  strengths?: string;
  price?: number;
  isActive: boolean;
  modifiedOn: Date;
}


@Component({
  selector: 'app-product-strength-view',
  imports : [MatTableModule,MatButtonModule,MatIconModule,MatProgressSpinnerModule,MatInputModule, MatSortModule, CommonModule],
  templateUrl: './product-strength-view.html',
  styleUrl: './product-strength-view.scss'
})

export class ProductStrengthViewComponent implements OnInit {
  @Input() productId!: string;
  displayedColumns: string[] = ['name', 'strengths', 'modifiedOn', 'actions'];
  dataSource = new MatTableDataSource<ProductStrength>([]);
  isLoading = signal(false);

  @ViewChild(MatSort) sort!: MatSort;

   constructor(private productStrengthService: ProductStrengthService, private confirmationService: ConfirmationDialogService,private cdr: ChangeDetectorRef, private notificationService: NotificationService, private dialog: MatDialog) {}

   ngOnInit(): void {
    if (this.productId) {
      this.loadStrengths();
    }
  }

  async loadStrengths(): Promise<void> {
  this.isLoading.set(true);

  try {
    const responseDto = await firstValueFrom(
      this.productStrengthService.getAllStrengthsByProductId(this.productId)
    );

    const mappedProducts: ProductStrength[] = responseDto.map(dto => ({
      id: dto.id,
      productId: dto.productID,
      name: dto.name,
      strengths: dto.strengths,
      price: dto.price,
      isActive: dto.isActive,
      modifiedOn: new Date(dto.modifiedOn),
    }));

    this.dataSource.data = mappedProducts;
    this.dataSource.sort = this.sort;
  } catch (err) {
    console.error('Failed to load products strengths', err);
  } finally {
    this.isLoading.set(false);
    this.cdr.detectChanges();
  }
}

  navigateToAdd() : void
  {
    const dialogRef = this.dialog.open(ProductStrengthAddComponent, {
    width: '400px',
    data: { productId: this.productId }
  });

  dialogRef.afterClosed().subscribe((shouldReload: boolean) => {
    if (shouldReload) {
      this.loadStrengths();
    }
  });
  }

  async deleteStrength(id: string): Promise<void> {
  const confirmed = await this.openConfirmation('Delete');
  if (!confirmed) return;
  try {
    const deletedId = await firstValueFrom(
      this.productStrengthService.deleteProductStrength(id)
    );
    this.notificationService.showSnackBar('Product Strength Deleted Successfully', 'success');
    this.dataSource.data = this.dataSource.data.filter(ps => ps.id !== deletedId);
  } catch (err) {
    console.error('Failed to delete product strength:', err);
    this.notificationService.showSnackBar('Failed to delete product strength:', 'failure');
  }
}



  editStrength(productStrength: ProductStrength): void {
  const dialogRef = this.dialog.open(ProductStrengthAddComponent, {
    width: '400px',
    data: {
      productId: this.productId,
      productStrength: productStrength
    }
  });

  dialogRef.afterClosed().subscribe((shouldReload: boolean) => {
    if (shouldReload) {
      this.loadStrengths(); // Refresh the table
    }
  });
}

async openConfirmation(action: string): Promise<boolean> {
  const data: ConfirmationDialogData = {
    title: `${action} Confirmation`,
    message: `<p>Are you sure you want to <strong>${action.toLowerCase()}</strong> this product strength?</p>`,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No'
  };

  const confirmed = await firstValueFrom(this.confirmationService.openConfirmation(data));
  return confirmed ?? false;
}


}
