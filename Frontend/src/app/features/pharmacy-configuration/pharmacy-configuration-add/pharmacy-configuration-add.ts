// pharmacy-configuration-add.ts
import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom, Subscription } from 'rxjs';
import { PharmacyConfigurationRequestModel } from '../model/pharmacy-configuration-create-request.model';
import { PharmacyConfigurationService } from '../pharmacy-configuration.services';
import { NotificationService } from '../../../shared/services/notification.service';
import { PharmacyService } from '../../pharmacy/pharmacy.services';
import { ConfirmationDialogData } from '../../../shared/models/confirmation-dialog.model';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { HttpErrorResponse } from '@angular/common/http';
import { NoWhitespaceValidator } from '../../../shared/validators/no-whitespace.validator';

@Component({
  standalone: true,
  selector: 'app-pharmacy-configuration-add',
  imports: [
    MatFormFieldModule,
    CommonModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonToggleModule
  ],
  templateUrl: './pharmacy-configuration-add.html',
  styleUrl: './pharmacy-configuration-add.scss'
})
export class PharmacyConfigurationAddComponent implements OnInit, OnDestroy {
  // private fb = inject(FormBuilder);
  // private pharmacyConfigurationService = inject(PharmacyConfigurationService);
  // private router = inject(Router);
  // private route = inject(ActivatedRoute);
  // private notificationService = inject(NotificationService);
  // private pharmacyService = inject(PharmacyService);
  private originalConfigData: Array<{ keyId: number, value: string }> = [];

  pharmacyConfigurationForm!: FormGroup;
  pharmacies = signal<Array<{ id: string; name: string }>>([]);
  integrationTypes = signal<Array<{ id: number; value: string }>>([]);
  integrationKeys = signal<Array<{ id: number; integrationTypeId: number; keyName: string; label: string }>>([]);

  isEditMode = signal(false);
  isSaveAndClose = false;
  showActivate = signal(false);
  showDeactivate = signal(false);
  showDelete = signal(false);

  private typeChangeSubscription!: Subscription;

  constructor(
    private fb: FormBuilder,
    private pharmacyConfigurationService: PharmacyConfigurationService,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private pharmacyService: PharmacyService,
    private confirmationService: ConfirmationDialogService
  ) { }

