export enum PermissionAction {
  Create = 'Create',
  Read = 'Read',
  Update = 'Update',
  Delete = 'Delete',
  Assign = 'Assign',
  NotApplicable = 'NotApplicable'
}

export enum PermissionResource {
  Patient = 'Patient',
  Lead = 'Lead',
  Doctor = 'Doctor',
  MedicalRecommendation = 'MedicalRecommendation',
  Proposal = 'Proposal',
  Order = 'Order',
  Product = 'Product',
  Pharmacy = 'Pharmacy',
  ProductPharmacyPrice = 'ProductPharmacyPrice',
  CommissionRatePerProduct = 'CommissionRatePerProduct',
  User = 'User',
  ShippingAddress = 'ShippingAddress',
  SMSChat = 'SMSChat',
  BulkEmailSMS = 'BulkEmailSMS',
  CommissionPayment = 'CommissionPayment',
  AppointmentBooking = 'AppointmentBooking',
  AppointmentCalendar = 'AppointmentCalendar',
  AppointmentTime = 'AppointmentTime',
  Task = 'Task',
  PharmacyConfiguration = 'PharmacyConfiguration'
}
