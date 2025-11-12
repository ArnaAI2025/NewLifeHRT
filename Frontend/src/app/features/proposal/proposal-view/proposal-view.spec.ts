import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProposalView } from './proposal-view';

describe('ProposalView', () => {
  let component: ProposalView;
  let fixture: ComponentFixture<ProposalView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProposalView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProposalView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
