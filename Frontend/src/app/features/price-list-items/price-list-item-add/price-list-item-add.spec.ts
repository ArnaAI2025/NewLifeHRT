import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceListItemAddComponent } from './price-list-item-add';

describe('PriceListItemAdd', () => {
  let component: PriceListItemAddComponent;
  let fixture: ComponentFixture<PriceListItemAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceListItemAddComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceListItemAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
