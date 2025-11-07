import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PharmacyConfigurationViewComponent } from './pharmacy-configuration-view';

describe('PharmacyConfigurationView', () => {
  let component: PharmacyConfigurationViewComponent;
  let fixture: ComponentFixture<PharmacyConfigurationViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PharmacyConfigurationViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PharmacyConfigurationViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
