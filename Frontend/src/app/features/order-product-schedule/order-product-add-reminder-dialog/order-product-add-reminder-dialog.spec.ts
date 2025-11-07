import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductAddReminderDialog } from './order-product-add-reminder-dialog';

describe('OrderProductAddReminderDialog', () => {
  let component: OrderProductAddReminderDialog;
  let fixture: ComponentFixture<OrderProductAddReminderDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductAddReminderDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductAddReminderDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
