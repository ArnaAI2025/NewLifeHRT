import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { LeadDisplay } from './model/lead-display.model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import { CreatePatientResponseDto } from '../patient/model/create-patient-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';
import { PatientLeadCommunicationDto } from '../patient/model/patient-lead-communication.response.model';



@Injectable({ providedIn: 'root' })
export class LeadManagementService {
    private baseHttp = inject(HttpService);
    
getAllLeads(): Observable<LeadDisplay[]> {
     const url = ApiResource.getURI(
      ApiHelper.lead.base,
      ApiHelper.lead.getAllLeads
    );
  return this.baseHttp.get<LeadDisplay[]>(url);
}
getLeadById(id: string): Observable<LeadDisplay> {
  const url = ApiResource.getURI(
    ApiHelper.lead.base,
    ApiHelper.lead.getById
  );
  return this.baseHttp.get<LeadDisplay>(`${url}${id}`);
}

getAllActiveSalesPerson() : Observable<DropDownResponseDto[]>
{
    const url = ApiResource.getURI(ApiHelper.user.base, ApiHelper.user.getAllActiveSalesPerson);
    return this.baseHttp.get(url);
}

bulkDeleteLeads(ids : string[]) : Observable<CommonOperationResponseDto>
{
  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.delete);
  return this.baseHttp.post(url,{ids});
}

createLead(payload: LeadDisplay) : Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.createLead); 
  return this.baseHttp.post(url, payload); 
}
updateLead( id : string,payload: LeadDisplay) : Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.updateLead) + `/${id}`; 
  return this.baseHttp.put(url, payload); 
}
bulkToggleActive(ids: string[], action: boolean): Observable<any> {
  const endpoint = action
    ? ApiHelper.lead.activateBulk
    : ApiHelper.lead.deactivateBulk;

  const url = ApiResource.getURI(ApiHelper.lead.base, endpoint);
  return this.baseHttp.patch(url, { ids });
}
bulkAssignee(ids: string[], assigneeId : number):  Observable<CommonOperationResponseDto> {

  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.bulkAssignee);
  return this.baseHttp.patch(url, { ids: ids, id : assigneeId }); 
}
bulkConvertToPatient(ids: string[]): Observable<CommonOperationResponseDto> {

  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.bulkConvertToPatient);
  return this.baseHttp.patch(url, { ids });
}
bulkToggleIsQualified(ids: string[]): Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.disqualifyLead);
  return this.baseHttp.patch(url, { ids });
}
getLeadsByCounselorId(counselorId : number | null) : Observable<PatientLeadCommunicationDto[]>
{
    const url = ApiResource.getURI(ApiHelper.lead.base, ApiHelper.lead.leadsByCounselorId)+'/'+counselorId;
    return this.baseHttp.get(url);
}
}
