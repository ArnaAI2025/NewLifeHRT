import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderReceipt } from './order-receipt';

describe('OrderReceipt', () => {
  let component: OrderReceipt;
  let fixture: ComponentFixture<OrderReceipt>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderReceipt]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderReceipt);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
