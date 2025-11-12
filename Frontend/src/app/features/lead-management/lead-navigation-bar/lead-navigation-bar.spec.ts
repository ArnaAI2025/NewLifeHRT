import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeadNavigationBar } from './lead-navigation-bar';

describe('LeadNavigationBar', () => {
  let component: LeadNavigationBar;
  let fixture: ComponentFixture<LeadNavigationBar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeadNavigationBar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeadNavigationBar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
