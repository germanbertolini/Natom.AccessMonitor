import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { Docket } from "src/app/classes/models/docket.model";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-docket-crud',
  styleUrls: ['./docket-crud.component.css'],
  templateUrl: './docket-crud.component.html'
})

export class DocketCrudComponent implements OnInit {
  private readonly httpClient: HttpClient;
  private readonly router: Router;
  private readonly notifier: NotifierService;
  private readonly confirmDialog: ConfirmDialogService;

  crud: CRUDView<Docket>;

  constructor(httpClientService: HttpClient, routerService: Router, routeService: ActivatedRoute, notifierService: NotifierService, confirmDialogService: ConfirmDialogService) {
    this.httpClient = httpClientService;
    this.router = routerService;
    this.notifier = notifierService;
    this.confirmDialog = confirmDialogService;
    this.crud = new CRUDView<Docket>(routeService);
    this.crud.model = new Docket();
  }

  onCancelClick() {
    this.confirmDialog.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    this.notifier.notify('success', 'Legajo guardado correctamente.');
    this.router.navigate(['/dockets']);
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#docket-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
