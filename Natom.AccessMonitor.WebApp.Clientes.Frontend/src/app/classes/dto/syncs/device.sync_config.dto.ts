export class DeviceSyncConfigDTO {
    public encrypted_instance_id: string;
    public interval_mins_to_server: number;
    public interval_mins_from_device: number;
    public last_sync: Date;
    public next_sync: Date;
}