import { HttpHeaders } from "@angular/common/http";
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DeviceDTO } from "src/app/classes/dto/device.dto";

@Component({
  selector: 'app-devices-syncs-clientes',
  templateUrl: './devices-syncs-clientes.component.html'
})
export class DevicesSyncsClientesComponent implements OnInit {

  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtDevices: DataTables.Settings = {};
  Devices: DeviceDTO[];
  Noty: any;
  clienteId: string;
  syncId: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.clienteId = decodeURIComponent(this.routeService.snapshot.paramMap.get('id_cliente'));
    this.syncId = decodeURIComponent(this.routeService.snapshot.paramMap.get('id'));
  }

  ngOnInit(): void {

    this.dtDevices = {
      pagingType: 'simple_numbers',
      pageLength: 10,
      serverSide: true,
      processing: true,
      info: true,
      language: {
        emptyTable: '',
        zeroRecords: 'No hay registros',
        lengthMenu: 'Mostrar _MENU_ registros',
        search: 'Buscar:',
        info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
        infoEmpty: 'De 0 a 0 de 0 registros',
        infoFiltered: '(filtrados de _MAX_ registros totales)',
        paginate: {
          first: 'Primero',
          last: 'Último',
          next: 'Siguiente',
          previous: 'Anterior'
        },
      },
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.DoPOST<ApiResult<DataTableDTO<DeviceDTO>>>("clientes/syncs/devices/list?encryptedId=" + encodeURIComponent(this.syncId), dataTablesParameters, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {
                          callback({
                            recordsTotal: response.data.recordsTotal,
                            recordsFiltered: response.data.recordsFiltered,
                            data: [] //Siempre vacío para delegarle el render a Angular
                          });
                          this.Devices = response.data.records;
                          if (this.Devices.length > 0) {
                            $('.dataTables_empty').hide();
                          }
                          else {
                            $('.dataTables_empty').show();
                          }
                          setTimeout(function() {
                            (<any>$("tbody tr").find('[data-toggle="tooltip"]')).tooltip();
                          }, 300);
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
      },
      columns: [
        { data: 'device_name' },
        { data: 'device_last_configuration_at' },
        { data: 'device_id' },
        { data: 'marca' },
        { data: 'modelo' },
        { data: 'firmware_version' }
      ]
    };
  }

}
