import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-lead-navigation-bar',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatButtonToggleModule,
    FormsModule
  ],
  templateUrl: './lead-navigation-bar.html',
  styleUrls: ['./lead-navigation-bar.scss']
})
export class LeadNavigationBar {
  @Input() isEditMode = false;
  @Input() isSaving = false;
  @Input() leadId: string | null = null;
  @Input() activeView: 'form' | 'sms' | 'reminders' = 'form';
  @Input() isActive = false;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();
  @Output() addLead = new EventEmitter<void>();
  @Output() saveAndClose = new EventEmitter<void>();
  @Output() deleteLead = new EventEmitter<void>();
  @Output() toggleActive = new EventEmitter<void>();

  private readonly router = inject(Router);

  onClose(): void {
    this.close.emit();
  }

  onSubmit(): void {
    this.save.emit();
  }

  onClickAddLead(): void {
    this.addLead.emit();
  }

  onSaveAndCloseClick(): void {
    this.saveAndClose.emit();
  }

  onDeleteClick(): void {
    this.deleteLead.emit();
  }

  onToggleActiveClick(): void {
    this.toggleActive.emit();
  }

  onViewToggle(event: MatButtonToggleChange): void {
    const selectedView = event.value as 'form' | 'sms' | 'reminders';
    if (selectedView === 'form' && this.isEditMode && this.leadId) {
      this.router.navigateByUrl(`/lead-management/edit/${this.leadId}`);
    } else if (selectedView === 'sms' && this.isEditMode && this.leadId) {
      this.router.navigateByUrl(`/lead-management/${this.leadId}/sms`);
    }
    else if (selectedView === 'reminders' && this.isEditMode && this.leadId) {
      this.router.navigateByUrl(`/lead-management/${this.leadId}/reminder`);
    }
  }

  get isSavingValue(): boolean {
    return typeof this.isSaving === 'function' ? this.isSaving : this.isSaving;
  }
}
