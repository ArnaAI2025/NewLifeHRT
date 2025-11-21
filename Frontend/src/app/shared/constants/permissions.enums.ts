export enum PermissionAction {
  Create = "Create",
  Update = "Update",
  Read = "Read",
  ActivateDeactivate = "ActivateDeactivate",
  Delete = "Delete",
  AcceptReject = "AcceptReject",
  Cancel = "Cancel",
  Clone = "Clone",
  Retry = "Retry",
  ApproveReject = "ApproveReject",
  MarkSelectedAsSeen = "MarkSelectedAsSeen",
  MarkAsComplete = "MarkAsComplete"
}

export enum PermissionResource {
  Dashboard = "Dashboard",
  User = "User",
  Patient = "Patient",
  Lead = "Lead",
  Appointment = "Appointment",
  Product = "Product",
  Pharmacy = "Pharmacy",
  PriceListItem = "PriceListItem",
  CommissionRate = "CommissionRate",
  Proposal = "Proposal",
  Order = "Order",
  OrderProductRefill = "OrderProductRefill",
  BulkSMS = "BulkSMS",
  PoolDetailsCounselorComission = "PoolDetailsCounselorComission",
  OrderProductScheduleCalendar = "OrderProductScheduleCalendar",
  PatientProposal = "PatientProposal",
  Coupon = "Coupon",
  Holiday = "Holiday",
  PharmacyConfiguration = "PharmacyConfiguration",
  LifeFileDashboard = "LifeFileDashboard",
  BulkSmsApproval = "BulkSmsApproval",
  UnseenSms = "UnseenSms",
  ReminderDashboard = "ReminderDashboard"
}

// Sub resources enums can be added here as needed
export enum PatientSubResource {
  CounselorNote = "CounselorNote",
  MedicalRecommendation = "MedicalRecommendation",
  Appointment = "Appointment",
  Reminder = "Reminder",
  Proposal = "Proposal",
  Order = "Order",
  SMS = "SMS"
}

export enum LeadSubResource {
  SMS = "SMS",
  Reminder = "Reminder"
}

export enum ProductSubResource {
  Strength = "Strength",
  PriceListItem = "PriceListItem",
  CommissionRate = "CommissionRate"
}

export enum PharmacySubResource {
  PriceListItem = "PriceListItem",
}

export enum ModuleType {
  PatientReminder = "PatientReminder",
  LeadReminder = "LeadReminder"
}

export enum ReminderDashboardSubResource {
  PatientReminder = "PatientReminder",
  LeadReminder = "LeadReminder"
}

