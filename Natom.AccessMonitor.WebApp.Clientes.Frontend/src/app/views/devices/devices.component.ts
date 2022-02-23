import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { DataTableDirective } from "angular-datatables";
import { NotifierService } from "angular-notifier";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { GoalDTO } from "src/app/classes/dto/goal.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { DataTablesResponse } from '../../classes/data-tables-response';
import { DeviceDTO } from "../../classes/dto/syncs/device.dto";
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-devices',
  styleUrls: ['./devices.component.css'],
  templateUrl: './devices.component.html'
})
export class DevicesComponent implements OnInit {
  @ViewChild('asignarModal', { static: false }) asignarModal : TemplateRef<any>; // Note: TemplateRef
  asignar_modal: NgbModalRef;
  asignar_encrypted_id: string;
  asignar_goal_encrypted_id: string;

  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  
  Devices: DeviceDTO[];
  Goals: GoalDTO[];
  Noty: any;

  constructor(private modalService: NgbModal,
              private apiService: ApiService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    
  }

  onEditClick(id: string) {
    this.routerService.navigate(['/devices/edit/' + id]);
  }

  onConfigSyncClick(encrypted_instance_id: string) {
    this.routerService.navigate(['/devices/sync/config/' + encodeURIComponent(encrypted_instance_id) ]);
  }

  onChangeGoalClick(encrypted_id: string) {
    this.asignar_encrypted_id = encrypted_id;
    this.asignar_goal_encrypted_id = "";
    this.asignar_modal = this.modalService.open(this.asignarModal);

    this.apiService.DoPOST<ApiResult<Array<GoalDTO>>>("goals/list_actives", {}, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {                          
                          this.Goals = response.data;
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
  }

  onAsignarConfirmadoClick() {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let encryptedId = this.asignar_encrypted_id;
    let goalId = this.asignar_goal_encrypted_id;
    let modalInstance = this.asignar_modal;

    if (goalId === undefined || goalId.length === 0) {
      this.confirmDialogService.showError("Debes seleccionar una Portería / Acceso al cual asignar el dispositivo.");
      return;
    }

    this.confirmDialogService.showConfirm("Desea asignar el dispositivo?", function () {  
      apiService.DoPOST<ApiResult<any>>("syncs/assign_device?encryptedId=" + encodeURIComponent(encryptedId) + "&goalId=" + encodeURIComponent(goalId), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                modalInstance.close();
                                                notifier.notify('success', 'Dispositivo asignado correctamente.');
                                                dataTableInstance.then((dtInstance: DataTables.Api) => {
                                                  dtInstance.ajax.reload()
                                                });
                                              }
                                            },
                                            (errorMessage) => {
                                              confirmDialogService.showError(errorMessage);
                                            });
                                          });
  }

  onDeleteClick(id: string) {
    console.log(id);
    let notifier = this.notifierService;
    this.confirmDialogService.showConfirm("Desea eliminar el dispositivo?", function () {  
      notifier.notify('success', 'Dispositivo eliminado con éxito.');
    });
  }

  ngOnInit(): void {

    this.dtIndex = {
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<DeviceDTO>>>("syncs/devices/list", dataTablesParameters, /*headers*/ null,
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
        { data: 'nombre', orderable: false },
        { data: 'device_id', orderable: false },
        { data: "device_ip", orderable: false },
        { data: 'location', orderable: false },
        { data: 'sync_name', orderable: false },
        { data: 'next_sync', orderable: false },
        { data: 'status', orderable: false },
        { data: '', orderable: false }
      ]
    };

    (<any>$('[data-toggle="tooltip"]')).tooltip();
  }

}
