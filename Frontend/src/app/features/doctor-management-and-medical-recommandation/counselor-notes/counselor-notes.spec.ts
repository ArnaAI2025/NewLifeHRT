import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounselorNotes } from './counselor-notes';

describe('CounselorNotes', () => {
  let component: CounselorNotes;
  let fixture: ComponentFixture<CounselorNotes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CounselorNotes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CounselorNotes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
