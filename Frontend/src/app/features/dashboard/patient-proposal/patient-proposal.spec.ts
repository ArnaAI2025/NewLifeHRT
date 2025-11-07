import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientProposal } from './patient-proposal';

describe('PatientProposal', () => {
  let component: PatientProposal;
  let fixture: ComponentFixture<PatientProposal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientProposal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientProposal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
