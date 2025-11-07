import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '../../../shared/services/notification.service';
import { MedicalRecommendationResponseDto } from '../../doctor-management-and-medical-recommandation/model/medical-recommendation-response.model';
import { SelectionModel } from '@angular/cdk/collections';
import { AppointmentGetByPatientIdResponseDto } from '../model/appointment-get-by-patientId-response.model';
import { AppointmentService } from '../appointment.services';

@Component({
  selector: 'app-patient-appointment-view',
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
    PatientNavigationBarComponent
  ],
  templateUrl: './patient-appointment-view.html',
  styleUrl: './patient-appointment-view.scss'
})
export class PatientAppointmentViewComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  patientId: string | null = null;
  displayedColumns = ['doctorName', 'counselorName', 'serviceName', 'doctorStartDateTime', 'doctorEndDateTime', 'status', 'description'];
  dataSource = new MatTableDataSource<AppointmentGetByPatientIdResponseDto>();
  searchKeyword = '';
  isLoading = signal(false)
  selection = new SelectionModel<AppointmentGetByPatientIdResponseDto>(true, []);

  constructor(
    private router: Router,
    private notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationDialogService,
    private appointmentService: AppointmentService
  ) { }

  async ngOnInit(): Promise<void> {
    this.patientId = this.activatedRoute.snapshot.paramMap.get('patientId');
    if (this.patientId) {
      this.loadAppointments(this.patientId);
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (data: AppointmentGetByPatientIdResponseDto, filter: string) => {
      const term = filter.trim().toLowerCase();
      return (
        data.doctorName?.toLowerCase().includes(term) ||
        data.counselorName?.toLowerCase().includes(term) ||
        data.serviceName?.toLowerCase().includes(term) ||
        data.status?.toLowerCase().includes(term)
      );
    };
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
  }

  loadAppointments(patientId: string): void {
    this.isLoading.set(true);
    this.appointmentService.getAppointmentsByPatientId(patientId).subscribe({
      next: (appointments) => {
        this.dataSource = new MatTableDataSource(appointments);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.notificationService.showSnackBar('Failed to load appointments', 'failure');
      }
    });
  }

  togglePatientActiveStatus(status: boolean): void { }
  onSaveAndClose(): void { }

  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }
  onSubmit(): void { }
  onClose(): void {
    this.router.navigate(['/patients/view']);
  }

  onAdd(): void {
    this.router.navigate(['/appointment/add']);
  }

}
