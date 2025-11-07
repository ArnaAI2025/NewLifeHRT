import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs/operators';

import { PatientService } from '../../patient/patient.services';
import { PatientLeadCommunicationDto } from '../../patient/model/patient-lead-communication.response.model';
import { UserAccountService } from '../../../shared/services/user-account.service';
import { EmailAndSmsService } from '../email-sms.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { LeadManagementService } from '../../lead-management/lead-management.service';
import { BatchMessageResponseDto } from '../model/batch-message-response.model';
import { BatchMessageRequestDto } from '../model/batch-message-request.model';

@Component({
  selector: 'app-bulk-sms',
  templateUrl: './bulk-sms.html',
  styleUrls: ['./bulk-sms.scss'],
  imports: [FormsModule, CommonModule],
  standalone: true,
})
export class BulkSms implements OnInit {
  entity = signal('patient');
  method = signal<'email' | 'sms' | 'both'>('email');
  audience = signal<'all' | 'selected'>('all');
  userId = signal<number | null>(null);

  patients = signal<PatientLeadCommunicationDto[]>([]);
  filteredPatients = signal<PatientLeadCommunicationDto[]>([]);
  selectedIds = signal<Set<string>>(new Set());

  searchTerm = signal('');
  stateFilter = signal('');

  states = signal<string[]>([]);

  subject = signal('');
  emailHtml = signal('');
  sms = signal('');
  emailPlainLength = signal(0);

  emailFont = signal('');
  codeMode = signal(false);

  summaryText = signal('');
  allVisibleChecked = signal(false);
  isSubmitting = signal(false);

  // Added signals for edit mode
  isEditMode = signal(false);
  batchMessageId = signal<string | null>(null);
  originalMethod = signal<'email' | 'sms' | 'both'>('email');
  createdByUserName = signal('');
  recipients = signal<{ name: string }[]>([]);

  private readonly patientService = inject(PatientService);
  private readonly userAccountService = inject(UserAccountService);
  private readonly emailAndSmsService = inject(EmailAndSmsService);
  private readonly notificationService = inject(NotificationService);
  private readonly leadManagementService = inject(LeadManagementService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const user = this.userAccountService.getUserAccount();
    if (user) {
      this.userId.set(user.id);
    }

    // Detect edit mode based on route param
    this.route.paramMap.subscribe(params => {
      const batchMessageId = params.get('batchMessageId');
      if (batchMessageId) {
        this.isEditMode.set(true);
        this.batchMessageId.set(batchMessageId);
        this.fetchBatchMessageForEdit(batchMessageId);
      } else {
        this.loadPatients();
      }
    });
  }

  fetchBatchMessageForEdit(batchMessageId: string): void {
    this.emailAndSmsService.getBatchMessageById(batchMessageId).subscribe({
      next: (dto: BatchMessageResponseDto) => {
        this.subject.set(dto.subject ?? '');
        this.sms.set(dto.message);
        this.emailHtml.set(dto.message);

        // Determine communication method
        this.originalMethod.set(dto.isMail && dto.isSms ? 'both' : (dto.isMail ? 'email' : 'sms'));
        this.method.set(this.originalMethod());

        // Set recipients for display
        this.recipients.set(dto.batchMessageRecipient?.map(r => ({ name: r.name ?? 'Unknown' })) ?? []);

        // Set created by user (providing fallback)
        this.createdByUserName.set(dto.createdByUser ?? 'Unknown User');

        // Set audience based on recipients count vs loaded patients/leads count
        this.audience.set(dto.batchMessageRecipient.length === this.patients().length ? 'all' : 'selected');

        // Set entity based on isPatient flag from response and load respective list with callback
        if (dto.isPatient === true) {
          this.entity.set('patient');
          this.loadPatients(() => {
            this.setSelectedFromRecipients(dto.batchMessageRecipient);
          });
        } else {
          this.entity.set('lead');
          this.loadLeads(() => {
            this.setSelectedFromRecipients(dto.batchMessageRecipient);
          });
        }
      },
      error: () => {
        this.notificationService.showSnackBar('Could not load batch message for editing.', 'failure');
      }
    });
  }

  // Helper to set selected IDs from batchMessageRecipient list
  private setSelectedFromRecipients(recipients: { patientId?: string; leadId?: string }[] | undefined): void {
    if (!recipients || recipients.length === 0) {
      this.selectedIds.set(new Set());
      return;
    }
    const isPatientEntity = this.entity() === 'patient';
    const ids = recipients
      .map(r => isPatientEntity ? r.patientId : r.leadId)
      .filter((id): id is string => !!id);  // filter non-null strings
    this.selectedIds.set(new Set(ids));
    this.updateAllVisibleChecked();
  }

  // In edit mode, only allow subject and sms/email body editing
  disableField(field: string): boolean {
    if (!this.isEditMode()) return false;
    return !(field === 'subject' || field === 'sms' || field === 'emailHtml');
  }

