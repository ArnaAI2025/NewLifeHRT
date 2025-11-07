import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { HttpService } from '../../shared/services/http.service';
import { PoolDetailResponseDto } from './model/pool-detail-response-model';
import { CommissionsPayableResponse } from './model/commissions-payable-response.model';

@Injectable({ providedIn: 'root' })
export class CommissionService {
  private baseHttp = inject(HttpService);

  getAllPoolDetails(fromDate?: Date, toDate?: Date): Observable<PoolDetailResponseDto[]> {
    const query: any = {};

    if (fromDate) {
      const localFrom = new Date(fromDate);
      localFrom.setHours(0, 0, 0, 0);
      query['fromDate'] = localFrom.toISOString();
    }

    if (toDate) {
      const localTo = new Date(toDate);
      localTo.setHours(23, 59, 59, 999);
      query['toDate'] = localTo.toISOString();
    }

    const url = ApiResource.getURI(
      ApiHelper.commission.base,
      ApiHelper.commission.getCounselorByDate,
      undefined,
      query
    );

    return this.baseHttp.get<PoolDetailResponseDto[]>(url);
  }

  // FIXED: Provide the method used by the component
  getCommissionsByPoolDetailId(poolDetailId: string): Observable<CommissionsPayableResponse[]> {
    const base = ApiResource.getURI(
      ApiHelper.commission.base,
      ApiHelper.commission.getCommissionsByPoolDetailId
    );
    const url = `${base}/${encodeURIComponent(poolDetailId)}`;
    return this.baseHttp.get<CommissionsPayableResponse[]>(url);
  }
  getCommissionDetailById(commissionsPayableId: string) {  
  const url = ApiResource.getURI(
    ApiHelper.commission.base,
    ApiHelper.commission.getCommissionById
  )+`/${commissionsPayableId}`;
  return this.baseHttp.get<any>(url); 
}

}
