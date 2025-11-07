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
import { OrderProductRefillAllResponseModel } from './model/order-products-refill-all-response.model';
import { OrderProductRefillGetByIdResponseModel } from './model/order-products-refill-getbyid-response.model';


@Injectable({ providedIn: 'root' })
export class OrderproductRefillService {
  constructor(private baseHttp: HttpService) { }
  getAllOrderProductsRefillDetails(): Observable<OrderProductRefillAllResponseModel[]> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductRefill.base,
      ApiHelper.orderProductRefill.getAllOrderProductsRefillDetails
    );
    return this.baseHttp.get(url);
  }

  deleteOrderProductsRefill(ids: string[]): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductRefill.base,
      ApiHelper.orderProductRefill.delete
    );
    return this.baseHttp.post(url, ids);
  }

  getOrderProductRefillDetailsById(id: string): Observable<OrderProductRefillGetByIdResponseModel> {
    const url = ApiResource.getURI(ApiHelper.orderProductRefill.base, `${ApiHelper.orderProductRefill.getById}/${id}`);
    return this.baseHttp.get<OrderProductRefillGetByIdResponseModel>(url);
  }

  updateOrderProductRefillDetail(id: string, data: any): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductRefill.base,
      `${ApiHelper.orderProductRefill.update}/${id}`
    );
    return this.baseHttp.put(url, data);
  }

}
