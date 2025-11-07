import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommisionRateAddComponent } from './commision-rate-add';

describe('CommisionRateAdd', () => {
  let component: CommisionRateAddComponent;
  let fixture: ComponentFixture<CommisionRateAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommisionRateAddComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommisionRateAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