  private loadPatients(afterLoad?: () => void): void {
    const id = this.userId();
    if (id !== null) {
      this.patientService.getPatientsByCounselorId(id).subscribe({
        next: (res) => {
          const mapped = res.map((p: any) => ({
            id: p.id,
            firstName: p.firstName,
            lastName: p.lastName,
            email: p.email,
            phoneNumber: p.phoneNumber,
            state: p.state ?? ''
          })) as PatientLeadCommunicationDto[];
          this.patients.set(mapped);
          this.applyFilters();
          if (afterLoad) afterLoad();
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to load patients. Please try again later.', 'failure');
        }
      });
    }
  }

  private loadLeads(afterLoad?: () => void): void {
    const id = this.userId();
    if (id !== null) {
      this.leadManagementService.getLeadsByCounselorId(id).subscribe({
        next: (res) => {
          const mapped = res.map((l: any) => ({
            id: l.id,
            firstName: l.firstName,
            lastName: l.lastName,
            email: l.email,
            phoneNumber: l.phoneNumber,
            state: l.state ?? ''
          })) as PatientLeadCommunicationDto[];
          this.patients.set(mapped);
          this.applyFilters();
          if (afterLoad) afterLoad();
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to load leads. Please try again later.', 'failure');
        }
      });
    }
  }

  applyMethod(value: 'email' | 'sms' | 'both'): void {
    this.method.set(value);
  }

  applyAudience(value: 'all' | 'selected'): void {
    this.audience.set(value);
  }

  onEntityChange(value: string): void {
    this.entity.set(value);
    if (value === 'patient') {
      this.loadPatients();
    } else if (value === 'lead') {
      this.loadLeads();
    }
  }

  applyFilters(): void {
    const term = this.searchTerm().trim().toLowerCase();
    const filtered = this.patients().filter(p => {
      const fullName = `${p.firstName} ${p.lastName}`.toLowerCase();
      const matchesSearch = term
        ? fullName.includes(term) ||
          (p.email?.toLowerCase().includes(term) ?? false) ||
          (p.phoneNumber?.includes(term) ?? false)
        : true;
      const matchesState = this.stateFilter() ? p.state === this.stateFilter() : true;
      return matchesSearch && matchesState;
    });
    this.filteredPatients.set(filtered);
    this.updateAllVisibleChecked();
  }

  trackById(_index: number, patient: PatientLeadCommunicationDto): string {
    return patient.id;
  }

  toggleRow(id: string, event: Event): void {
    const target = event.target as HTMLInputElement;
    const current = new Set(this.selectedIds());
    if (target.checked) {
      current.add(id);
    } else {
      current.delete(id);
    }
    this.selectedIds.set(current);
    this.updateAllVisibleChecked();
  }

  toggleAll(event: Event): void {
    const target = event.target as HTMLInputElement;
    let newSet = new Set<string>();
    if (target.checked) {
      newSet = new Set(this.filteredPatients().map(p => p.id));
    }
    this.selectedIds.set(newSet);
    this.updateAllVisibleChecked();
  }

  updateAllVisibleChecked(): void {
    const filtered = this.filteredPatients();
    const selected = this.selectedIds();
    this.allVisibleChecked.set(filtered.length > 0 && filtered.every(p => selected.has(p.id)));
  }

  exec(command: string): void {
    document.execCommand(command, false);
    this.updateEmailPlainLength();
  }

  setFont(font: string): void {
    document.execCommand('fontName', false, font);
    this.updateEmailPlainLength();
  }

  highlight(): void {
    document.execCommand('backColor', false, 'yellow');
    this.updateEmailPlainLength();
  }

  insertLink(): void {
    const url = prompt('Enter the link URL:', 'http://');
    if (url) {
      document.execCommand('createLink', false, url);
      this.updateEmailPlainLength();
    }
  }

  insertImage(): void {
    const url = prompt('Enter the image URL:', 'http://');
    if (url) {
      document.execCommand('insertImage', false, url);
      this.updateEmailPlainLength();
    }
  }

  toggleCode(): void {
    this.codeMode.set(!this.codeMode());
  }

  onEditorInput(): void {
    const editor = document.querySelector('.editor-surface');
    if (editor) {
      this.emailHtml.set(editor.innerHTML);
      this.updateEmailPlainLength();
    }
  }

  onCodeInput(): void {
    const editor = document.querySelector('.editor-surface');
    if (editor) {
      editor.innerHTML = this.emailHtml();
      this.updateEmailPlainLength();
    }
  }

  updateEmailPlainLength(): void {
    const temp = document.createElement('div');
    temp.innerHTML = this.emailHtml();
    this.emailPlainLength.set(temp.textContent?.length ?? 0);
  }

