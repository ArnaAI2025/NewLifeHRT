import { Injectable } from '@angular/core';
import { catchError, Observable, of, shareReplay, tap } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import {
  HttpRequestOptions,
  DefaultHttpRequestOptions
} from '../../shared/models/http-request-options.model';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { PharmacyGetAllResponseDto } from './model/pharmacy-all-response.model';
import { PharmacyGetByIdResponseDto } from './model/pharmacy-by-id.response.model';
import { CurrencyResponseDto } from './model/currency-response-model';
import { PharmacyCreateRequestDto } from './model/create-pharmacy-model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import { PharmacyShippingMethodResponseDto } from './model/pharmacy-shipping-method.model';
import { PharmaciesDropdownResponseDto } from './model/pharmacies-dropdown-response.model';


@Injectable({ providedIn: 'root' })
export class PharmacyService {
  constructor(private baseHttp: HttpService) {}
  private _getAllShippingMethods$?: Observable<DropDownResponseDto[]>;
  getAllPharmacy(): Observable<PharmacyGetAllResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.pharmacy.base,
      ApiHelper.pharmacy.getAllPharmacy
    );
    return this.baseHttp.get(url);
  }

  deletePharmacies(pharmacyIds: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.pharmacy.base, ApiHelper.pharmacy.delete);
  return this.baseHttp.post(url, { pharmacyIds });
  }

  getPharmacyById(id: string) : Observable<PharmacyGetByIdResponseDto> {
    const url = ApiResource.getURI(ApiHelper.pharmacy.base, `${ApiHelper.pharmacy.getPharmacyById}/${id}`);
    return this.baseHttp.get<PharmacyGetByIdResponseDto>(url);
  }

  getAllCurrencies() : Observable<CurrencyResponseDto[]> {
    const url = ApiResource.getURI(ApiHelper.pharmacy.base, ApiHelper.pharmacy.getAllCurrencies);
    return this.baseHttp.get(url);
  }

  createPharmacy(data: PharmacyCreateRequestDto) : Observable<any> {
    const url = ApiResource.getURI(ApiHelper.pharmacy.base, ApiHelper.pharmacy.createPharmacy);
    return this.baseHttp.post(url, data);

  }

  updatePharmacy(id: string, data:PharmacyCreateRequestDto) : Observable<any>
  {
    const url = ApiResource.getURI(ApiHelper.pharmacy.base, `${ApiHelper.pharmacy.updatePharmacy}/${id}`);
    return this.baseHttp.put(url,data);
  }

  activatePharmacies(pharmacyIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.pharmacy.base,
      ApiHelper.pharmacy.activatePharmacy
    );
    return this.baseHttp.post(url, { pharmacyIds });
  }

  deactivatePharmacies(pharmacyIds: string[]) :Observable<any>
  {
     const url = ApiResource.getURI(
      ApiHelper.pharmacy.base,
      ApiHelper.pharmacy.deactivatePharmacy
    );
    return this.baseHttp.post(url, { pharmacyIds });
  }
    getAllShippingMethods(): Observable<DropDownResponseDto[]> {
      if (!this._getAllShippingMethods$) {
        const url = ApiResource.getURI(
          ApiHelper.pharmacy.base,
          ApiHelper.pharmacy.getAllShippingMethods
        );
        this._getAllShippingMethods$ = this.baseHttp.get<DropDownResponseDto[]>(url).pipe(
          shareReplay(1)
        );
      }
      return this._getAllShippingMethods$;
    }
    getAllPharmacyShippingMethods(id: string) : Observable<PharmacyShippingMethodResponseDto[]> {
    const url = ApiResource.getURI(ApiHelper.pharmacy.base, `${ApiHelper.pharmacy.getAllPharmacyShippingMethods}/${id}`);
    return this.baseHttp.get<PharmacyShippingMethodResponseDto[]>(url);
  }
  getAllActivePharmacies(): Observable<PharmaciesDropdownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.pharmacy.base,
      ApiHelper.pharmacy.getAllActivePharmacies
    );
    return this.baseHttp.get<PharmaciesDropdownResponseDto[]>(url);
  }
}
