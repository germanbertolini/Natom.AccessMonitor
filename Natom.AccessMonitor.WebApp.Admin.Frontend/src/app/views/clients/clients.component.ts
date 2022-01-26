import { HttpClient } from "@angular/common/http";
import { Component, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { Client } from "src/app/classes/dto/client.dto";
import { DataTablesResponse } from '../../classes/data-tables-response';
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";
import { NewSyncComponent } from "./modals/new-sync.component";

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html'
})
export class ClientsComponent implements OnInit {
  @ViewChild(NewSyncComponent, { static: false })
  newSyncComponent: NewSyncComponent;

  dtClients: DataTables.Settings = {};
  Clients: Client[];

  constructor(private httpClientService: HttpClient,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
  }

  onNewSyncClick(id: string) {
    console.log("new click");
    this.newSyncComponent.Show(id);
  }

  onEditClick(id: string) {
    this.routerService.navigate(['/clients/edit/' + id]);
  }

  onDeleteClick(id: string) {
    console.log(id);
    let notifier = this.notifierService;
    this.confirmDialogService.showConfirm("Desea dar de baja al cliente?", function () {  
      notifier.notify('success', 'Cliente dado de baja con éxito.');
    });
  }

  ngOnInit(): void {

    this.dtClients = {
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
        this.Clients = [
          {
            encrypted_id: "231987213987987",
            razon_social: "Roque Perez S.A.",
            nombre_fantasia: "",
            cuit: "11-11111111-3",
            registered_at: new Date('2019-02-26T00:00:00'),
            email: "",
            phone: "",
            address: "",
            city: "",
            state: "",
            users: [],
            syncs: [],
            country: {
              encrypted_id: "132987132987",
              name: "Argentina",
              icon: "arg.png"
            }
          },
          {
            encrypted_id: "231987987023879",
            razon_social: "Techint S.A.",
            nombre_fantasia: "",
            cuit: "11-11111111-4",
            registered_at: new Date('2019-02-26T00:00:00'),
            email: "",
            phone: "",
            address: "",
            city: "",
            state: "",
            users: [],
            syncs: [],
            country: {
              encrypted_id: "132987132987",
              name: "Argentina",
              icon: "arg.png"
            }
          }
        ];
        callback({
          recordsTotal: this.Clients.length,
          recordsFiltered: this.Clients.length,
          data: [] //Siempre vacío para delegarle el render a Angular
        });
        if (this.Clients.length > 0) {
          $('.dataTables_empty').hide();
        }
        else {
          $('.dataTables_empty').show();
        }
        setTimeout(function() {
          (<any>$("tbody tr").find('[data-toggle="tooltip"]')).tooltip();
        }, 300);
      },
      columns: [
        { data: 'cuit' },
        { data: 'razon_social' },
        { data: "registered_at" },
        { data: 'country.name' }
      ]
    };
  }

}
