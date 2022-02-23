import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { DeviceSyncConfigDTO } from "src/app/classes/dto/syncs/device.sync_config.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";

@Component({
  selector: 'app-devices-sync-config',
  styleUrls: ['./devices-sync-config.component.css'],
  templateUrl: './devices-sync-config.component.html'
})

export class DevicesSyncConfigComponent implements OnInit {
  crud: CRUDView<DeviceSyncConfigDTO>;
  dtDevices: DataTables.Settings = {};
  instance_id: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {

    this.instance_id = decodeURIComponent(this.routeService.snapshot.paramMap.get('instance_id'));

    this.crud = new CRUDView<DeviceSyncConfigDTO>(routeService);
    this.crud.model = new DeviceSyncConfigDTO();

    this.apiService.DoGET<ApiResult<DeviceSyncConfigDTO>>("syncs/get_config?instanceId=" + encodeURIComponent(this.instance_id), /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {
                          this.crud.model = response.data;
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.interval_mins_from_device === undefined || this.crud.model.interval_mins_from_device === 0)
    {
      this.confirmDialogService.showError("Debe ingresar un intervalo de lectura de relojes válido.");
      return;
    }

    if (this.crud.model.interval_mins_from_device < 1 || this.crud.model.interval_mins_from_device > 60)
    {
      this.confirmDialogService.showError("El intervalo de lectura de relojes debe comprender entre 1 y 60 minutos.");
      return;
    }

    if (this.crud.model.interval_mins_to_server === undefined || this.crud.model.interval_mins_to_server === 0)
    {
      this.confirmDialogService.showError("Debe ingresar un intervalo de sincronización válido.");
      return;
    }

    if (this.crud.model.interval_mins_to_server < 5 || this.crud.model.interval_mins_to_server > 60)
    {
      this.confirmDialogService.showError("El intervalo de sincronización debe comprender entre 5 y 60 minutos.");
      return;
    }

    this.apiService.DoPOST<ApiResult<DeviceSyncConfigDTO>>("syncs/set_config", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Configuración del sincronizador guardada correctamente.');
          this.routerService.navigate(['/devices']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {
    setTimeout(function() {
      (<any>$("#device-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
  }

}
