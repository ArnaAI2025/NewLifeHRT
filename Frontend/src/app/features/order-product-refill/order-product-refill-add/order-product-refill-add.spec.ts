import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductRefillAdd } from './order-product-refill-add';

describe('OrderProductRefillAdd', () => {
  let component: OrderProductRefillAdd;
  let fixture: ComponentFixture<OrderProductRefillAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductRefillAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductRefillAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
