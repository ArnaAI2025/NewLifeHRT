import { Injectable, inject } from '@angular/core';
import { HttpService } from '../../shared/services/http.service';
import { Observable } from 'rxjs';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { CouponRequest } from './model/coupon-request.model';
import { CouponResponse } from './model/coupon-response.model';
import { DropDownResponseDto } from '../../shared/models/drop-down-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';
import { BulkOperationResponseDto } from '../../shared/models/bulk-operation-response.model';
import { ProposalBulkResponseDto } from './model/proposal-bulk-response.model';
import { HttpParams } from '@angular/common/http';
import { Status } from '../../shared/enums/status.enum';
import { ProposalDetailRequestDto } from './model/proposal-detail-request.model';
@Injectable({ providedIn: 'root' })
export class ProposalService {
  Status = Status;
  private baseHttp = inject(HttpService);
  bulkToggleActive(ids: string[], action: boolean): Observable<any> {
    const endpoint = action
      ? ApiHelper.coupon.activateCoupon
      : ApiHelper.coupon.deactivateCoupon;

    const url = ApiResource.getURI(ApiHelper.coupon.base, endpoint);
    return this.baseHttp.patch(url, { ids });
  }

  addCoupon(payload: CouponRequest): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.createCoupon
    );
    return this.baseHttp.post<CommonOperationResponseDto>(url, payload);
  }
  getCouponById(id: string): Observable<CouponResponse> {
    const url = ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.getCouponById
    );
    return this.baseHttp.get<CouponResponse>(`${url}/${id}`);
  }
  updateCoupon(id: string, payload: CouponRequest) {
    const url = ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.updateCoupon
    );
    return this.baseHttp.put<CommonOperationResponseDto>(
      `${url}/${id}`,
      payload
    );
  }
  deleteCoupon(ids: string[]): Observable<any> {
    const url = ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.delete
    );
    return this.baseHttp.post(url, { ids });
  }

  getAllCoupon(): Observable<CouponResponse[]> {
    const url = ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.getAllCoupons
    );
    return this.baseHttp.get(url);
  }

  getCouponsForDropDown(): Observable<CouponResponse[]> {
    const url = `${ApiResource.getURI(
      ApiHelper.coupon.base,
      ApiHelper.coupon.getCoupons
    )}`;
    return this.baseHttp.get<CouponResponse[]>(url);
  }
  createProposal(payload: any) {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.create
    );
    return this.baseHttp.post<CommonOperationResponseDto>(url, payload);
  }
  updateProposal(id: string | null, payload: any) {
    const url = `${ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.update
    )}/${id}`;
    return this.baseHttp.put<CommonOperationResponseDto>(url, payload);
  }

  deleteProposal(ids: string[]): Observable<BulkOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.delete
    );
    return this.baseHttp.post(url, { ids });
  }
  getProposalById(proposalId: string | null) {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.get
    );
    return this.baseHttp.get<any>(`${url}/${proposalId}`);
  }

  getAllProposals(ids?: number[], patientId?: string | null): Observable<ProposalBulkResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.getAllPropsals
    );

    const params: Record<string, any> = {};

    if (ids && ids.length > 0) {
      params['ids'] = ids;
    }

    if (patientId) {
      params['patientId'] = patientId;
    }

    return this.baseHttp.get<ProposalBulkResponseDto[]>(url, { params });
  }

  bulkAssignee(
    ids: string[],
    assigneeId: number
  ): Observable<CommonOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.bulkAssignee
    );
    return this.baseHttp.patch(url, { ids: ids, id: assigneeId });
  }
  updateProposalStatus(
    proposalId: string,
    status: number,
    description?: string | null
  ): Observable<CommonOperationResponseDto> {
    let endpoint: string;

    switch (status) {
      case Status.Approved:
        endpoint = ApiHelper.proposal.acceptProposal;
        break;
      case Status.Canceled:
        endpoint = ApiHelper.proposal.cancelProposal;
        break;
      case Status.Rejected:
        endpoint = ApiHelper.proposal.rejectProposal;
        break;
      case Status.ApprovedByPatient:
        endpoint = ApiHelper.proposal.acceptByPatientProposal;
        break;
      case Status.RejectedByPatient:
        endpoint = ApiHelper.proposal.rejectByPatientProposal;
        break;
      default:
        throw new Error(`Unsupported proposal status: ${status}`);
    }

    const url = ApiResource.getURI(ApiHelper.proposal.base, endpoint);
    const payload = status === Status.Rejected || Status.RejectedByPatient ? `"${description}"` : null;
    return this.baseHttp.patch<CommonOperationResponseDto>(
      `${url}/${proposalId}`,
      payload
    );
  }
  cloneOrder(orderId: string | null) {
    const url = `${ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.clone
    )}/${orderId}`;
    return this.baseHttp.post<any>(url, null);
  }
  getAllPropsalsOnPatientId(
    id?: string
  ): Observable<ProposalBulkResponseDto[]> {
    const baseUrl = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.getAllPropsalsOnPatientId
    );
    const url = id ? `${baseUrl}/${id}` : baseUrl;
    return this.baseHttp.get<ProposalBulkResponseDto[]>(url);
  }
  updateProposalDetails(proposalId: string, proposalDetails: ProposalDetailRequestDto[]): Observable<BulkOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.proposal.base,
      ApiHelper.proposal.updateProposalDetails
    );
    return this.baseHttp.patch(`${url}/${proposalId}`, { ids: proposalDetails });
  }

}
