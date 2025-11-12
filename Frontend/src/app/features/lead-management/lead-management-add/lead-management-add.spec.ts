import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeadManagementAdd } from './lead-management-add';

describe('LeadManagementAdd', () => {
  let component: LeadManagementAdd;
  let fixture: ComponentFixture<LeadManagementAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeadManagementAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeadManagementAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
