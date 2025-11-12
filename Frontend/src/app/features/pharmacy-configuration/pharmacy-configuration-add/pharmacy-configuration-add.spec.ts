import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacyConfigurationAddComponent } from './pharmacy-configuration-add';

describe('PharmacyConfigurationAdd', () => {
  let component: PharmacyConfigurationAddComponent;
  let fixture: ComponentFixture<PharmacyConfigurationAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacyConfigurationAddComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacyConfigurationAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
