import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkSmsApproval } from './bulk-sms-approval';

describe('BulkSmsApproval', () => {
  let component: BulkSmsApproval;
  let fixture: ComponentFixture<BulkSmsApproval>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BulkSmsApproval]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BulkSmsApproval);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
