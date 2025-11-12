import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductScheduleEditDialog } from './order-product-schedule-edit-dialog';

describe('OrderProductScheduleEditDialog', () => {
  let component: OrderProductScheduleEditDialog;
  let fixture: ComponentFixture<OrderProductScheduleEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductScheduleEditDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductScheduleEditDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
