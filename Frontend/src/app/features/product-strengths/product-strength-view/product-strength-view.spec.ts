import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductStrengthView } from './product-strength-view';

describe('ProductStrengthView', () => {
  let component: ProductStrengthView;
  let fixture: ComponentFixture<ProductStrengthView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductStrengthView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductStrengthView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
