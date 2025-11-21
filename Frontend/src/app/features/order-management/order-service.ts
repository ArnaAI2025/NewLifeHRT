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
import { MarkReadyToLifeFileResponseDto } from "./model/mark-ready-lifefile-response.model";
import { OrderPaymentRequestDto } from "./model/order-payment-request.model";
import { OrderPaymentResponseDto } from "./model/order-payment-response.model";
import { DropDownResponseDto } from "../../shared/models/drop-down-response.model";
import { PrescriptionReceiptDto } from "./prescription/prescription.model";
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
  getReceiptById(id: string | null): Observable<PrescriptionReceiptDto> {
    const url = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getReceiptByOrderId) + '/' + id;
    return this.baseHttp.get<PrescriptionReceiptDto>(url);
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
  getOrderTemplate(orderId: string | null, isScheduleDrug?: boolean): Observable<PrescriptionReceiptDto> {
    const queryParams: any = {};
    if (isScheduleDrug !== undefined) {
      queryParams.isScheduleDrug = isScheduleDrug;
    }
    const url = ApiResource.getURI(ApiHelper.order.base,`${orderId}/${ApiHelper.order.getPrescription}`,undefined,queryParams);
    return this.baseHttp.get<PrescriptionReceiptDto>(url);
  }

  downloadOrderPdf(orderId: string|null, isScheduleDrug?: boolean,isReceipt?:boolean) {
    const params: Record<string, any> = {};
    if (isScheduleDrug !== undefined) {
      params["isScheduleDrug"] = isScheduleDrug;
    }
    if(isReceipt != undefined){
      params["isReceipt"] = isReceipt;
    }
    const url = ApiResource.getURI(ApiHelper.order.base,`${orderId}/${ApiHelper.order.downloadOrderPdf}`,undefined,params);
    return this.baseHttp.get(url, {
      responseType: 'blob' as 'blob'
    });
  }
  
}
