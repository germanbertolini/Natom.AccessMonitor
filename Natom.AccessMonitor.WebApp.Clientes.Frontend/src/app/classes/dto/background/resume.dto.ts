import { OrganizationDTO } from "./organization.dto";

export class ResumeDTO {
    public current_year: number;
    public organization: OrganizationDTO;
    public unassigned_devices: Array<string>;
    public places_without_hours: Array<string>;
}