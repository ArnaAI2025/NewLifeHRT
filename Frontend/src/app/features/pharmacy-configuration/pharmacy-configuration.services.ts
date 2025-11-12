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
import { PharmacyConfigurationGetAllResponseDto } from './model/pharmacy-configuration-getall-response.model';
import { IntegrationTypeResponseModel } from './model/integration-type-response.model';
import { IntegrationKeyResponseModel } from './model/integrationkey-response.model';
import { PharmacyConfigurationRequestModel } from './model/pharmacy-configuration-create-request.model';
import { PharmacyConfigurationGetByIdResponseModel } from './model/pharmacy-configuration-getbyid-response.model';
import { PharmacyConfigurationCreateUpdateResponseModel } from './model/pharmacy-configuration-create-update-response.model';


@Injectable({ providedIn: 'root' })
export class PharmacyConfigurationService {
  constructor(private baseHttp: HttpService, private http: HttpClient) { }

  getAllPharmacyConfigurations(): Observable<PharmacyConfigurationGetAllResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.pharmacyConfiguration.base,
      ApiHelper.pharmacyConfiguration.getAll
    );
    return this.baseHttp.get(url);
  }

  activatePharmacyConfigurations(pharmacyConfigurationIds: string[]): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.pharmacyConfiguration.base,
      ApiHelper.pharmacyConfiguration.activate
    );
    return this.baseHttp.post(url, { ids: pharmacyConfigurationIds });
  }

  deactivatePharmacyConfigurations(pharmacyConfigurationIds: string[]): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, ApiHelper.pharmacyConfiguration.deactivate);
    return this.baseHttp.post(url, { ids: pharmacyConfigurationIds });
  }

  deletePharmacyConfigurations(pharmacyConfigurationIds: string[]): Observable<any> {
    const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, ApiHelper.pharmacyConfiguration.delete);
    return this.baseHttp.post(url, { ids: pharmacyConfigurationIds });
  }

   getActiveIntegrationTypes(): Observable<IntegrationTypeResponseModel[]> {
    const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, 'get-all-active-integrationtypes');
    return this.baseHttp.get(url);
  }

  getIntegrationKeysByTypeId(integrationTypeId: number): Observable<IntegrationKeyResponseModel[]> {
    const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, `integration-keys/${integrationTypeId}`);
    return this.baseHttp.get(url);
  }

  createPharmacyConfiguration(request: PharmacyConfigurationRequestModel): Observable<PharmacyConfigurationCreateUpdateResponseModel> {
    const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, 'create');
    const payload = {
      PharmacyId: request.pharmacyId,
      TypeId: request.typeId,
      ConfigData: request.configData.map(d => ({ KeyId: d.keyId, Value: d.value }))
    };
    return this.baseHttp.post(url, payload);
  }

  getById(id: string): Observable<PharmacyConfigurationGetByIdResponseModel> {
  const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, `get-by-id/${id}`);
  return this.baseHttp.get(url);
}

updatePharmacyConfiguration(id: string, request: PharmacyConfigurationRequestModel): Observable<PharmacyConfigurationCreateUpdateResponseModel> {
  const url = ApiResource.getURI(ApiHelper.pharmacyConfiguration.base, `update/${id}`);
  const payload = {
    PharmacyId: request.pharmacyId,
    TypeId: request.typeId,
    ConfigData: request.configData.map(d => ({
      KeyId: d.keyId,
      Value: d.value
    }))
  };
  return this.baseHttp.put(url, payload);
}

}
