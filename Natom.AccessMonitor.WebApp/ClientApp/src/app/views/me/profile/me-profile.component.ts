import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { User } from "src/app/classes/models/user.model";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-me-profile',
  styleUrls: ['./me-profile.component.css'],
  templateUrl: './me-profile.component.html'
})

export class MeProfileComponent implements OnInit {
  private readonly httpClient: HttpClient;
  private readonly router: Router;
  private readonly notifier: NotifierService;
  private readonly confirmDialog: ConfirmDialogService;

  crud: CRUDView<User>;
  dtDevices: DataTables.Settings = {};

  constructor(httpClientService: HttpClient, routerService: Router, routeService: ActivatedRoute, notifierService: NotifierService, confirmDialogService: ConfirmDialogService) {
    this.httpClient = httpClientService;
    this.router = routerService;
    this.notifier = notifierService;
    this.confirmDialog = confirmDialogService;
    this.crud = new CRUDView<User>(routeService);
    this.crud.model = new User();

    //MOCK
    this.crud.model.encrypted_id = "adssdadas123e213132";
    this.crud.model.first_name = "German";
    this.crud.model.last_name = "Bertolini";
    this.crud.model.picture_url = "https://lh3.googleusercontent.com/ogw/ADea4I77Za6iqEqbdUL2uqgk2F88wtfI43U8O3gxDBdbRg=s128-c-mo";
    this.crud.model.email = "german.bertolini@gmail.com";
    this.crud.model.registered_at = new Date('2020-12-28T00:00:00');
    this.crud.model.business_name = "Natom";
    this.crud.model.business_role_name = "Administrador";
    this.crud.model.country_icon = "arg";
  }

  onCancelClick() {
      window.history.back();
  }

  onChangePasswordClick() {
    let notifier = this.notifier;
    this.confirmDialog.showConfirm("¿Realmente desea cambiar su clave?", function() {
      notifier.notify('info', 'Se ha enviado un mail a su casilla de correo.');
    });
  }

  ngOnInit(): void {
    setTimeout(function() {
      (<any>$("#device-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
  }

}