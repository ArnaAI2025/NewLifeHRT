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
import { CommisionRatesGetAllResponseDto } from './model/commision-rate-getall-response.model';
import { CommisionRatesByProductIdResponseDto } from './model/commisionrate-by-productid-response.model';
import { ProductPharmacyDropdownResponseDto } from '../price-list-items/model/product-pharmacy-dropdown-response.model';
import { CommisionRateGetByIdResponseDto } from './model/commision-rate-getbyid-response.model';
import { CommisionRateRequestDto } from './model/commision-rate-create-request.model';

@Injectable({ providedIn: 'root' })
export class CommisionRateService {
  constructor(private baseHttp: HttpService, private http : HttpClient) {}

  getAllCommisionRates(): Observable<CommisionRatesGetAllResponseDto[]> {
        const url = ApiResource.getURI(
          ApiHelper.commisionRates.base,
          ApiHelper.commisionRates.getAllCommisionRates
        );
        return this.baseHttp.get<CommisionRatesGetAllResponseDto[]>(url);
  }

  getAllCommisionRatesByProductId(productId: string): Observable<CommisionRatesByProductIdResponseDto[]> {
        const url = ApiResource.getURI(ApiHelper.commisionRates.base, `${ApiHelper.commisionRates.getAllCommisionRatesByProductId}/${productId}`);
        return this.baseHttp.get<CommisionRatesByProductIdResponseDto[]>(url);
  }

  activateCommisionRates(commisionRatesIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.commisionRates.base,
      ApiHelper.commisionRates.activateCommisionRates
    );
    return this.baseHttp.post(url, commisionRatesIds);
  }

  deactivateCommisionRates(commisionRatesIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.commisionRates.base,
      ApiHelper.commisionRates.deactivateCommisionRates
    );
    return this.baseHttp.post(url, commisionRatesIds );
  }

  deleteCommisionRates(commisionRatesIds: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.commisionRates.base, ApiHelper.commisionRates.deleteCommisionRates);
  return this.baseHttp.post(url, commisionRatesIds);
  }

  getAllProductsForDropdown(): Observable<ProductPharmacyDropdownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.product.base,
      ApiHelper.product.getAllProductsForDropdown
    );
    return this.baseHttp.get<ProductPharmacyDropdownResponseDto[]>(url);
  }

  getCommisionRateById(id: string): Observable<CommisionRateGetByIdResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.commisionRates.base,
      `${ApiHelper.commisionRates.getCommisionRateById}/${id}`
    );
    return this.baseHttp.get<CommisionRateGetByIdResponseDto>(url);
  }

  createCommisionRate(request: CommisionRateRequestDto): Observable<any> {
      const url = ApiResource.getURI(
        ApiHelper.commisionRates.base,
        ApiHelper.commisionRates.create
      );
      return this.baseHttp.post(url, request);
    }

    updateCommisionRate(id: string, request: CommisionRateRequestDto): Observable<any> {
      const url = ApiResource.getURI(
        ApiHelper.commisionRates.base,
        `${ApiHelper.commisionRates.update}/${id}`
      );
      return this.baseHttp.put(url, request);
    }



}

