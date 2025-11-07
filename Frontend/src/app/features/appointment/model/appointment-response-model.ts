export class AppointmentResponseDto {
  appointmentId!: string;
  slotId!: string;
  startDateTime!: string;
  endDateTime!: string;
  patientId!: string;
  patientName!: string;
  modeId!: number;
  modeName!: string;
  statusId!: number;
  statusName!: string;
  description?: string;
  serviceName!: string;
  doctorId!: number;
  doctorName!: string;
  counselorName!: string;
  title!: string;
  doctorStartDateTime!: string;
  doctorEndDateTime!: string;
  colorCode?: string;
}
