import { Injectable } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import {
  HttpRequestOptions,
  DefaultHttpRequestOptions
} from '../../shared/models/http-request-options.model';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { PriceListItemsGetAllResponseDto } from './model/pricelistitems-getall-response.model';
import { PriceListItemsByProductIdResponseDto } from './model/pricelistitem-by-productid-response.model';
import { PriceListItemsByPharmacyIdResponseDto } from './model/pricelistitem-by-pharmacyid-response.model';
import { PriceListItemGetByIdResponseDto } from './model/price-list-item-by-id.response.model';
import { DropdownResponseDto } from './model/dropdown-response.model';
import { PriceListItemRequestDto } from './model/price-list-item-create-request.model';
import { ProductPharmacyDropdownResponseDto } from './model/product-pharmacy-dropdown-response.model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';

@Injectable({ providedIn: 'root' })
export class PriceListItemService {
  constructor(private baseHttp: HttpService, private http : HttpClient) {}

  getAllPriceListItems(): Observable<PriceListItemsGetAllResponseDto[]> {
      const url = ApiResource.getURI(
        ApiHelper.priceListItems.base,
        ApiHelper.priceListItems.getAllPriceListItems
      );
      return this.baseHttp.get<PriceListItemsGetAllResponseDto[]>(url);
    }

  getAllPriceListItemsByProductId(productId: string): Observable<PriceListItemsByProductIdResponseDto[]> {
      const url = ApiResource.getURI(ApiHelper.priceListItems.base, `${ApiHelper.priceListItems.getAllPriceListItemsByProductId}/${productId}`);
      return this.baseHttp.get<PriceListItemsByProductIdResponseDto[]>(url);
    }

  getAllPriceListItemsByPharmacyId(pharmacyId: string): Observable<PriceListItemsByPharmacyIdResponseDto[]> {
      const url = ApiResource.getURI(ApiHelper.priceListItems.base, `${ApiHelper.priceListItems.getAllPriceListItemsByPharmacyId}/${pharmacyId}`);
      return this.baseHttp.get<PriceListItemsByPharmacyIdResponseDto[]>(url);
    }

  activatePriceListItems(priceListItemIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.priceListItems.base,
      ApiHelper.priceListItems.activatePriceListItems
    );
    return this.baseHttp.post(url, { priceListItemIds });
  }

  deactivatePriceListItems(priceListItemIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.priceListItems.base,
      ApiHelper.priceListItems.deactivatePriceListItems
    );
    return this.baseHttp.post(url, { priceListItemIds });
  }

  deletePriceListItems(priceListItemIds: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.priceListItems.base, ApiHelper.priceListItems.deletePriceListItems);
  return this.baseHttp.post(url, { priceListItemIds });
  }

  getPriceListItemById(id: string): Observable<PriceListItemGetByIdResponseDto> {
  const url = ApiResource.getURI(
    ApiHelper.priceListItems.base,
    `${ApiHelper.priceListItems.getPriceListItemById}/${id}`
  );
  return this.baseHttp.get<PriceListItemGetByIdResponseDto>(url);
}

getAllLifeFileDrugForms(): Observable<DropdownResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.priceListItems.base,
    ApiHelper.priceListItems.getAllLifeFileDrugForms
  );
  return this.baseHttp.get<DropdownResponseDto[]>(url);
}

getAllLifeFileQuantityUnits(): Observable<DropdownResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.priceListItems.base,
    ApiHelper.priceListItems.getAllLifeFileQuantityUnits
  );
  return this.baseHttp.get<DropdownResponseDto[]>(url);
}

getAllLifeFileScheduleCodes(): Observable<DropdownResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.priceListItems.base,
    ApiHelper.priceListItems.getAllLifeFileScheduleCodes
  );
  return this.baseHttp.get<DropdownResponseDto[]>(url);
}

getAllProductsForDropdown(): Observable<ProductPharmacyDropdownResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.product.base,
    ApiHelper.product.getAllProductsForDropdown
  );
  return this.baseHttp.get<ProductPharmacyDropdownResponseDto[]>(url);
}

getAllPharmaciesForDropdown(): Observable<ProductPharmacyDropdownResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.pharmacy.base,
    ApiHelper.pharmacy.getAllPharmaciesForDropdown
  );
  return this.baseHttp.get<ProductPharmacyDropdownResponseDto[]>(url);
}

createPriceListItem(request: PriceListItemRequestDto): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.priceListItems.base,
      ApiHelper.priceListItems.create
    );
    return this.baseHttp.post(url, request);
  }

  updatePriceListItem(id: string, request: PriceListItemRequestDto): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.priceListItems.base,
      `${ApiHelper.priceListItems.update}/${id}`
    );
    return this.baseHttp.put(url, request);
  }
    getAllActivePriceListItemsByPharmacyId(pharmacyId: string): Observable<PriceListItemsByPharmacyIdResponseDto[]> {
      const url = ApiResource.getURI(ApiHelper.priceListItems.base, `${ApiHelper.priceListItems.getAllActivePriceListItemsByPharmacyId}/${pharmacyId}`);
      return this.baseHttp.get<PriceListItemsByPharmacyIdResponseDto[]>(url);
    }
}

