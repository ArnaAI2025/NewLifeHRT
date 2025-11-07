export class CreateReminderRequestDto {
  reminderDateTime!: string;
  reminderTypeId!: number;
  description?: string;
  isRecurring!: boolean;
  recurrenceRuleId?: number;
  recurrenceEndDateTime!: string | null;
  leadId?: string;
  patientId?: string;
}
