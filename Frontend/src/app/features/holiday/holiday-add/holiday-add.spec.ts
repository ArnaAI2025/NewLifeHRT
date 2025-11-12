import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HolidayAddComponent } from './holiday-add';

describe('HolidayAdd', () => {
  let component: HolidayAddComponent;
  let fixture: ComponentFixture<HolidayAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HolidayAddComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HolidayAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
