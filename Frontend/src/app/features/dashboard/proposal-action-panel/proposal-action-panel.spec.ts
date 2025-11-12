import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProposalActionPanel } from './proposal-action-panel';

describe('ProposalActionPanel', () => {
  let component: ProposalActionPanel;
  let fixture: ComponentFixture<ProposalActionPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProposalActionPanel]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProposalActionPanel);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
