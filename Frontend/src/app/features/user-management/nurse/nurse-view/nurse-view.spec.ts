import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NurseView } from './nurse-view';

describe('NurseView', () => {
  let component: NurseView;
  let fixture: ComponentFixture<NurseView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NurseView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NurseView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
