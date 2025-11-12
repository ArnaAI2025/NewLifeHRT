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
import { AppointmentResponseDto } from './model/appointment-response-model';
import { AppointmentServiceResponse } from './model/appointment-service-response.model';
import { AppointmentModeResponse } from './model/appointment-modes.model';
import { SlotResponse } from './model/appointment-slots-response.model';
import { CreateAppointmentRequestDto } from './model/create-appointment-request.model';
import { AppointmentResultResponseDto } from './model/appointment-response-result.model';
import { AppointmentGetResponseDto } from './model/appointment-get-response.model';
import { GetAllHolidaysRequestDto } from './model/holidays-request.model';
import { HolidayResponseDto } from './model/holiday-response.model';
import { GetAllAppointmentsRequestDto } from './model/get-all-appointments-request.model';
import { AppointmentGetByPatientIdResponseDto } from './model/appointment-get-by-patientId-response.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  constructor(private baseHttp: HttpService, private http : HttpClient) {}

  getAppointments(data:GetAllAppointmentsRequestDto): Observable<AppointmentResponseDto[]>
  {
    const url = ApiResource.getURI(ApiHelper.appointment.base, ApiHelper.appointment.getAllAppointments);
    return this.baseHttp.post<AppointmentResponseDto[]>(url, data);
  }

  deleteAppointment(appointmentId: string) {
    const url = `${ApiResource.getURI(ApiHelper.appointment.base, ApiHelper.appointment.deleteAppointment)}/${appointmentId}`;
    return this.baseHttp.delete(url);
  }

  getAllClinicAppointmentServices() : Observable<AppointmentServiceResponse[]>
  {
      const url = ApiResource.getURI(ApiHelper.clinicService.base, ApiHelper.clinicService.getAllServiceTypes);
      return this.baseHttp.get(url);
  }

  getAllAppointmentModes() : Observable<AppointmentModeResponse[]>
  {
      const url = ApiResource.getURI(ApiHelper.appointment.base, ApiHelper.appointment.getAllAppointmentModes);
      return this.baseHttp.get(url);
  }

  getAppointmentSlots(serviceLinkId:string,doctorId:string,appointmentDate:Date): Observable<SlotResponse[]>
  {
    const params = {
    serviceLinkId:serviceLinkId,
    doctorId:doctorId,
    appointmentDate:appointmentDate.toDateString()
  };
    const url = ApiResource.getURI(ApiHelper.appointment.base, ApiHelper.appointment.getAllSlots);
    return this.baseHttp.get<SlotResponse[]>(url, { params });
  }

  createAppointment(data: CreateAppointmentRequestDto) : Observable<AppointmentResultResponseDto> {
    const url = ApiResource.getURI(ApiHelper.appointment.base, ApiHelper.appointment.create);
    return this.baseHttp.post(url, data);
  }

  updateAppointment(id: string, data:CreateAppointmentRequestDto) : Observable<AppointmentResultResponseDto>{
    const url = ApiResource.getURI(ApiHelper.appointment.base, `${ApiHelper.appointment.update}/${id}`);
    return this.baseHttp.put(url,data);
  }

  getAppointmentById(id: string) : Observable<AppointmentGetResponseDto> {
    const url = ApiResource.getURI(ApiHelper.appointment.base, `${ApiHelper.appointment.getById}/${id}`);
    return this.baseHttp.get<AppointmentGetResponseDto>(url);
  }

  getAllHolidays(data: GetAllHolidaysRequestDto): Observable<HolidayResponseDto[]> {
    const url = ApiResource.getURI(ApiHelper.holiday.base, ApiHelper.holiday.getAll);
    return this.baseHttp.post<HolidayResponseDto[]>(url, data);
  }

  getAppointmentsByPatientId(patientId: string) : Observable<AppointmentGetByPatientIdResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.appointment.base, `${ApiHelper.appointment.getAppointmentsByPatientId}/${patientId}`);
  return this.baseHttp.get<AppointmentGetByPatientIdResponseDto[]>(url);
}


}
