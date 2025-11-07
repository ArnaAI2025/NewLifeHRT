import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { HttpService } from '../../shared/services/http.service';
import { ApiResource } from '../../shared/constants/api-resource';
import { ApiHelper } from '../../shared/constants/api.helper';
import { ConversationResponseDto } from './model/conversation-response.model';
import { CommonOperationResponseDto } from '../../shared/models/common-operation-response.model';
import { BatchMessageRequestDto } from './model/batch-message-request.model';
import { MessageResponseDto } from './model/message-response.model';
import { BatchMessageResponseDto } from './model/batch-message-response.model';
import { BulkOperationResponseDto } from '../../shared/models/bulk-operation-response.model';



@Injectable({ providedIn: 'root' })
export class EmailAndSmsService {
    private baseHttp = inject(HttpService);
    getConversationByPatientId(id: string): Observable<ConversationResponseDto> {
      const url = ApiResource.getURI(
        ApiHelper.conversation.base,
        ApiHelper.conversation.getConversationByPatientId
      );
      return this.baseHttp.get<ConversationResponseDto>(`${url}/${id}`);
    }
    getConversationByLeadId(id: string): Observable<ConversationResponseDto> {
      const url = ApiResource.getURI(
        ApiHelper.conversation.base,
        ApiHelper.conversation.getConversationByLeadId
      );
      return this.baseHttp.get<ConversationResponseDto>(`${url}/${id}`);
    }
    sendMessage(payload: any) : Observable<CommonOperationResponseDto> {
      const url = ApiResource.getURI(ApiHelper.conversation.base, ApiHelper.conversation.create); 
      return this.baseHttp.post(url, payload); 
    }
    create(payload: BatchMessageRequestDto) : Observable<CommonOperationResponseDto> {
      const url = ApiResource.getURI(ApiHelper.batchMessage.base, ApiHelper.batchMessage.create); 
      return this.baseHttp.post(url, payload); 
    }
    getUnseenSms(counselorId : number): Observable<MessageResponseDto[]> {
      const url = ApiResource.getURI(ApiHelper.conversation.base, ApiHelper.conversation.getUnReadConversationByCounselorId)+`/${counselorId}`;
      return this.baseHttp.get<MessageResponseDto[]>(`${url}`);
    }
  markMessagesAsRead(ids: string[]): Observable<any> {
  const url = ApiResource.getURI(ApiHelper.conversation.base, ApiHelper.conversation.markMessagesAsRead);
  return this.baseHttp.patch(url, { ids });
  }
  getAllBatchMessages(): Observable<BatchMessageResponseDto[]> {
    const url = ApiResource.getURI(
      ApiHelper.batchMessage.base,
      ApiHelper.batchMessage.getAll
    );
    return this.baseHttp.get<BatchMessageResponseDto[]>(url);
  }
  getBatchMessageById(id: string): Observable<BatchMessageResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.batchMessage.base,
      ApiHelper.batchMessage.getById
    );
    return this.baseHttp.get<BatchMessageResponseDto>(`${url}${id}`);
  }
     approve(id: string, payload: BatchMessageRequestDto): Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.batchMessage.base, ApiHelper.batchMessage.approve) + `/${id}`;
  return this.baseHttp.patch(url, payload);
}

reject(id: string, payload: BatchMessageRequestDto): Observable<CommonOperationResponseDto> {
  const url = ApiResource.getURI(ApiHelper.batchMessage.base, ApiHelper.batchMessage.reject) + `/${id}`;
  return this.baseHttp.patch(url, payload);
}
  deleteBatchMessages(ids: string[]): Observable<BulkOperationResponseDto> {
    const url = ApiResource.getURI(
      ApiHelper.batchMessage.base,
      ApiHelper.batchMessage.delete
    );
    return this.baseHttp.post(url, { ids });
  }

}
