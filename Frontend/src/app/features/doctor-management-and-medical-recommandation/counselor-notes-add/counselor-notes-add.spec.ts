import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounselorNotesAdd } from './counselor-notes-add';

describe('CounselorNotesAdd', () => {
  let component: CounselorNotesAdd;
  let fixture: ComponentFixture<CounselorNotesAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CounselorNotesAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CounselorNotesAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
