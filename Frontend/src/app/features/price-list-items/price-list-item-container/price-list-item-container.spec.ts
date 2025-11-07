import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceListItemContainerComponent } from './price-list-item-container';

describe('PriceListItemContainer', () => {
  let component: PriceListItemContainerComponent;
  let fixture: ComponentFixture<PriceListItemContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceListItemContainerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceListItemContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
