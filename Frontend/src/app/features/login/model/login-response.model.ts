import { VerifyOtpResponseDto } from "./verify-otp-response.model";

export class LoginResponseDto {
  constructor(
    public userId: number,
    public otpId: string,
    public expiresAt: string,
    public tokens:VerifyOtpResponseDto,
    public mustChangePassword : boolean
  ) {}
}
