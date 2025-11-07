import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientView } from './patient-view';

describe('PatientView', () => {
  let component: PatientView;
  let fixture: ComponentFixture<PatientView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
