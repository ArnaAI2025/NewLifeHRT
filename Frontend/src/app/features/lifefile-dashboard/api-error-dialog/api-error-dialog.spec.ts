import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiErrorDialog } from './api-error-dialog';

describe('ApiErrorDialog', () => {
  let component: ApiErrorDialog;
  let fixture: ComponentFixture<ApiErrorDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApiErrorDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApiErrorDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
