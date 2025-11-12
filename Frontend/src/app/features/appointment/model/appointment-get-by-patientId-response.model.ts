export class AppointmentGetByPatientIdResponseDto {
  appointmentId!: string;
  status!: string;
  serviceName!: string;
  doctorName!: string;
  counselorName!: string;
  doctorStartDateTime!: string;
  doctorEndDateTime!: string;
  description?: string;
  utcStartDateTime!: string;
  utcEndDateTime!: string;
  patientName!: string;
}
