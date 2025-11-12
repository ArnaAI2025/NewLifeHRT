import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacyView } from './pharmacy-view';

describe('PharmacyView', () => {
  let component: PharmacyView;
  let fixture: ComponentFixture<PharmacyView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacyView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacyView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
