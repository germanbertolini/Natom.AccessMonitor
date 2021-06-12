import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { User } from "src/app/classes/models/user.model";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-user-crud',
  styleUrls: ['./user-crud.component.css'],
  templateUrl: './user-crud.component.html'
})

export class UserCrudComponent implements OnInit {
  private readonly httpClient: HttpClient;
  private readonly router: Router;
  private readonly notifier: NotifierService;
  private readonly confirmDialog: ConfirmDialogService;

  crud: CRUDView<User>;

  constructor(httpClientService: HttpClient, routerService: Router, routeService: ActivatedRoute, notifierService: NotifierService, confirmDialogService: ConfirmDialogService) {
    this.httpClient = httpClientService;
    this.router = routerService;
    this.notifier = notifierService;
    this.confirmDialog = confirmDialogService;
    this.crud = new CRUDView<User>(routeService);
    this.crud.model = new User();
  }

  onCancelClick() {
    this.confirmDialog.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    this.notifier.notify('success', 'Usuario guardado correctamente.');
    this.router.navigate(['/users']);
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#user-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
