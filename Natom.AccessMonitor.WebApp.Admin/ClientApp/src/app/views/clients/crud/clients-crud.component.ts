import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { Client } from "src/app/classes/models/client.model";
import { User } from "src/app/classes/models/user.model";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-clients-crud',
  styleUrls: ['./clients-crud.component.css'],
  templateUrl: './clients-crud.component.html'
})

export class ClientsCrudComponent implements OnInit {
  crud: CRUDView<Client>;
  crudUser: User;
  dtUsers: DataTables.Settings = {};

  constructor(private httpClientService: HttpClient,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<Client>(routeService);
    this.crudUser = new User();
    this.crud.model = new Client();
    this.crud.model.users = new Array<User>();
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    this.notifierService.notify('success', 'Cliente guardado correctamente.');
    this.routerService.navigate(['/clients']);
  }

  onDeleteUserClick(encrypted_id: string) {
    console.log(encrypted_id);
    this.confirmDialogService.showConfirm("¿Está seguro de eliminar el Usuario?", () => {
      console.log(encrypted_id);
      this.crud.model.users = this.crud.model.users.filter(i => i.encrypted_id !== encrypted_id);
      console.log (this.crud.model.users);
    });
  }

  onAddUserClick() {
    if (this.crudUser.first_name === undefined)     this.confirmDialogService.showError("Debe ingresar el NOMBRE.");
    else if (this.crudUser.last_name === undefined) this.confirmDialogService.showError("Debe ingresar el APELLIDO.");
    else if (this.crudUser.email === undefined)     this.confirmDialogService.showError("Debe ingresar el EMAIL.");
    else {
      this.crudUser.encrypted_id = "NEW_" + new Date().getTime().toString();
      this.crudUser.state = "PENDIENTE DE CONFIRMACIÓN";
      this.crud.model.users.push(this.crudUser);

      this.crudUser = new User(); //LIMPIAMOS CAMPOS
      this.notifierService.notify('info', 'Usuario agregado.');
    }
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#docket-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
    this.dtUsers = {
      pagingType: 'simple_numbers',
      pageLength: 10,
      serverSide: false,
      processing: true,
      search: false,
      paging: false,
      info: false,
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
        callback({
          recordsTotal: this.crud.model.users.length,
          recordsFiltered: this.crud.model.users.length,
          data: [] //vacío para que del Rendering se encargue Angular
        });
        
        $('.dataTables_empty').hide();
      },
      columns: [
        { data: 'first_name' },
        { data: 'email' },
        { data: 'state' },
        { data: 'encrypted_id' }
      ]
    };
    
  }

}
