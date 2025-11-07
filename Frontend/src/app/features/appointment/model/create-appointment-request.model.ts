export class CreateAppointmentRequestDto {
  slotId!: string;          
  appointmentDate!: string; 
  patientId!: string;        
  doctorId!: string;
  modeId!: string;
  description?: string;    
}
