import { OrganizationDTO } from "./organization.dto";

export class ResumeDTO {
    public current_year: number;
    public organization: OrganizationDTO;
    public unassigned_devices: Array<string>;
}