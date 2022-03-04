export class SyncTimeDTO {
    public instance_id: string;
    public installer_name: string;
    public last_sync_at: Date;
    public next_sync_at: Date;
    public each_miliseconds: number;
    public next_on_miliseconds: number;
}