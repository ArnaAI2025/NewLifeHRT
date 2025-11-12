import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CouponView } from './coupon-view';

describe('CouponView', () => {
  let component: CouponView;
  let fixture: ComponentFixture<CouponView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CouponView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CouponView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
