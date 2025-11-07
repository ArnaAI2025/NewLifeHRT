import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientNavigationBar } from './patient-navigation-bar';

describe('PatientNavigationBar', () => {
  let component: PatientNavigationBar;
  let fixture: ComponentFixture<PatientNavigationBar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientNavigationBar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientNavigationBar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
