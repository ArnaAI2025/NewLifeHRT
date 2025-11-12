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
import { ReminderTypesResponseDto } from './model/get-all-reminder-types.response.model';
import { RecurrenceRulesResponseDto } from './model/get-all-recurrence-rule.response.model';
import { CreateReminderRequestDto } from './model/create-reminder.request.model';
import { ReminderDashboard } from './model/reminder-dashboard.response.model';

@Injectable({ providedIn: 'root' })
export class ReminderService {
  constructor(private baseHttp: HttpService, private http: HttpClient) { }

  getAllReminderTypes(): Observable<ReminderTypesResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.reminder.base,
      ApiHelper.reminder.getAllReminderTypes
    );
    return this.baseHttp.get<ReminderTypesResponseDto[]>(url);
  }

  getAllRecurrenceRules(): Observable<RecurrenceRulesResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.reminder.base,
      ApiHelper.reminder.getAllRecurrenceRules
    );
    return this.baseHttp.get<RecurrenceRulesResponseDto[]>(url);
  }

  createReminder(request: CreateReminderRequestDto): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.reminder.base,
      ApiHelper.reminder.create
    );
    return this.baseHttp.post(url, request);
  }

  getAllActiveRemindersForPatients(): Observable<ReminderDashboard[]> {
    const url = ApiResource.getURI(ApiHelper.reminder.base, ApiHelper.reminder.allActiveRemindersForPatient);
    return this.baseHttp.get<ReminderDashboard[]>(url);
  }

  getAllActiveRemindersForLeads(): Observable<ReminderDashboard[]> {
    const url = ApiResource.getURI(ApiHelper.reminder.base, ApiHelper.reminder.allActiveRemindersForLeads);
    return this.baseHttp.get<ReminderDashboard[]>(url);
  }

  markReminderAsCompleted(reminderId: string): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.reminder.base, ApiHelper.reminder.markReminderasCompleted) + `/${reminderId}`;
    return this.baseHttp.put<any>(url, {});
  }
}

