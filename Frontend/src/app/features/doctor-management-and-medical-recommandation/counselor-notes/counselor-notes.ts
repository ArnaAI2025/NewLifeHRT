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
import { CounselorNoteDisplay } from '../model/counselor-note-display.model';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { SelectionModel } from '@angular/cdk/collections';
import { NotificationService } from '../../../shared/services/notification.service';
import { DoctorManagementAndMedicalRecommandationService } from '../doctor-management-and-medical-recommandation.service';
import { CounselorNotesAddComponent } from '../counselor-notes-add/counselor-notes-add';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';

@Component({
  selector: 'app-counselor-notes',
  standalone: true,
  templateUrl: './counselor-notes.html',
  styleUrls: ['./counselor-notes.scss'],
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
})
export class CounselorNotesComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  patientId!: string;

  displayedColumns: string[] = [
    'sNo',
    'subject',
    'note',
    'isAdminMailSent',
    'isDoctorMailSent',
    'status',
    'actions',
  ];
  dataSource = new MatTableDataSource<CounselorNoteDisplay>();
  searchKeyword = '';
  isLoading = signal(false);
  isDeleting = false;
  isBulkProcessing = false;
  bulkAction: 'activate' | 'deactivate' | null = null;
  activeRow: CounselorNoteDisplay | null = null;
  selection = new SelectionModel<CounselorNoteDisplay>(true, []);
  selectedNoteForView: CounselorNoteDisplay | null = null;
  isViewMode = false;
  pagedData: CounselorNoteDisplay[] = [];
  pageSize = 5;
  pageIndex = 0;

  constructor(
    private router: Router,
    private readonly doctorManagementAndMedicalRecommandationService: DoctorManagementAndMedicalRecommandationService,
    private readonly userAccountService: UserAccountService,
    private readonly notificationService: NotificationService,
    private readonly activatedRoute: ActivatedRoute,
    private dialog: MatDialog,
    private confirmationService: ConfirmationDialogService
  ) {}

  async ngOnInit(): Promise<void> {
    this.patientId = this.activatedRoute.snapshot.paramMap.get('patientId')!;
    await this.userAccount();
    this.loadNotesData();
  }

  async userAccount(): Promise<void> {
    const user = this.userAccountService.getUserAccount();
    if (!user) {
      await this.router.navigate(['/login']);
      return;
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (
      data: CounselorNoteDisplay,
      filter: string
    ): boolean => {
      const value = filter.toLowerCase();
      return (
        data.subject.toLowerCase().includes(value) ||
        data.note.toLowerCase().includes(value) ||
        data.patientId.toLowerCase().includes(value)
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
    if (!this.dataSource.filter) {
      this.dataSource.filter = '';
    }
    this.resetPaginator();
    this.selection.clear();
  }

  formatNotesFromApi(data: any[]): CounselorNoteDisplay[] {
    return data.map((note) => ({
      id: note.id,
      patientId: note.patientId,
      subject: note.subject,
      note: note.note,
      isAdminMailSent: note.isAdminMailSent,
      isDoctorMailSent: note.isDoctorMailSent,
      isActive: note.isActive,
    }));
  }

  loadNotesData(): void {
    if (!this.patientId) {
      return;
    }
    this.isLoading.set(true);
    this.selection.clear();

    this.doctorManagementAndMedicalRecommandationService
      .getAllCounselorBasedOnPatientId(this.patientId)
      .subscribe({
        next: (apiResponse) => {
          this.dataSource.data = this.formatNotesFromApi(apiResponse);
          this.dataSource.filter = '';
          this.updatePagedData();
        },
        error: (err) => {
          console.error('Error:', err);
          this.notificationService.showSnackBar(
            'Failed to load counselor notes.',
            'failure'
          );
        },
        complete: () => {
          this.isLoading.set(false);
          this.isBulkProcessing = false;
          this.bulkAction = null;
        },
      });
  }

  onDelete(data: CounselorNoteDisplay): void {
    const subject = data.subject ?? '';
    this.confirmationService
      .openConfirmation({
        title: 'Delete Counselor Note',
        message: `Are you sure you want to <strong>delete</strong> this counselor note?`,
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel',
      })
      .subscribe((confirmed) => {
        if (!confirmed) return;

        this.isDeleting = true;

        this.doctorManagementAndMedicalRecommandationService
          .deleteCounselorNote(data.id)
          .subscribe({
            next: (response) => {
              if (response) {
                this.notificationService.showSnackBar(
                  'Counselor note deleted successfully',
                  'success'
                );
                this.loadNotesData();
                this.selection.clear?.(); // clear selection if present
              }
            },
            error: (err) => {
              console.error('Delete error:', err);
              const errorMessage =
                err?.error?.message || 'Failed to delete counselor note.';
              this.notificationService.showSnackBar(errorMessage, 'failure');
            },
            complete: () => {
              this.isDeleting = false;
            },
          });
      });
  }

  viewNote(data: CounselorNoteDisplay): void {
    this.openAddNoteDialog(this.patientId, data, true);
  }
  openAddNoteDialog(
    patientId: string | null,
    noteData: CounselorNoteDisplay | null,
    isViewMode: boolean
  ): void {
    const dialogRef = this.dialog.open(CounselorNotesAddComponent, {
      width: '500px',
      data: {
        patientId,
        noteData: noteData,
        isViewMode: isViewMode,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.loadNotesData();
    });
  }

  openAddNotePopup(): void {
    this.openAddNoteDialog(this.patientId, null, false);
  }
  togglePatientActiveStatus(status: boolean | any): void {}
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
