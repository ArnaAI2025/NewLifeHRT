import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommisionRateViewComponent } from './commision-rate-view';

describe('CommisionRateView', () => {
  let component: CommisionRateViewComponent;
  let fixture: ComponentFixture<CommisionRateViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommisionRateViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommisionRateViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
