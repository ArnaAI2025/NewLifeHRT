export class AppointmentGetResponseDto {
  id!: string;                  
  slotId!: string;            
  appointmentDate!: string;     
  patientId!: string;           
  doctorId!: string;
  modeId!: string;
  statusId!: string;
  description?: string;
  serviceId!:string;       
}
