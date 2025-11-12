import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductScheduleSummaryViewComponent } from './order-product-schedule-summary-view';

describe('OrderProductScheduleSummaryView', () => {
  let component: OrderProductScheduleSummaryViewComponent;
  let fixture: ComponentFixture<OrderProductScheduleSummaryViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductScheduleSummaryViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductScheduleSummaryViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
