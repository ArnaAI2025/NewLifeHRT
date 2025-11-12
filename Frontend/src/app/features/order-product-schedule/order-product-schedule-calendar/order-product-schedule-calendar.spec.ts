import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductScheduleCalendarComponent } from './order-product-schedule-calendar';

describe('OrderProductScheduleCalendar', () => {
  let component: OrderProductScheduleCalendarComponent;
  let fixture: ComponentFixture<OrderProductScheduleCalendarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductScheduleCalendarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductScheduleCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
