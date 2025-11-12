import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommisionRateTableComponent } from './commision-rate-table';

describe('CommisionRateTable', () => {
  let component: CommisionRateTableComponent;
  let fixture: ComponentFixture<CommisionRateTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommisionRateTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommisionRateTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
