import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalRecommandationView } from './medical-recommandation-view';

describe('MedicalRecommandationView', () => {
  let component: MedicalRecommandationView;
  let fixture: ComponentFixture<MedicalRecommandationView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalRecommandationView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalRecommandationView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
