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
import { ProductResponseDto } from './model/product-response.model';
import { ProductTypeResponseDto } from './model/product-type-response.model';
import { ProductCategoriesResponseDto } from './model/product-categories-response.model';
import { CreateProductRequestDto } from './model/create-product-request.model';
import { ProductFullResponseDto } from './model/product-full-response.model';


@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private baseHttp: HttpService, private http : HttpClient) {}

getAllProducts(): Observable<ProductResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.product.base,
    ApiHelper.product.getAllProducts
  );
  return this.baseHttp.get(url);
}
getAllActiveProducts(): Observable<ProductResponseDto[]> {
  const url = ApiResource.getURI(
    ApiHelper.product.base,
    ApiHelper.product.getAllActiveProducts
  );
  return this.baseHttp.get(url);
}

publishProducts(productIds: string[]) : Observable<any> {
  const url = ApiResource.getURI(
    ApiHelper.product.base,
    ApiHelper.product.publish
  );
  return this.baseHttp.post(url, { productIds });
}

deactivateProducts(productIds: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.deActivate);
  return this.baseHttp.post(url, { productIds });
}

deleteProducts(productIds: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.delete);
  return this.baseHttp.post(url, { productIds });
}

getTypeOptions() : Observable<ProductTypeResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.getProductTypes);
  return this.baseHttp.get(url);
}

getCategoriesOptions() : Observable<ProductCategoriesResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.getProductCategories);
  return this.baseHttp.get(url);
}

getWebFormOptions() : Observable<ProductCategoriesResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.getProductWebForms);
  return this.baseHttp.get(url);
}

createProduct(data: CreateProductRequestDto) : Observable<any> {
  const url = ApiResource.getURI(ApiHelper.product.base, ApiHelper.product.createProduct);
  return this.baseHttp.post(url, data);

}

getProductById(id: string) : Observable<ProductFullResponseDto> {
  const url = ApiResource.getURI(ApiHelper.product.base, `${ApiHelper.product.getProductById}/${id}`);
  return this.baseHttp.get<ProductFullResponseDto>(url);
}

updateProduct(id: string, data:CreateProductRequestDto) : Observable<any>
{
  const url = ApiResource.getURI(ApiHelper.product.base, `${ApiHelper.product.updateProduct}/${id}`);
  return this.baseHttp.put(url,data);
}

}
