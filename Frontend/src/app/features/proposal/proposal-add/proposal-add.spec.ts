import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProposalAdd } from './proposal-add';

describe('ProposalAdd', () => {
  let component: ProposalAdd;
  let fixture: ComponentFixture<ProposalAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProposalAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProposalAdd);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
