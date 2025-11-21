import { Injectable } from '@angular/core';
import {  Observable, shareReplay } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { PatientResponseDto } from './model/patient-response.model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';
import { PatientAttachmentResponseDto } from './model/patient-attachment-response.model ';
import { ShippingAddressRequestDto } from './model/shipping-address.model';
import { ShippingAddressResponseDto } from './model/shipping-address-response.model';
import { PatientCreditCardResponseDto } from './model/patient-credit-card-response';
import { PatientLeadCommunicationDto } from './model/patient-lead-communication.response.model';
import { PatientCounselorInfoDto } from './model/patient-counselor-info';

@Injectable({ providedIn: 'root' })
export class PatientService {
  constructor(private baseHttp: HttpService) {}

  getAllPatients(): Observable<PatientResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getAllPatient
    );
    return this.baseHttp.get(url);
  }
  getPatientById(id: string): Observable<PatientResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getUserById
    );
    return this.baseHttp.get<PatientResponseDto>(`${url}${id}`);
  }

  bulkDeletePatients(ids: string[]): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.delete
    );
    return this.baseHttp.post(url, { ids });
  }

  getAllActiveVisitType(): Observable<DropDownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getAllActiveVisitType
    );
    return this.baseHttp.get(url);
  }
  getAllActiveDocumentCategory(): Observable<DropDownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getAllActiveDocumentCategory
    );
    return this.baseHttp.get(url);
  }
  getAllActiveAgenda(): Observable<DropDownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getAllActiveAgenda
    );
    return this.baseHttp.get(url);
  }
  createPatient(payload: any): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.add
    );
    return this.baseHttp.post(url, payload);
  }
  updatePatient(
    patientId: string,
    payload: any
  ): Observable<CommonOperationResponseDto> {
    const url =
      ApiResource.getURI(ApiHelper.patient.base, ApiHelper.patient.update) +
      `/${patientId}`;
    return this.baseHttp.put(url, payload);
  }
  bulkToggleActive(ids: string[], action: boolean): Observable<any> {
    const endpoint = action
      ? ApiHelper.patient.deactivateBulk
      : ApiHelper.patient.activateBulk;

    const url = ApiResource.getURI(ApiHelper.patient.base, endpoint);
    return this.baseHttp.patch(url, { ids });
  }
  togglePatientStatusById(
    id: string,
    isActivating: boolean
  ): Observable<CommonOperationResponseDto> {
    const endpoint = isActivating
      ? ApiHelper.patient.activate
      : ApiHelper.patient.deactivate;
    const url = `${ApiResource.getURI(ApiHelper.patient.base, endpoint)}/${id}`;

    return this.baseHttp.patch<CommonOperationResponseDto>(url, {});
  }
  getAllActivePatients(patientId?: string): Observable<DropDownResponseDto[]> {
    const baseUri = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getAllActivePatient
    );
    const url = patientId
      ? `${ApiResource.getURI(
          ApiHelper.patient.base,
          ApiHelper.patient.getAllActivePatient
        )}/${patientId}`
      : baseUri;
    return this.baseHttp.get<DropDownResponseDto[]>(url);
  }

  deletePatient(patientId: string) {
    const url = `${ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.delete
    )}/${patientId}`;
    return this.baseHttp.delete(url);
  }
  bulkAssignee(
    ids: string[],
    assigneeId: number
  ): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.bulkAssignee
    );
    return this.baseHttp.patch(url, { ids: ids, id: assigneeId });
  }
  uploadFiles(payload: FormData) {
    const url = ApiResource.getURI(
      ApiHelper.documentUpload.base,
      ApiHelper.documentUpload.uploadDocument
    );
    return this.baseHttp.post(url, payload);
  }
  loadUploadedDocuments(
    patientId: string
  ): Observable<PatientAttachmentResponseDto[]> {
    const url =
      ApiResource.getURI(
        ApiHelper.documentUpload.base,
        ApiHelper.documentUpload.getAllDocuments
      ) + `/${patientId}`;
    return this.baseHttp.get(url);
  }
  getFileUrl(
    patientDocumentAttchmentId: string
  ): Observable<CommonOperationResponseDto> {
    const url = `${ApiResource.getURI(
      ApiHelper.documentUpload.base,
      ApiHelper.documentUpload.getDocumentId
    )}/${patientDocumentAttchmentId}`;
    return this.baseHttp.get(url);
  }
  deleteFiles(fileIds: string[]): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.documentUpload.base,
      ApiHelper.documentUpload.deleteDocument
    );
    const payload = { ids: fileIds };
    return this.baseHttp.patch<CommonOperationResponseDto>(url, payload);
  }
  // downloadFiles(fileIds: string[]): Observable<CommonOperationResponseDto[]> {
  //   const url = ApiResource.getURI(ApiHelper.documentUpload.base, ApiHelper.documentUpload.downloadDocument);
  //   const payload = { ids: fileIds };
  //   return this.baseHttp.post<CommonOperationResponseDto[]>(url, payload);
  // }
  createShippingAddress(
    payload: ShippingAddressRequestDto
  ): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.create
    );
    return this.baseHttp.post(url, payload);
  }
  updateShippingAddress(
    payload: ShippingAddressRequestDto
  ): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.update
    );
    return this.baseHttp.put(url, payload);
  }

  getByShippingId(id: string): Observable<ShippingAddressResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.getShippingAddressById
    );
    return this.baseHttp.get<ShippingAddressResponseDto>(`${url}/${id}`);
  }
  getAllAddressBasedOnPatientId(
    id: string | null
  ): Observable<ShippingAddressResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.getAllShippingAddressBasedOnPatientId
    );
    return this.baseHttp.get<ShippingAddressResponseDto[]>(`${url}/${id}`);
  }
  bulkDeleteShippingAddress(
    ids: string[]
  ): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.bulkDelete
    );
    return this.baseHttp.post(url, { ids: ids });
  }
  bulkToggleShippingAddressActive(
    ids: string[],
    action: boolean
  ): Observable<any> {
    const endpoint = action
      ? ApiHelper.shippingAddress.bulkActive
      : ApiHelper.shippingAddress.bulkInactive;

    const url = ApiResource.getURI(ApiHelper.shippingAddress.base, endpoint);
    return this.baseHttp.patch(url, { ids });
  }
  getCreditCardByPatientId(
    id: string
  ): Observable<PatientCreditCardResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getCreditCardByPatientId
    );
    return this.baseHttp.get<PatientCreditCardResponseDto[]>(`${url}/${id}`);
  }
  getAllActiveCountries(): Observable<DropDownResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.shippingAddress.base,
      ApiHelper.shippingAddress.getAllCountries
    );
    return this.baseHttp.get(url);
  }
  getAllActiveStates(countryId: number): Observable<DropDownResponseDto[]> {
    const url =
      ApiResource.getURI(
        ApiHelper.shippingAddress.base,
        ApiHelper.shippingAddress.getAllStatesByCountryId
      ) +
      '/' +
      countryId;
    return this.baseHttp.get(url);
  }
  getPatientsByCounselorId(
    counselorId: number | null
  ): Observable<PatientLeadCommunicationDto[]> {
    const url =
      ApiResource.getURI(
        ApiHelper.patient.base,
        ApiHelper.patient.getPatientsByCounselorId
      ) +
      '/' +
      counselorId;
    return this.baseHttp.get(url);
  }

  getAllPatientsCounselorInfo(): Observable<PatientCounselorInfoDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getPatientCounselorinfo
    );
    return this.baseHttp.get(url);
  }
  getPhysianInfoBasedOnPatientId(id?: string): Observable<DropDownResponseDto> {
    const baseUrl = ApiResource.getURI(
      ApiHelper.patient.base,
      ApiHelper.patient.getPhysianInfoBasedOnPatientId
    );
    const url = id ? `${baseUrl}/${id}` : baseUrl;
    return this.baseHttp.get<DropDownResponseDto>(url);
  }
}
