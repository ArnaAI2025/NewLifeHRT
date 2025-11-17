import { Injectable, inject } from "@angular/core";
import { HttpService } from "../../shared/services/http.service";
import { Observable } from "rxjs";
import { ApiResource } from "../../shared/constants/api-resource";
import { ApiHelper } from "../../shared/constants/api.helper";
import { OrderBulkResponseDto } from "./model/bulk-order-response.model";
import { BulkOperationResponseDto } from "../../shared/models/bulk-operation-response.model";
import { OrderResponseDto } from "./model/order.model";
import { CommonOperationResponseDto } from "../../shared/models/common-operation-response.model";
import { OrderStatus } from "../../shared/enums/order-status.enus";
import { OrderReceiptResponse } from "./model/order-receipt-response.model";
import { MarkReadyToLifeFileResponseDto } from "./model/mark-ready-lifefile-response.model";
import { OrderPaymentRequestDto } from "./model/order-payment-request.model";
import { OrderPaymentResponseDto } from "./model/order-payment-response.model";
import { DropDownResponseDto } from "../../shared/models/drop-down-response.model";
@Injectable({ providedIn: 'root' })
export class OrderService {
  private baseHttp = inject(HttpService);

  getAllOrders(patientId?: string): Observable<OrderBulkResponseDto[]> {
    const baseUrl = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getAll);
    const url = patientId ? `${baseUrl}/${patientId}` : baseUrl;
    return this.baseHttp.get<OrderBulkResponseDto[]>(url);
  }
  deleteOrders(ids: string[]): Observable<BulkOperationResponseDto> {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.delete);
    return this.baseHttp.post(url, { ids });
  }
  getOrderById(id: string): Observable<OrderResponseDto> {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getById) + '/' + id;
    return this.baseHttp.get<OrderResponseDto>(url);
  }
  createOrder(payload: any) {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.create);
    return this.baseHttp.post<CommonOperationResponseDto>(url, payload);
  }
  updateOrder(id: string | null, payload: any) {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.update)}/${id}`;
    return this.baseHttp.put<CommonOperationResponseDto>(url, payload);
  }
  updateOrderStatus(orderId: string, status: number, description?: string | null): Observable<CommonOperationResponseDto> {
    let url: string;

    switch (status) {
      case OrderStatus.Completed:
        url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.accept);
        break;
      case OrderStatus.Cancel_rejected:
        url = ApiResource.getURI(ApiHelper.proposal.base, ApiHelper.proposal.reject);
        break;
      case OrderStatus.Cancel_noMoney:
        url = ApiResource.getURI(ApiHelper.proposal.base, ApiHelper.proposal.rejectOrderNoMoney);
        break;
      default:
        throw new Error(`Unsupported order status: ${status}`);
    }

    const payload =
      (status === OrderStatus.Cancel_rejected || status === OrderStatus.Cancel_noMoney)
        ? { reason: description }
        : null;

    return this.baseHttp.patch<CommonOperationResponseDto>(`${url}/${orderId}`, payload);
  }
  getReceiptById(id: string): Observable<OrderReceiptResponse> {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getReceiptByOrderId) + '/' + id;
    return this.baseHttp.get<OrderReceiptResponse>(url);
  }
  getPrescriptionOrderById(id: string): Observable<OrderReceiptResponse> {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getPrescriptionByOrderId) + '/' + id;
    return this.baseHttp.get<OrderReceiptResponse>(url);
  }
  getSignedPrescriptionOrderById(id: string, isSigned: boolean | null): Observable<OrderReceiptResponse> {
    let url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getSignedPrescriptionByOrderId)}/${id}`;
    if (isSigned !== null && isSigned !== undefined) {
      url += `?isSigned=${isSigned}`;
    }
    return this.baseHttp.get<OrderReceiptResponse>(url);
  }
  markReadyToLifeFile(id: string): Observable<MarkReadyToLifeFileResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.markReadyToLifeFile)}/${id}`;
    return this.baseHttp.patch<MarkReadyToLifeFileResponseDto>(url, {});
  }
  generateCommission(id: string): Observable<MarkReadyToLifeFileResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.generateCommission)}/${id}`;
    return this.baseHttp.patch<MarkReadyToLifeFileResponseDto>(url, {});
  }
  updateOrderPayment(payload: OrderPaymentRequestDto): Observable<OrderPaymentResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.updatePayment)}`;
    return this.baseHttp.patch<OrderPaymentResponseDto>(url, payload);
  }
  cancelGenerateCommission(id: string): Observable<CommonOperationResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.cancelGenerateCommission)}/${id}`;
    return this.baseHttp.patch<CommonOperationResponseDto>(url, {});
  }
  refundOrder(orderId: string, refundAmount: number): Observable<CommonOperationResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.processRefund)}/${orderId}`;
    return this.baseHttp.post<CommonOperationResponseDto>(url, { refundAmount });
  }
  settleOutstandingRefund(orderId: string, settleAmount: number): Observable<CommonOperationResponseDto> {
    const url = `${ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.settleOutstandingRefund)}/${orderId}`;
    return this.baseHttp.post<CommonOperationResponseDto>(url, { settleAmount });
  }
  getAllActiveCourierServices() : Observable<DropDownResponseDto[]> {
      const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getAllCourierServices);
      return this.baseHttp.get(url);
  }
}
