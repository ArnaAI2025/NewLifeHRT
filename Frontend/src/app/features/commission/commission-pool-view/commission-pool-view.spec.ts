import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionPoolView } from './commission-pool-view';

describe('CommissionPoolView', () => {
  let component: CommissionPoolView;
  let fixture: ComponentFixture<CommissionPoolView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommissionPoolView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommissionPoolView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
