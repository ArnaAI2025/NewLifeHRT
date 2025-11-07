import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListView } from './user-list-view';

describe('UserListView', () => {
  let component: UserListView;
  let fixture: ComponentFixture<UserListView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserListView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserListView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
