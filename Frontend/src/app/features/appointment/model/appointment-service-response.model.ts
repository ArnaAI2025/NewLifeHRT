import { UserDto } from "../../user-management/model/user-dto.model";

export class AppointmentServiceResponse{
    id!:string;
    serviceName!:string;
    displayName!:string;
    maxDuration?:string;
    users:UserDto[] = [];
}