  submit(): void {
    if (!this.subject().trim()) {
      this.notificationService.showSnackBar('Subject is required.', 'failure');
      return;
    }
    if ((this.method() === 'email' || this.method() === 'both') && !this.emailHtml().trim()) {
      this.notificationService.showSnackBar('Email body is required for email or both methods.', 'failure');
      return;
    }
    if ((this.method() === 'sms' || this.method() === 'both') && !this.sms().trim()) {
      this.notificationService.showSnackBar('SMS body is required for SMS or both methods.', 'failure');
      return;
    }
    if (this.audience() === 'selected' && this.selectedIds().size === 0) {
      this.notificationService.showSnackBar('Please select at least one recipient.', 'failure');
      return;
    }
    if (this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);

    const isPatientEntity = this.entity() === 'patient';

    const batchRecipients = Array.from(this.selectedIds()).map(id => ({
      patientId: isPatientEntity ? id : null,
      leadId: !isPatientEntity ? id : null,
      status: undefined,
      errorReason: null
    }));

    const allRecipients = this.patients().map(p => ({
      patientId: isPatientEntity ? p.id : null,
      leadId: !isPatientEntity ? p.id : null,
      status: undefined,
      errorReason: null
    }));

    const payload = {
      subject: this.subject(),
      message: this.emailHtml() || this.sms(),
      createdByUserId: this.userId() ?? 0,
      createdAt: new Date().toISOString(),
      approvedByUserId: null,
      approvedAt: null,
      status: null,
      notes: null,
      isSms: this.method() === 'sms' || this.method() === 'both',
      isMail: this.method() === 'email' || this.method() === 'both',
      batchMessageRecipients: this.audience() === 'all' ? allRecipients : batchRecipients
    };

    this.emailAndSmsService.create(payload)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (response) => {
          if (response.id) {
            this.notificationService.showSnackBar('Batch message request submitted successfully.', 'success');
            this.summaryText.set('Batch message request submitted successfully.');
            if (isPatientEntity) {
              this.loadPatientsAfterSubmit();
            } else {
              this.loadLeads(); // reload leads instead
            }
            this.reset();
          } else {
            this.notificationService.showSnackBar(`Failed to submit: ${response.message || 'Unknown error'}`, 'failure');
          }
        },
        error: (error) => {
          console.error('Error submitting batch message:', error);
          this.notificationService.showSnackBar('An unexpected error occurred while submitting. Please try again later.', 'failure');
        }
      });
  }

  private loadPatientsAfterSubmit(): void {
    const id = this.userId();
    if (id !== null) {
      this.patientService.getPatientsByCounselorId(id).subscribe({
        next: (res) => {
          this.patients.set(res);
          this.applyFilters();
        },
        error: () => {
          this.notificationService.showSnackBar('Failed to reload patients after submit.', 'failure');
        }
      });
    }
  }

  reset(): void {
    this.entity.set('patient');
    this.method.set('email');
    this.audience.set('all');
    this.searchTerm.set('');
    this.stateFilter.set('');
    this.selectedIds.set(new Set());
    this.subject.set('');
    this.emailHtml.set('');
    this.sms.set('');
    this.emailPlainLength.set(0);
    this.emailFont.set('');
    this.codeMode.set(false);
    this.summaryText.set('');
    this.applyFilters();
  }
  approve(): void {
  if (!this.batchMessageId()) {
    this.notificationService.showSnackBar('No batch selected for approval', 'failure');
    return;
  }

  const payload: BatchMessageRequestDto = this.buildPayload();
  this.emailAndSmsService.approve(this.batchMessageId()!, payload).subscribe({
    next: () => {
      this.notificationService.showSnackBar('Batch approved successfully', 'success');
      this.redirectionToBulkSmsList();
    },
    error: () => {
      this.notificationService.showSnackBar('Failed to approve batch', 'failure');
    },
  });
}

reject(): void {
  if (!this.batchMessageId()) {
    this.notificationService.showSnackBar('No batch selected for rejection', 'failure');
    return;
  }

  const payload: BatchMessageRequestDto = this.buildPayload();
  this.emailAndSmsService.reject(this.batchMessageId()!, payload).subscribe({
    next: () => {
      this.notificationService.showSnackBar('Batch rejected successfully', 'success');
      this.redirectionToBulkSmsList();
    },
    error: () => {
      this.notificationService.showSnackBar('Failed to reject batch', 'failure');
    },
  });
}
redirectionToBulkSmsList(): void {
  this.router.navigateByUrl('/bulk-sms-approval');
}
private buildPayload(): BatchMessageRequestDto {
  return {
    batchMessageId: this.batchMessageId() ?? undefined,
    subject: this.subject(),
    message: this.sms() || this.emailHtml(),
    createdByUserId: this.userId() ?? 0,
    createdAt: new Date().toISOString(),
  };
}


}
