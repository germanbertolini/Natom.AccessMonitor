import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CookieService } from "ngx-cookie-service";
import { Observable } from "rxjs";
import { OrganizationDTO } from "../classes/dto/background/organization.dto";
import { ResumeDTO } from "../classes/dto/background/resume.dto";
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

    constructor(private jsonAppConfig: JsonAppConfigService,
                private apiService: ApiService,
                private cookieService: CookieService,
                private confirmDialogService: ConfirmDialogService) {
        this.resume = new ResumeDTO();
        this.resume.unassigned_devices = new Array<string>();
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
                                }
                            },
                            (errorMessage) => {
                            _confirmDialogService.showError(errorMessage);
                            });
    }

}