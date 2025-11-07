import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnseenSms } from './unseen-sms';

describe('UnseenSms', () => {
  let component: UnseenSms;
  let fixture: ComponentFixture<UnseenSms>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UnseenSms]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnseenSms);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
