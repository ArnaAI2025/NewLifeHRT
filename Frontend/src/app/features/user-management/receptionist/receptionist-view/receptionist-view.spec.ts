import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceptionistView } from './receptionist-view';

describe('ReceptionistView', () => {
  let component: ReceptionistView;
  let fixture: ComponentFixture<ReceptionistView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceptionistView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReceptionistView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
