import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAssignee } from './user-assignee';

describe('UserAssignee', () => {
  let component: UserAssignee;
  let fixture: ComponentFixture<UserAssignee>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserAssignee]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserAssignee);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
