import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SettleUpDialog } from './settle-up-dialog';

describe('SettleUpDialog', () => {
  let component: SettleUpDialog;
  let fixture: ComponentFixture<SettleUpDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SettleUpDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SettleUpDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
