import { UserRole } from "../enums/user-role.enum";
import { DropDownResponseDto } from "./drop-down-response.model";

export interface GetRolesForCreateUserResponseDto extends DropDownResponseDto {
  roleEnum: UserRole;
}
