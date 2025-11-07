import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RefundAmountDialog } from './refund-amount-dialog';

describe('RefundAmountDialog', () => {
  let component: RefundAmountDialog;
  let fixture: ComponentFixture<RefundAmountDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RefundAmountDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RefundAmountDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
