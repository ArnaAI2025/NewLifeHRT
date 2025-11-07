import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LifefileDashboardViewComponent } from './lifefile-dashboard-view';

describe('LifefileDashboardView', () => {
  let component: LifefileDashboardViewComponent;
  let fixture: ComponentFixture<LifefileDashboardViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LifefileDashboardViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LifefileDashboardViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
