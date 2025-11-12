import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceListItemViewComponent } from './price-list-item-view';

describe('PriceListItemView', () => {
  let component: PriceListItemViewComponent;
  let fixture: ComponentFixture<PriceListItemViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceListItemViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceListItemViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
