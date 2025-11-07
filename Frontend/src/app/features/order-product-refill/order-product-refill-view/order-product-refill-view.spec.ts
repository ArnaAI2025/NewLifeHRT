import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderProductRefillViewComponent } from './order-product-refill-view';

describe('OrderProductRefillView', () => {
  let component: OrderProductRefillViewComponent;
  let fixture: ComponentFixture<OrderProductRefillViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderProductRefillViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderProductRefillViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
