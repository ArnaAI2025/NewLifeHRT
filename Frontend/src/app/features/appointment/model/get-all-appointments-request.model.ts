export class GetAllAppointmentsRequestDto {
  startDate!: string;
  endDate!: string;
  doctorIds?: string[];
}
