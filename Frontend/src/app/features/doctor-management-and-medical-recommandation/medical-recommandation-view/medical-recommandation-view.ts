import { Component, OnInit, ViewChild, AfterViewInit, signal } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { SelectionModel } from '@angular/cdk/collections';
import { ActivatedRoute, Router } from '@angular/router';


import { MedicalRecommendationResponseDto } from '../model/medical-recommendation-response.model';
import { DoctorManagementAndMedicalRecommandationService } from '../doctor-management-and-medical-recommandation.service';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { ConfirmationDialogData } from '../../../shared/components/confirmation-dialog/confirmation-dialog';
import { firstValueFrom } from 'rxjs';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';


@Component({
  selector: 'app-medical-recommandation-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    PatientNavigationBarComponent,
  ],
  templateUrl: './medical-recommandation-view.html',
  styleUrls: ['./medical-recommandation-view.scss'],
})
export class MedicalRecommandationView implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  activeRow: any = null;
  patientId: string | null = null;

  displayedColumns = [
    'sNo',
    'consultationDate',
    'medicationTypeName',
    'title',
    'actions',
  ];
  dataSource = new MatTableDataSource<MedicalRecommendationResponseDto>();

  searchKeyword = '';
  isLoading = signal(false);
  isDeleting = false;

  selection = new SelectionModel<MedicalRecommendationResponseDto>(true, []);
  selectedRecommendation: MedicalRecommendationResponseDto | null = null;
  isViewMode = false;
  pagedData: MedicalRecommendationResponseDto[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private router: Router,
    private doctorService: DoctorManagementAndMedicalRecommandationService,
    private userAccountService: UserAccountService,
    private notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationDialogService
  ) {}

  async ngOnInit(): Promise<void> {
    this.patientId = this.activatedRoute.snapshot.paramMap.get('patientId');
    // await this.ensureUserLoggedIn();
    this.loadRecommendations();
  }

  private async ensureUserLoggedIn(): Promise<void> {
    if (!this.userAccountService.getUserAccount()) {
      await this.router.navigate(['/login']);
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // Filter logic on title, medicationTypeName, consultationDate (as string)
    this.dataSource.filterPredicate = (
      data: MedicalRecommendationResponseDto,
      filter: string
    ) => {
      const lowerFilter = filter.toLowerCase();

      // Format consultationDate for filtering
      const formattedDate = new Date(data.consultationDate)
        .toLocaleDateString()
        .toLowerCase();

      return (
        (data.title?.toLowerCase() ?? '').includes(lowerFilter) ||
        (data.medicationTypeName?.toLowerCase() ?? '').includes(lowerFilter) ||
        formattedDate.includes(lowerFilter)
      );
    };
    this.paginator.page.subscribe((event: PageEvent) => {
      this.pageSize = event.pageSize;
      this.pageIndex = event.pageIndex;
      this.updatePagedData();
    });

    this.updatePagedData();
  }

  applyFilter(): void {
    this.dataSource.filter = this.searchKeyword.trim().toLowerCase();
    this.selection.clear();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
    if (!this.dataSource.filter) {
      this.dataSource.filter = '';
    }
    this.resetPaginator();
  }

  loadRecommendations(): void {
    if (!this.patientId) return;

    this.isLoading.set(true);
    this.selection.clear();

    this.doctorService
      .getAllMedicationTypeBasedOnPatientId(this.patientId)
      .subscribe({
        next: (res: MedicalRecommendationResponseDto[]) => {
          this.dataSource.data = res ?? [];
          this.dataSource.filter = '';
          this.updatePagedData();
        },
        error: (error) => {
          console.error('Failed to load recommendations:', error);
          this.notificationService.showSnackBar(
            'Failed to load recommendations.',
            'failure'
          );
        },
        complete: () => {
          this.isLoading.set(false);
        },
      });
  }
  async openConfirmation(action: string, message: string): Promise<boolean> {
    const data: ConfirmationDialogData = {
      title: `${action} Confirmation`,
      message: `<p>${message}</p>`,
    };

    const confirmed = await firstValueFrom(
      this.confirmationService.openConfirmation(data)
    );
    return confirmed ?? false;
  }
  async onDelete(
    recommendation: MedicalRecommendationResponseDto
  ): Promise<void> {
    const actionText = 'Delete';
    const patientLabel = `recommendation`;

    const confirmed = await this.openConfirmation(
      actionText,
      `Are you sure you want to <strong>${actionText.toLowerCase()}</strong> ${patientLabel}?`
    );

    if (!confirmed) return;

    this.isDeleting = true;
    this.doctorService
      .deleteMedicationRecommendation(recommendation.id)
      .subscribe({
        next: () => {
          this.notificationService.showSnackBar(
            'Recommendation deleted successfully.',
            'success'
          );
          this.loadRecommendations();
        },
        error: (err) => {
          console.error('Delete failed:', err);
          this.notificationService.showSnackBar(
            'Failed to delete recommendation.',
            'failure'
          );
        },
        complete: () => {
          this.isDeleting = false;
        },
      });
  }

  onAdd(): void {
    this.router.navigate(['/medication-recommendation-add', this.patientId]);
  }

  onEdit(recommendation: MedicalRecommendationResponseDto): void {
    if (!this.patientId) {
      return;
    }
    this.router.navigate([
      '/medication-recommendation-edit',
      this.patientId,
      recommendation.id,
    ]);
  }
  togglePatientActiveStatus(status: boolean): void {}
  onSaveAndClose(): void {}
  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }
  onSubmit(): void {}
  onClose(): void {
    this.router.navigate(['/patients/view']);
  }

  private updatePagedData(): void {
    const filtered = this.dataSource.filteredData?.length
      ? this.dataSource.filteredData
      : this.dataSource.data;
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
