import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Device } from "src/app/classes/models/device.model";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { DataTablesResponse } from '../../classes/data-tables-response';

@Component({
  selector: 'app-device-crud',
  templateUrl: './device-crud.component.html'
})

export class DeviceCrudComponent implements OnInit {
  crud: CRUDView;//<Device>;

  dtDevices: DataTables.Settings = {};
  httpClient: HttpClient;
  Devices: object[];

  constructor(httpClient: HttpClient, route: ActivatedRoute) {
    this.httpClient = httpClient;
    this.crud = new CRUDView(route);
  }

  ngOnInit(): void {

    this.dtDevices = {
      pagingType: 'full_numbers',
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
          first: 'Prim.',
          last: 'Últ.',
          next: 'Sig.',
          previous: 'Ant.'
        },
      },
      ajax: (dataTablesParameters: any, callback) => {
        //this.httpClient
        //  .post<DataTablesResponse>(
        //    this.connectService.URL + 'read_records_dt.php',
        //    dataTablesParameters, {}
        //  ).subscribe(resp => {
        //    this.Members = resp.data;
        //    this.NumberOfMembers = resp.data.length;
        //    $('.dataTables_length>label>select, .dataTables_filter>label>input').addClass('form-control-sm');
        //    callback({
        //      recordsTotal: resp.recordsTotal,
        //      recordsFiltered: resp.recordsFiltered,
        //      data: []
        //    });
        //    if (this.NumberOfMembers > 0) {
        //      $('.dataTables_empty').css('display', 'none');
        //    }
        //  }
        //  );
        this.Devices = [
          { nombre: "Portería 1 - Lector 1", dispositivo_id: "21987", estado: "Activo", id: 1 },
          { nombre: "Portería 1 - Lector 2", dispositivo_id: "21988", estado: "Desconectado", id: 2 },
          { nombre: "Portería 2 - Lector 1", dispositivo_id: "21989", estado: "Activo", id: 3 }
        ];
        callback({
          recordsTotal: this.Devices.length,
          recordsFiltered: this.Devices.length,
          data: this.Devices
        });
      },
      columns: [
        {
          data: 'nombre'
        },
        {
          data: 'dispositivo_id'
        },
        {
          data: 'estado'
        },
        {
          data: "id",
          render: function (data, type, row, meta) {
            var html = "";
            html += '&nbsp;<a class="btn btn-primary btn-sm edit-smt-btn" id="' + data + '" data-toggle="tooltip" data-placement="top" title="Editar"><i class="fa fa-edit" aria-hidden="true"></i></a>';
            html += '&nbsp;<a class="btn btn-danger btn-sm delete-smt-btn" id="' + data + '" data-toggle="tooltip" data-placement="top" title="Eliminar"><i class="fa fa-times" aria-hidden="true"></i></a>';
            return html;
          }
        }
      ],
      rowCallback: function (row, data, index) {
        (<any>$(row).find('[data-toggle="tooltip"]')).tooltip();
      },
      drawCallback: function () {
        $(this).on("click", function () {
          if ($(this).is(".collapsed")) {
            (<any>$(this).find('tbody tr.child [data-toggle="tooltip"]')).tooltip();
          }
        });
      }
    };
  }

}