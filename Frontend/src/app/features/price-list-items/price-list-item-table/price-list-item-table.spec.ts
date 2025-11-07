import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceListItemTableComponent } from './price-list-item-table';

describe('PriceListItemTable', () => {
  let component: PriceListItemTableComponent;
  let fixture: ComponentFixture<PriceListItemTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceListItemTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceListItemTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
