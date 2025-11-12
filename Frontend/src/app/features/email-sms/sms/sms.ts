import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnInit,
  ViewChild,
  OnDestroy,
  ChangeDetectorRef,
  signal,
  CUSTOM_ELEMENTS_SCHEMA
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConversationResponseDto } from '../model/conversation-response.model';
import { EmailAndSmsService } from '../email-sms.services';
import { MessageResponseDto } from '../model/message-response.model';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SmsSignalRService } from '../../../shared/services/sms-signalr.service';
import { PatientNavigationBarComponent } from '../../../shared/components/patient-navigation-bar/patient-navigation-bar';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import 'emoji-picker-element';
import { SmsMessage, Sender } from '../model/sms-message.model';
import { LeadNavigationBar } from '../../lead-management/lead-navigation-bar/lead-navigation-bar';
import { ConfirmationDialogService } from '../../../shared/components/confirmation-dialog/confirmation-dialog.services';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-sms-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, PatientNavigationBarComponent, PickerModule, LeadNavigationBar],
  templateUrl: './sms.html',
  styleUrls: ['./sms.scss'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class Chat implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('thread') thread!: ElementRef<HTMLDivElement>;
  @ViewChild('draftInput') draftInput!: ElementRef<HTMLTextAreaElement>;

  patientId: string | null = null;
  leadId: string | null = null;

  conversationInformation: ConversationResponseDto | null = null;
  messages = signal<SmsMessage[]>([]);
  typing = false;
  draft = '';
  emojiPickerOpen = false;

  private emailAndSmsService = inject(EmailAndSmsService);
  private route = inject(ActivatedRoute);
  private smsSignalRService = inject(SmsSignalRService);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private subscription?: Subscription;
  private confirmationService = inject(ConfirmationDialogService);
  
ngOnInit(): void {
  this.patientId = this.route.snapshot.paramMap.get('patientId');
  this.leadId = this.route.snapshot.paramMap.get('leadId');
  if (!this.patientId && !this.leadId) return;

  const getConversation$ = this.patientId
    ? this.emailAndSmsService.getConversationByPatientId(this.patientId)
    : this.emailAndSmsService.getConversationByLeadId(this.leadId!);

  getConversation$.subscribe({
    next: (res) => {
      this.conversationInformation = res;
      this.messages.set(res.messages.map(this.mapMessage));
      this.scrollToBottom();

      if (!this.conversationInformation.phoneNumber || this.conversationInformation.phoneNumber.trim() === '') {
        this.confirmationService.openConfirmation({
          title: 'Phone Number Missing',
          message: 'No phone number present. Please add a phone number to continue.',
          confirmButtonText: 'OK',
          showCancelButton: false
        }).subscribe(() => {
        });
        //return;
      }

      this.smsSignalRService.startConnection(this.patientId ?? null, this.leadId ?? null);

      if (this.conversationInformation.conversationId) {
        this.smsSignalRService.setActiveConversation(this.conversationInformation.conversationId);
      }

      this.subscription = this.smsSignalRService.messages$.subscribe(newMessages => {
        const lastMessage = newMessages[newMessages.length - 1];
        if (lastMessage) {
          const mapped = this.mapMessage(lastMessage as MessageResponseDto);
          this.messages.update(msgs => [...msgs, mapped]);
          this.scrollToBottom();
          this.cdr.detectChanges();
        }
      });
    },
    error: (err) => console.error('Error loading conversation', err),
  });
}


  private getInitials(name?: string | null): string {
    if (!name) return 'N';
    const parts = name.trim().split(' ');
    if (parts.length === 0) return 'N';
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  }

  private mapMessage = (m: MessageResponseDto): SmsMessage => {
    const sender: Sender = m.direction === 'Outbound' ? 'clinic' : 'patient';

    const counselorInitials = sender === 'clinic' ? this.getInitials(m.counselorName) : undefined;

    const textContents = m.messageContents?.filter(c => c.contentType === 'text').map(c => c.content) || [];
    const text = textContents.length ? textContents.join('\n') : undefined;

    const mediaContents = m.messageContents?.filter(c =>
      c.contentType !== 'text'
    ).map(c => ({
      contentType: c.contentType,
      content: c.content
    })) || [];

    return {
      sender,
      text,
      html: undefined,
      list: undefined,
      mediaContents,
      time: new Date(m.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      status: m.isRead ? 'Read' : m.isSent ? 'Delivered' : undefined,
      counselorInitials,
    };
  };

  textContentExists(m: SmsMessage): boolean {
    return !!m.text && m.text.trim().length > 0;
  }

  isUrl(str: string | undefined): boolean {
    if (!str) return false;
    try {
      const url = new URL(str);
      return url.protocol === 'https:' || url.protocol === 'http:';
    } catch {
      return false;
    }
  }

  isImage(contentType: string | undefined): boolean {
    return !!contentType && contentType.startsWith('image/');
  }

  isVideo(contentType: string | undefined): boolean {
    return !!contentType && contentType.startsWith('video/');
  }

  isAudio(contentType: string | undefined): boolean {
    return !!contentType && contentType.startsWith('audio/');
  }

  toggleEmojiPicker(): void {
    this.emojiPickerOpen = !this.emojiPickerOpen;
  }

  addEmoji(event: any): void {
    const emoji = event.detail?.unicode;
    if (!emoji) return;

    const textarea: HTMLTextAreaElement = this.draftInput.nativeElement;
    if (textarea) {
      const start = textarea.selectionStart;
      const end = textarea.selectionEnd;
      this.draft = this.draft.substring(0, start) + emoji + this.draft.substring(end);
      this.cdr.detectChanges();
      setTimeout(() => {
        textarea.selectionStart = textarea.selectionEnd = start + emoji.length;
        textarea.focus();
      }, 0);
    } else {
      this.draft += emoji;
      this.cdr.detectChanges();
    }
    this.emojiPickerOpen = false;
  }

send(): void {
  const text = this.draft.trim();
  if (!text || !this.conversationInformation) return;

  if (!this.conversationInformation.phoneNumber || this.conversationInformation.phoneNumber.trim() === '') {
    this.confirmationService.openConfirmation({
      title: 'Phone Number Missing',
      message: 'No phone number present. Please add a phone number before sending messages.',
      confirmButtonText: 'OK',
      showCancelButton: false
    }).subscribe();
    return;
  }

  const dto = {
    patientId: this.patientId ?? null,
    leadId: this.leadId ?? null,
    to: this.conversationInformation.phoneNumber,
    message: {
      conversationId: this.conversationInformation.conversationId,
      direction: 'Outbound',
      isRead: true,
      isSent: true,
      timestamp: new Date().toISOString(),
      userId: this.conversationInformation.currentCounselorId || undefined,
    },
    messageContent: {
      contentType: 'text',
      content: text,
    },
  };

  this.emailAndSmsService.sendMessage(dto).subscribe({
    next: (res) => {
      if (!res.id || res.id === '00000000-0000-0000-0000-000000000000') {
        this.notificationService.showSnackBar(res.message || 'Failed to send message', 'failure');
        return;
      }
      this.notificationService.showSnackBar(res.message, 'success');
      this.messages.update(msgs => [
        ...msgs,
        {
          sender: 'clinic',
          text,
          time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
          status: 'Delivered',
          counselorInitials: this.getInitials(this.conversationInformation?.currentCounselorName)
        },
      ]);
      this.draft = '';
      this.scrollToBottom();
      this.cdr.detectChanges();
    },
    error: (err) => {
      this.notificationService.showSnackBar('Unexpected error: ' + err.message, 'failure');
      console.error('Failed to send message', err);
    },
  });
}


  private scrollToBottom(): void {
    setTimeout(() => {
      this.thread.nativeElement.scrollTo({
        top: this.thread.nativeElement.scrollHeight,
        behavior: 'smooth',
      });
    });
  }

  ngAfterViewInit(): void {
    this.scrollToBottom();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.smsSignalRService.stopConnection();
  }

  togglePatientActiveStatus(status: boolean): void {
    console.log('Patient active status toggled:', status);
  }
  onSaveAndClose(): void {
    console.log('Save and Close clicked');
  }
  onClickAddPatient(): void {
    this.router.navigate(['/patient/add']);
  }

  toggleLeadActiveStatus(event: Event): void {
    const status = (event.target as HTMLInputElement).checked;
    console.log('Lead active status toggled:', status);
  }
  onClickAddLead(): void {
    this.router.navigate(['/lead-management/add']);
  }
  onSubmit(): void {
    console.log('Submit clicked');
  }
  onClose(): void {
    if (this.patientId) {
      this.router.navigate(['/patients/view']);
    } else if (this.leadId) {
      this.router.navigate(['/lead-management/view']);
    } else {
      this.router.navigate(['/']);
    }
  }
}
