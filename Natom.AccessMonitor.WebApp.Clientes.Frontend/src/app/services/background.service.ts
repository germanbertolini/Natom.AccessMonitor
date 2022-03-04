import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import * as moment from "moment";
import { CookieService } from "ngx-cookie-service";
import { Observable } from "rxjs";
import { OrganizationDTO } from "../classes/dto/background/organization.dto";
import { ResumeDTO } from "../classes/dto/background/resume.dto";
import { SyncTimeDTO } from "../classes/dto/background/sync-time.dto";
import { ApiResult } from "../classes/dto/shared/api-result.dto";
import { ConfirmDialogService } from "../components/confirm-dialog/confirm-dialog.service";
import { SpinnerLoadingService } from "../components/spinner-loading/spinner-loading.service";
import { ApiService } from "./api.service";
import { JsonAppConfigService } from "./json-app-config.service";

@Injectable({
    providedIn: 'root'
  })
export class BackgroundService {
    resume: ResumeDTO;
    last_sync_at: Date;
    next_sync_at: Date;

    constructor(private jsonAppConfig: JsonAppConfigService,
                private apiService: ApiService,
                private cookieService: CookieService,
                private confirmDialogService: ConfirmDialogService) {
        this.last_sync_at = null;
        this.next_sync_at = null;
        this.resume = new ResumeDTO();
        this.resume.unassigned_devices = new Array<string>();
        this.resume.places_without_hours = new Array<string>();
        this.resume.organization = new OrganizationDTO();
        this.resume.organization.picture_url = "/assets/img/buildings.png";
        this.resume.organization.country_icon = "arg";
        this.refreshStatusResume();
    }

    public refreshStatusResume() {
        let _confirmDialogService = this.confirmDialogService;
        let _resume = this.resume;

        this.apiService.DoGET<ApiResult<ResumeDTO>>("background/resume", /*headers*/ null,
                            (response) => {
                                if (!response.success) {
                                    _confirmDialogService.showError(response.message);
                                }
                                else {
                                    this.resume = response.data;
                                    this.initSyncTimers();
                                }
                            },
                            (errorMessage) => {
                            _confirmDialogService.showError(errorMessage);
                            });
    }

    public initSyncTimers() {
        let syncs = this.resume.syncs_times;

        if (syncs.length > 0) {
            this.last_sync_at = syncs[0].last_sync_at;
            
            for (let i = 0; i < syncs.length; i ++) {
                if (syncs[i].next_on_miliseconds > 0) {
                    if (i == 0)
                        this.next_sync_at = syncs[i].next_sync_at;

                    let syncData = syncs[i];
                    let myInstance = this;

                    if (this.next_sync_at > syncData.next_sync_at)
                        this.next_sync_at = syncData.next_sync_at;

                    setTimeout(() => {
                        let _syncData = syncData;
                        let _myInstance = myInstance;

                        let logica = () => {
                            _syncData.last_sync_at = _syncData.next_sync_at;
                            _syncData.next_sync_at = new Date(moment(_syncData.last_sync_at).add(_syncData.each_miliseconds, 'milliseconds').format());

                            _myInstance.last_sync_at = _syncData.last_sync_at;

                            if (_myInstance.next_sync_at <= _syncData.last_sync_at)
                                _myInstance.next_sync_at = _syncData.next_sync_at;
                            else if (_myInstance.next_sync_at > _syncData.next_sync_at)
                                _myInstance.next_sync_at = _syncData.next_sync_at;
                        }

                        logica();

                        setInterval(logica, syncData.each_miliseconds);

                    }, syncs[i].next_on_miliseconds);
                }
            }
        }
    }

}