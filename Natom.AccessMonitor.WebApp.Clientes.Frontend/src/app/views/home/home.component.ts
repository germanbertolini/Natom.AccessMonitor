import { DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { PanoramaActualDTO } from 'src/app/classes/dto/panorama-actual.dto';
import { PanoramaPorcentajesDTO } from 'src/app/classes/dto/panorama-porcentajes.dto';
import { ApiResult } from 'src/app/classes/dto/shared/api-result.dto';
import { ConfirmDialogService } from 'src/app/components/confirm-dialog/confirm-dialog.service';
import { ApiService } from 'src/app/services/api.service';
import { BackgroundService } from 'src/app/services/background.service';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html',
  providers: [DatePipe]
})
export class HomeComponent {
  panoramaActual: PanoramaActualDTO;
  panoramaPorcentajes: PanoramaPorcentajesDTO;
  filterPlaceId: string;
  myDate: Date;

  constructor(private apiService: ApiService,
              private datePipe: DatePipe,
              private confirmDialogService: ConfirmDialogService,
              private routerService: Router,
              private backgroundService: BackgroundService) {
    this.filterPlaceId = "";
    this.panoramaActual = new PanoramaActualDTO();
    this.panoramaPorcentajes = new PanoramaPorcentajesDTO();

    this.getPanoramaActual();
    this.getPanoramaPorcentajes();

    setInterval(() => this.getPanoramaActual(), 60000);
  }

  onFilterPlaceChange() {
    this.getPanoramaActual();
    this.getPanoramaPorcentajes();
  }

  getPanoramaActual() {
    this.apiService.DoHiddenGET<ApiResult<PanoramaActualDTO>>("horarios/panorama/actual?encryptedPlaceId=" + encodeURIComponent(this.filterPlaceId), /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {
                          this.panoramaActual = response.data;
                          this.myDate = new Date();
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
  }

  getPanoramaPorcentajes() {
    this.apiService.DoHiddenGET<ApiResult<PanoramaPorcentajesDTO>>("horarios/panorama/porcentajes?encryptedPlaceId=" + encodeURIComponent(this.filterPlaceId), /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {
                          this.panoramaPorcentajes = response.data;
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
  }
  
}
