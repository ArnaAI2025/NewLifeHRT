export interface CounselorNoteRequest {
  patientId: string | null;
  counselorId : number | undefined;
  subject: string;
  note: string;
  isAdminMailSent: boolean;
  isDoctorMailSent: boolean;
}
