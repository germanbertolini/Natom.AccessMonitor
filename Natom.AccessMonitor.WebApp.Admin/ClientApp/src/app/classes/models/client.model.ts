import { Country } from "./country.model";
import { User } from "./user.model";

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
    public country: Country;
    public users: Array<User>;
    public registered_at: Date;
}