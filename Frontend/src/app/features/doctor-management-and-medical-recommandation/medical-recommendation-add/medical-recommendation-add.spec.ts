import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalRecommendationAdd } from './medical-recommendation-add';

describe('MedicalRecommendationAdd', () => {
  let component: MedicalRecommendationAdd;
  let fixture: ComponentFixture<MedicalRecommendationAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalRecommendationAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalRecommendationAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
