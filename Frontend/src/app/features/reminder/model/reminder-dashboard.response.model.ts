export class ReminderDashboard {
  reminderId!: string;
  name!: string;
  reminderDateTime!: string;
  reminderTypeName!: string;
  description?: string | null;
  isRecurring!: boolean;
  recurrenceEndDate?: string | null;
  patientId?: string | null;
  leadId?: string | null;
}
