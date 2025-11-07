import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounselorCommissionDetailView } from './counselor-commission-detail-view';

describe('CounselorCommissionDetailView', () => {
  let component: CounselorCommissionDetailView;
  let fixture: ComponentFixture<CounselorCommissionDetailView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CounselorCommissionDetailView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CounselorCommissionDetailView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
