import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReminderView } from './reminder-view';

describe('ReminderView', () => {
  let component: ReminderView;
  let fixture: ComponentFixture<ReminderView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReminderView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReminderView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
