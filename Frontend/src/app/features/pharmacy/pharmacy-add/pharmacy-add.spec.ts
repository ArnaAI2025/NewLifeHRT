import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacyAdd } from './pharmacy-add';

describe('PharmacyAdd', () => {
  let component: PharmacyAdd;
  let fixture: ComponentFixture<PharmacyAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacyAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacyAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
