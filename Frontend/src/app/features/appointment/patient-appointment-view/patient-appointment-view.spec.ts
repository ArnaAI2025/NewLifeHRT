import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientAppointmentView } from './patient-appointment-view';

describe('PatientAppointmentView', () => {
  let component: PatientAppointmentView;
  let fixture: ComponentFixture<PatientAppointmentView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientAppointmentView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientAppointmentView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
