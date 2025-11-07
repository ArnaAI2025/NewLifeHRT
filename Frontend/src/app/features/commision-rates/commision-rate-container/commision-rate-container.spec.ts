import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommisionRateContainerComponent } from './commision-rate-container';

describe('CommisionRateContainer', () => {
  let component: CommisionRateContainerComponent;
  let fixture: ComponentFixture<CommisionRateContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommisionRateContainerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommisionRateContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
