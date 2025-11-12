import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserManagementFormComponent } from './user-management-form';

describe('UserManagementForm', () => {
  let component: UserManagementFormComponent;
  let fixture: ComponentFixture<UserManagementFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserManagementFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserManagementFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