  async ngOnInit(): Promise<void> {
    this.initForm();
    await Promise.all([
      this.loadPharmacies(),
      this.loadIntegrationTypes()
    ]);
    this.typeChangeSubscription = this.pharmacyConfigurationForm.get('type')!.valueChanges.subscribe(
      async (typeId: number) => {
        await this.onTypeChanged(typeId);
      }
    );

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      await this.loadPharmacyConfigurationById(id);
    }
  }

  ngOnDestroy(): void {
    if (this.typeChangeSubscription) {
      this.typeChangeSubscription.unsubscribe();
    }
  }

  private initForm() {
    this.pharmacyConfigurationForm = this.fb.group({
      pharmacy: [null, Validators.required],
      type: [null, Validators.required]
    });
  }

  getKeyControlName(keyId: number): string {
    return `key_${keyId}`;
  }

  private async loadPharmacies(): Promise<void> {
    try {
      const res = await firstValueFrom(this.pharmacyService.getAllActivePharmacies());
      this.pharmacies.set(res.map(r => ({ id: r.id, name: r.name })));
    } catch (err) {
      this.notificationService.showSnackBar('Failed to load pharmacies.', 'failure');
    }
  }

  private async loadIntegrationTypes(): Promise<void> {
    try {
      const resp = await firstValueFrom(this.pharmacyConfigurationService.getActiveIntegrationTypes());
      this.integrationTypes.set(resp.map((r: any) => ({ id: r.id, value: r.value })));
    } catch (err) {
      this.notificationService.showSnackBar('Failed to load integration types.', 'failure');
    }
  }

  private async onTypeChanged(typeId: number | null, isInitialLoad = false): Promise<void> {
    this.clearExistingKeyControls();

    if (!typeId) {
      this.integrationKeys.set([]);
      return;
    }

    try {
      const keys = await firstValueFrom(
        this.pharmacyConfigurationService.getIntegrationKeysByTypeId(typeId)
      );

      this.integrationKeys.set(keys);

      keys.forEach(k => {
        const controlName = this.getKeyControlName(k.id);
        this.pharmacyConfigurationForm.addControl(
          controlName,
          new FormControl('', [Validators.required,NoWhitespaceValidator])
        );
        if (
          isInitialLoad ||
          (this.isEditMode() && typeId === this.pharmacyConfigurationForm.value.type)
        ) {
          const existing = this.originalConfigData.find(cd => cd.keyId === k.id);
          if (existing) {
            this.pharmacyConfigurationForm.get(controlName)?.setValue(existing.value);
          }
        }
      });
    } catch (err) {
      this.notificationService.showSnackBar('Failed to load integration keys.', 'failure');
    }
  }

  private clearExistingKeyControls(): void {
    const currentKeys = this.integrationKeys();
    currentKeys.forEach(k => {
      const controlName = this.getKeyControlName(k.id);
      if (this.pharmacyConfigurationForm.contains(controlName)) {
        this.pharmacyConfigurationForm.removeControl(controlName);
      }
    });
  }

  async submitForm(closeAfterSave = false): Promise<void> {
    if (this.pharmacyConfigurationForm.invalid) {
      this.pharmacyConfigurationForm.markAllAsTouched();
      this.notificationService.showSnackBar('Please fill all required fields.', 'failure');
      return;
    }

    const payload: PharmacyConfigurationRequestModel = {
      pharmacyId: this.pharmacyConfigurationForm.value.pharmacy,
      typeId: this.pharmacyConfigurationForm.value.type,
      configData: this.integrationKeys().map(k => ({
        keyId: k.id,
        value: this.pharmacyConfigurationForm.get(this.getKeyControlName(k.id))?.value.trim() ?? ''
      }))
    };

    try {
      if (this.isEditMode()) {
        const id = this.route.snapshot.paramMap.get('id')!;
        await firstValueFrom(
          this.pharmacyConfigurationService.updatePharmacyConfiguration(id, payload)
        );
        this.notificationService.showSnackBar('Configuration updated successfully.', 'success');
      } else {
        const created = await firstValueFrom(
          this.pharmacyConfigurationService.createPharmacyConfiguration(payload)
        );
        this.notificationService.showSnackBar(created.message || 'Configuration created successfully.', 'success');
        this.notificationService.showSnackBar(created.message || 'Configuration created successfully.', 'success');

        if (!closeAfterSave) {
          this.router.navigate(['/pharmacyconfiguration/edit', created.pharmacyConfigurationId]);
        }
      }

      if (closeAfterSave) {
        this.router.navigate(['/pharmacyconfiguration/view']);
      }
    } catch (err: unknown) {
      const httpError = err as HttpErrorResponse;
      const errorMsg =
        httpError.error?.message ||
        httpError.error?.Message ||
        httpError.message ||
        'Failed to save configuration.';
      this.notificationService.showSnackBar(errorMsg, 'failure');
    }
  }

  private async loadPharmacyConfigurationById(id: string): Promise<void> {
    try {
      const response = await firstValueFrom(
        this.pharmacyConfigurationService.getById(id)
      );

      this.pharmacyConfigurationForm.patchValue({
        pharmacy: response.pharmacyId,
        type: response.typeId
      });

      this.originalConfigData = response.configData;

      await this.onTypeChanged(response.typeId, true);
      if (response.status === 'Active') {
        this.showDeactivate.set(true);
        this.showDelete.set(true);
        this.showActivate.set(false);
      } else if (response.status === 'Inactive') {
        this.showActivate.set(true);
        this.showDelete.set(true);
        this.showDeactivate.set(false);
      }

    } catch (err) {
      this.notificationService.showSnackBar('Failed to load configuration.', 'failure');
    }
  }

  goBack(): void {
    this.router.navigate(['/pharmacyconfiguration/view']);
  }

  navigateToAddPharmacyConfiguration() {
    this.router.navigate(['/pharmacyconfiguration/add']);
  }

  async activateConfig(): Promise<void> {
    const confirmed = await this.openConfirmation('Activate');
    if (!confirmed) return;
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      await firstValueFrom(this.pharmacyConfigurationService.activatePharmacyConfigurations([id]));
      this.notificationService.showSnackBar('Pharmacy Configuration activated successfully.', 'success');
      await this.loadPharmacyConfigurationById(id);
    } catch {
      this.notificationService.showSnackBar('Activation failed.', 'failure');
    }
  }

  async deactivateConfig(): Promise<void> {
    const confirmed = await this.openConfirmation('Deactivate');
    if (!confirmed) return;
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      await firstValueFrom(this.pharmacyConfigurationService.deactivatePharmacyConfigurations([id]));
      this.notificationService.showSnackBar('Pharmacy Configuration deactivated successfully.', 'success');
      await this.loadPharmacyConfigurationById(id);
    } catch {
      this.notificationService.showSnackBar('Deactivation failed.', 'failure');
    }
  }

  async deleteConfig(): Promise<void> {
    const confirmed = await this.openConfirmation('Delete');
    if (!confirmed) return;
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      await firstValueFrom(this.pharmacyConfigurationService.deletePharmacyConfigurations([id]));
      this.notificationService.showSnackBar('Pharmacy Configuration deleted successfully.', 'success');
      this.router.navigate(['/pharmacyconfiguration/view']);
    } catch {
      this.notificationService.showSnackBar('Delete failed.', 'failure');
    }
  }

  async openConfirmation(action: string): Promise<boolean> {
    const data: ConfirmationDialogData = {
      title: `${action} Confirmation`,
      message: `<p>Are you sure you want to <strong>${action.toLowerCase()}</strong> this pharmacy configuration?</p>`,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    };

    const confirmed = await firstValueFrom(this.confirmationService.openConfirmation(data));
    return confirmed ?? false;
  }
}
