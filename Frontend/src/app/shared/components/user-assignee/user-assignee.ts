import { Component, Inject, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DropDownResponseDto } from '../../models/drop-down-response.model';
import { UserManagementService } from '../../../features/user-management/user-management.service';
import { startWith, map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { firstValueFrom } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserRole } from '../../enums/user-role.enum';

interface DialogData {
  isFrom: number;
}

@Component({
  selector: 'app-user-assignee',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatIconModule,
    CommonModule,
  ],
  templateUrl: './user-assignee.html',
  styleUrl: './user-assignee.scss',
})
export class UserAssigneeComponent implements OnInit {
  userForm!: FormGroup;
  userList: DropDownResponseDto[] = [];
  filteredUserList: DropDownResponseDto[] = [];
  isLoadingUsers = signal(false);
  isFrom: number;

  UserRole = UserRole;

  private userManagementService = inject(UserManagementService);
  private dialogRef = inject(MatDialogRef<UserAssigneeComponent>);
  private fb = inject(FormBuilder);

  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData) {
    this.isFrom = data?.isFrom || 0;
  }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      userId: [null, Validators.required],
    });

    console.log('Dialog opened from user role:', UserRole[this.isFrom]);

    // Trigger backend call on each keyup (debounced)
    this.userForm
      .get('userId')
      ?.valueChanges.pipe(
        map((val) => (typeof val === 'string' ? val : val?.value ?? '')),
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe((searchTerm) => {
        this.loadUsers(searchTerm);
      });

    // Initial load with empty search
    this.loadUsers('');
  }

  async loadUsers(searchTerm: string) {
    this.isLoadingUsers.set(true);
    try {
      this.userList = await firstValueFrom(
        this.userManagementService.getAllActiveUsers(this.isFrom, searchTerm)
      );
      this.filteredUserList = this.userList; // backend already limits to top 7
    } catch (err) {
      console.error('Failed to load users:', err);
      this.userList = [];
      this.filteredUserList = [];
    } finally {
      this.isLoadingUsers.set(false);
    }
  }

  displayUser(user?: DropDownResponseDto): string {
    return user ? user.value : '';
  }

  filterUsers(name: string): DropDownResponseDto[] {
    const filterValue = name.toLowerCase();
    return this.userList.filter((person) =>
      person.value.toLowerCase().includes(filterValue)
    );
  }

  closeDialog() {
    this.dialogRef.close(null);
  }

  onSave() {
    if (this.userForm.valid) {
      this.dialogRef.close(this.userForm.value.userId);
      this.userForm.reset();
    }
  }

  isFieldInvalid(field: string) {
    const control = this.userForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getFieldError(field: string) {
    const control = this.userForm.get(field);
    if (!control || !control.errors) return '';
    return control.errors?.['required']
      ? `${UserRole[this.isFrom]} is required`
      : 'Invalid value';
  }
   formatUserRole(role: string): string {
    return role
      .replace(/([a-z])([A-Z])/g, '$1 $2') 
      .replace(/^./, (c) => c.toUpperCase()); 
  }
}
