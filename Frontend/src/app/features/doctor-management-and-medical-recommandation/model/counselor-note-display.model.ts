export interface CounselorNoteDisplay {
  id: string;
  patientId: string;
  subject: string;
  note: string;
  isAdminMailSent: boolean;
  isDoctorMailSent: boolean;
  isActive: boolean;
}
