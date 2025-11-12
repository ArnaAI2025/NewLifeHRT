import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesPersonView } from './sales-person-view';

describe('SalesPersonView', () => {
  let component: SalesPersonView;
  let fixture: ComponentFixture<SalesPersonView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalesPersonView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SalesPersonView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
