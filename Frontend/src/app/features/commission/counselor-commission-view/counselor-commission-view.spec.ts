import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounselorCommissionView } from './counselor-commission-view';

describe('CounselorCommissionView', () => {
  let component: CounselorCommissionView;
  let fixture: ComponentFixture<CounselorCommissionView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CounselorCommissionView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CounselorCommissionView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
