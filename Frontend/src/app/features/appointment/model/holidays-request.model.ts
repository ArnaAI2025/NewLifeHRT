export class GetAllHolidaysRequestDto {
  startDate!: string;    
  endDate!: string;
  doctorIds?: string[]; 
}