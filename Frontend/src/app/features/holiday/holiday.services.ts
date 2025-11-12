import { Injectable } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import {
  HttpRequestOptions,
  DefaultHttpRequestOptions
} from '../../shared/models/http-request-options.model';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class HolidayService {
  constructor(private baseHttp: HttpService, private http: HttpClient) { }

  createHoliday(request: any): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.holiday.base, ApiHelper.holiday.create);
    return this.baseHttp.post(url, request);
  }

}
