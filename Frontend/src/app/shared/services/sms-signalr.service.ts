import { Injectable, NgZone } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import * as signalR from '@microsoft/signalr';
import { AppSettingsService } from "./app-settings.service";
import { ApiResource } from "../constants/api-resource";

@Injectable({
  providedIn: 'root'
})
export class SmsSignalRService {
  private hubConnection!: signalR.HubConnection;

  private messagesMap: Map<string, any[]> = new Map();

  private messagesSubject = new BehaviorSubject<any[]>([]);
  messages$ = this.messagesSubject.asObservable();

  private activeConversationId: string | null = null;

  constructor(
    private zone: NgZone,
    private appSettingsService: AppSettingsService
  ) {}

  startConnection(patientId: string | null, leadId: string | null): void {
    const url = ApiResource.getURI('', '').replace('/api/', '/smshub');
    let query = '';
    if (patientId) {
      query = `patientId=${patientId}`;
    } else if (leadId) {
      query = `leadId=${leadId}`;
    } else {
      console.error('No patientId or leadId provided for SignalR connection');
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${url}?${query}`, {
        accessTokenFactory: () => this.appSettingsService.authToken ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR connection error:', err));

    this.hubConnection.on('ReceiveMessage', (message: any) => {
      this.zone.run(() => {
        const conversationId = message.conversationId;
        if (!this.messagesMap.has(conversationId)) {
          this.messagesMap.set(conversationId, []);
        }
        this.messagesMap.get(conversationId)?.push(message);

        if (conversationId === this.activeConversationId) {
          this.messagesSubject.next([...this.messagesMap.get(conversationId)!]);
        }
      });
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('SignalR disconnected'))
        .catch(err => console.error('SignalR disconnect error:', err));
    }
  }

  setActiveConversation(conversationId: string): void {
    this.activeConversationId = conversationId;
    const messages = this.messagesMap.get(conversationId) ?? [];
    this.messagesSubject.next([...messages]);
  }
}
