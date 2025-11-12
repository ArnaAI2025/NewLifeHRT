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
import { OrderProductScheduleFilterRequestDto } from './model/order-product-schedule-filter-request.model';
import { OrderProductScheduleResponseDto } from './model/order-product-schedule-filter-response.model';
import { ScheduleSummaryResponse } from './model/shcedule-summary-response.model';


@Injectable({ providedIn: 'root' })
export class OrderproductScheduleCalendarService {
  constructor(private baseHttp: HttpService) { }

  getAllScheduleOfOrderProductsForLoggedInPatient(data: OrderProductScheduleFilterRequestDto): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.orderProductSchedule.base, ApiHelper.orderProductSchedule.getAllOrderProductsScheduleDetails);
    return this.baseHttp.post<any>(url, data);
  }

  getPatientScheduleSummary(): Observable<ScheduleSummaryResponse> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      ApiHelper.orderProductSchedule.schedulesSummary
    );
    return this.baseHttp.get<ScheduleSummaryResponse>(url);
  }

  updateScheduleTime(scheduleIds: string[], time: string): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      ApiHelper.orderProductSchedule.updateScheduleTime
    );

    const requestDto = {
      time: time,
      scheduleIds: scheduleIds
    };

    return this.baseHttp.post<any>(url, requestDto);
  }

  getScheduleSummaryById(id: string): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      `${ApiHelper.orderProductSchedule.schedulesSummaryById}/${id}`
    );
    return this.baseHttp.get<any>(url);
  }

  updateScheduleSummary(id: string, request: any): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      `${ApiHelper.orderProductSchedule.updateScheduleSummary}/${id}`
    );
    return this.baseHttp.put<any>(url, request);
  }

  createPatientSelfReminder(request: any): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      ApiHelper.orderProductSchedule.createSelfReminderForPatient
    );
    return this.baseHttp.post<any>(url, request);
  }

  getPatientSelfReminders(data: { startDate: string; endDate: string }): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.orderProductSchedule.base,
      ApiHelper.orderProductSchedule.getPatientSelfReminders
    );
    return this.baseHttp.post<any>(url, data);
  }


}
