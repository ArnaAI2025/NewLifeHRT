import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShippingAddressAdd } from './shipping-address-add';

describe('ShippingAddressAdd', () => {
  let component: ShippingAddressAdd;
  let fixture: ComponentFixture<ShippingAddressAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingAddressAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShippingAddressAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
