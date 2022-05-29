import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables";
import { NotifierService } from "angular-notifier";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { DocketListDTO } from "src/app/classes/dto/dockets/docket-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTablesResponse } from '../../classes/data-tables-response';
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-dockets',
  templateUrl: './dockets.component.html'
})
export class DocketsComponent implements OnInit {  
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Dockets: DocketListDTO[];
  Noty: any;
  filterStatusValue: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService,
              private authService: AuthService) {
    this.filterStatusValue = "ACTIVOS";
  }

  onFiltroEstadoChange(newValue: string) {
    this.filterStatusValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onReportWorkedHoursClick(id: string) {
    this.routerService.navigate(['/reports/worked-hours/by_docket/' + id]);
  }

  onEditClick(id: string) {
    this.routerService.navigate(['/dockets/edit/' + id]);
  }

  onEnableClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea dar de alta al legajo?", function () {  
      apiService.DoPOST<ApiResult<any>>("dockets/enable?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Legajo dado de alta con éxito.');
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

  onDisableClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea dar de baja al legajo?", function () {  
      apiService.DoDELETE<ApiResult<any>>("dockets/disable?encryptedId=" + encodeURIComponent(id), /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Legajo dado de baja con éxito.');
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<DocketListDTO>>>("dockets/list?status=" + this.filterStatusValue, dataTablesParameters, /*headers*/ null,
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
                          this.Dockets = response.data.records;
                          if (this.Dockets.length > 0) {
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
        { data: 'docket_number' },
        { data: 'employee_first_name' },
        { data: "employee_last_name" },
        { data: 'employee_dni' },
        { data: 'employee_title' },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
