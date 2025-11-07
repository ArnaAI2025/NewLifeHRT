import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShippingAddressView } from './shipping-address-view';

describe('ShippingAddressView', () => {
  let component: ShippingAddressView;
  let fixture: ComponentFixture<ShippingAddressView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingAddressView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShippingAddressView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
