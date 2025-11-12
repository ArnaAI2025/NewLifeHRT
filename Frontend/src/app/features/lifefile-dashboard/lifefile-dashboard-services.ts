import { Injectable, inject } from "@angular/core";
import { HttpService } from "../../shared/services/http.service";
import { Observable } from "rxjs";
import { ApiResource } from "../../shared/constants/api-resource";
import { ApiHelper } from "../../shared/constants/api.helper";
import { BulkOperationResponseDto } from "../../shared/models/bulk-operation-response.model";
import { CommonOperationResponseDto } from "../../shared/models/common-operation-response.model";
import { OrderStatus } from "../../shared/enums/order-status.enus";
import { OrderProcessingErrorResponseDto } from "./model/order-processing-error-response.model";
@Injectable({ providedIn: 'root' })
export class LifeFileDashboardService {
    private baseHttp = inject(HttpService);

    getAllOrderprocessingApiTrackingErrors(): Observable<OrderProcessingErrorResponseDto[]> {
      const baseUrl = ApiResource.getURI(ApiHelper.order.base, ApiHelper.order.getAllOrderprocessingApiTrackingErrors);
      return this.baseHttp.get<OrderProcessingErrorResponseDto[]>(baseUrl);
    }

}
