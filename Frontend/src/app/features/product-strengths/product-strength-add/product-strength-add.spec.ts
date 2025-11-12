import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductStrengthAdd } from './product-strength-add';

describe('ProductStrengthAdd', () => {
  let component: ProductStrengthAdd;
  let fixture: ComponentFixture<ProductStrengthAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductStrengthAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductStrengthAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
