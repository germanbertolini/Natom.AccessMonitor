import { CountryDTO } from "./country.dto";
import { SyncronizerDTO } from "./syncronizer.dto";
import { UserDTO } from "./user.dto";

export class Client {
    public encrypted_id: string;
    public razon_social: string;
    public nombre_fantasia: string;
    public cuit: string;
    public email: string;
    public phone: string;
    public address: string;
    public city: string;
    public state: string;
    public country: CountryDTO;
    public users: Array<UserDTO>;
    public syncs: Array<SyncronizerDTO>;
    public registered_at: Date;
}