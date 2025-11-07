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
import { UserResponseDto } from './model/user-response.model';
import { createUserResponseDto } from './model/create-user-response.model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';
import { U } from '@angular/cdk/keycodes';
import { TimezoneResponseDto } from './model/timezone-response.model';


@Injectable({ providedIn: 'root' })
export class UserManagementService {
  constructor(private baseHttp: HttpService) {}
clinicServiceByRoleType(serviceName: string) {
  const url = ApiResource.getURI(
    ApiHelper.clinicService.base,
    ApiHelper.clinicService.getAllServiceByType,
    serviceName
  );
  return this.baseHttp.get(url);
}
addUser(payload: any) : Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.add);
  return this.baseHttp.post(url, payload);
}
updateUser(id : number,payload: any) : Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.update)+ `/${id}`;
  return this.baseHttp.put(url, payload);
}
 getAllUsers(roleId: number): Observable<UserResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getAllUser) + `/${roleId}`;
  return this.baseHttp.get(url);
}
getUserById(id : number) : Observable<UserResponseDto>
{
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getUserById) + `${id}`;
  return this.baseHttp.get(url);
}
deleteById(id : number) : Observable<CommonOperationResponseDto>
{
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.delete) + `/${id}`;
  return this.baseHttp.delete(url);
}

bulkToggleActive(ids: number[], action: boolean): Observable<any> {
  const endpoint = action
    ? ApiHelper.user.deactivateBulk
    : ApiHelper.user.activateBulk;

  const url = ApiResource.getURI(ApiHelper.user.base, endpoint);
  return this.baseHttp.patch(url, { ids });
}
bulkDelete(ids: number[]): Observable<any> {
  const endpoint = ApiHelper.user.deleteBulk;

  const url = ApiResource.getURI(ApiHelper.user.base, endpoint);
  return this.baseHttp.post(url, { ids });
}
getAllActiveDoctors() : Observable<DropDownResponseDto[]>
{
    const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getAllActiveDoctors);
    return this.baseHttp.get(url);
}
getAllActiveSalesPerson() : Observable<DropDownResponseDto[]>
{
    const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getAllActiveSalesPerson);
    return this.baseHttp.get(url);
}
getAllActiveUsers(roleId: number, searchTerm: string = ''): Observable<DropDownResponseDto[]> {
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getActiveUsers);

  const requestBody = {
    roleId: roleId,
    searchTerm: searchTerm
  };

  return this.baseHttp.post<DropDownResponseDto[]>(url, requestBody);
}

getAllVacationUsers() : Observable<DropDownResponseDto[]>
{
  const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getAllVacationUsers);
  return this.baseHttp.get(url);
}

getAllTimezones(): Observable<TimezoneResponseDto[]>
{
  const url = ApiResource.getURI(ApiHelper.timezone.base, ApiHelper.timezone.getAll);
  return this.baseHttp.get(url);
}

}
