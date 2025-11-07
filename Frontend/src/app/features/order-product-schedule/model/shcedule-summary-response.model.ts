import { ScheduleSummaryDto } from "./schedule-summary-schedules-response.model";

export class ScheduleSummaryResponse {
  isPatient!: boolean;
  message!: string;
  schedules!: ScheduleSummaryDto[];
}
