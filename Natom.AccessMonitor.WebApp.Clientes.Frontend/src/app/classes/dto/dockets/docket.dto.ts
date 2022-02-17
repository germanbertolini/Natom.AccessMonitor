import { DocketRangeDTO } from "./docket-range.dto";

export class DocketDTO {
    public encrypted_id: string;
    public docket_number: string;
    public first_name: string;
    public last_name: string;
    public dni: string;
    public title_encrypted_id: string;
    public apply_ranges: boolean;
    public ranges: DocketRangeDTO[];
    public hour_value: number;
    public extra_hour_value: number;
}