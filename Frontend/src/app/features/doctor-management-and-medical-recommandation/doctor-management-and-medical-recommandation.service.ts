import { Injectable } from '@angular/core';
import {  Observable, shareReplay } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import {MedicalRecommendationRequestDto} from './model/medical-recommadation-request.model'
import { CounselorNoteRequest } from './model/counselor-note-request.model';
import { PatientResponseDto } from '../patient/model/patient-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';


@Injectable({ providedIn: 'root' })
export class DoctorManagementAndMedicalRecommandationService {
  constructor(private baseHttp: HttpService) {}
  private _medicationType$?: Observable<DropDownResponseDto[]>;
  private _followUpTests$?: Observable<DropDownResponseDto[]>;
getPatientById(id: string): Observable<PatientResponseDto> {
  const url = ApiResource.getURI(ApiHelper.patient.base, ApiHelper.patient.getUserById);
  return this.baseHttp.get<PatientResponseDto>(`${url}${id}`);
}
  getAllCounselorBasedOnPatientId(patientId: string | null): Observable<any[]> {
  const url = `${ApiResource.getURI(ApiHelper.counselorNote.base, ApiHelper.counselorNote.getAllCounselorBasedOnPatientId)}/${patientId}`;
  return this.baseHttp.get(url); 
}
createCouncelorNote(payload: CounselorNoteRequest) {
  const url = ApiResource.getURI(ApiHelper.counselorNote.base, ApiHelper.counselorNote.add); 
  return this.baseHttp.post(url, payload); 
}
deleteCounselorNote(patientId: string) {
  const url = `${ApiResource.getURI(ApiHelper.counselorNote.base, ApiHelper.counselorNote.deleteNotes)}/${patientId}`;
  return this.baseHttp.delete(url); 
}
 
getAllMedicationType(): Observable<DropDownResponseDto[]> {
    if (!this._medicationType$) {
      const url = ApiResource.getURI(
        ApiHelper.medicalRecommendation.base,
        ApiHelper.medicalRecommendation.getAllMedicationType
      );
      this._medicationType$ = this.baseHttp.get<DropDownResponseDto[]>(url).pipe(
        shareReplay(1)
      );
    }
    return this._medicationType$;
  }

  getAllFollowUpTests(): Observable<DropDownResponseDto[]> {
    if (!this._followUpTests$) {
      const url = ApiResource.getURI(
        ApiHelper.medicalRecommendation.base,
        ApiHelper.medicalRecommendation.getAllFollowUpTests
      );
      this._followUpTests$ = this.baseHttp.get<DropDownResponseDto[]>(url).pipe(
        shareReplay(1)
      );
    }
    return this._followUpTests$;
  }
medicalRecommandation(payload: MedicalRecommendationRequestDto, isEdit: boolean): Observable<CommonOperationResponseDto> {
  const baseUrl = ApiResource.getURI(ApiHelper.medicalRecommendation.base, '');
  
  if (isEdit) {
    const updateUrl = ApiResource.getURI(ApiHelper.medicalRecommendation.base, ApiHelper.medicalRecommendation.update);
    return this.baseHttp.put(updateUrl, payload);
  } else {
    const createUrl = ApiResource.getURI(ApiHelper.medicalRecommendation.base, ApiHelper.medicalRecommendation.create);
    return this.baseHttp.post(createUrl, payload);
  }
}

getMedicalRecommendationById(id : string) : Observable<MedicalRecommendationRequestDto>
{
  const url = `${ApiResource.getURI(ApiHelper.medicalRecommendation.base, ApiHelper.medicalRecommendation.get)}/${id}`;
  return this.baseHttp.get(url); 
}
  getAllMedicationTypeBasedOnPatientId(patientId: string | null): Observable<any[]> {
  const url = `${ApiResource.getURI(ApiHelper.medicalRecommendation.base, ApiHelper.medicalRecommendation.getAllBasedOnPatientId)}/${patientId}`;
  return this.baseHttp.get(url); 
}
  deleteMedicationRecommendation(patientId: string | null): Observable<any[]> {
  const url = `${ApiResource.getURI(ApiHelper.medicalRecommendation.base, ApiHelper.medicalRecommendation.delete)}/${patientId}`;
  return this.baseHttp.delete(url); 
}
}
