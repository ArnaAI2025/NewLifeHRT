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
import { ProductStrengthResponseDto } from './model/product-strength-response.model';
import { CreateProductRequestDto } from '../product/model/create-product-request.model';
import { ProductStrengthCreateRequestDto } from './model/product-strength-create-request.model';


@Injectable({ providedIn: 'root' })
export class ProductStrengthService {
  constructor(private baseHttp: HttpService, private http : HttpClient) {}

  getAllStrengthsByProductId(productId: string): Observable<ProductStrengthResponseDto[]> {
    const url = ApiResource.getURI(ApiHelper.productStrength.base, `${ApiHelper.productStrength.getAllStrengthsByProductId}/${productId}`);
    return this.baseHttp.get<ProductStrengthResponseDto[]>(url);
  }

  deleteProductStrength(id: string): Observable<string>{
    const url = ApiResource.getURI(ApiHelper.productStrength.base, `${id}`);
    return this.baseHttp.delete<string>(url);
  }

  createProductStrength(data: ProductStrengthCreateRequestDto): Observable<any>
  {
    const url = ApiResource.getURI(ApiHelper.productStrength.base, ApiHelper.productStrength.createProductStrength);
    return this.baseHttp.post(url, data);
  }

  updateProductStrength(id: string, data:ProductStrengthCreateRequestDto) : Observable<any>
  {
    const url = ApiResource.getURI(ApiHelper.productStrength.base, `${ApiHelper.productStrength.updateProductStrength}/${id}`);
    return this.baseHttp.put(url,data);
  }
}
