import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { OrganizationDTO } from "src/app/classes/dto/background/organization.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { BackgroundService } from "src/app/services/background.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-me-organization',
  styleUrls: ['./me-organization.component.css'],
  templateUrl: './me-organization.component.html'
})

export class MeOrganizationComponent implements OnInit {

  model: OrganizationDTO;

  constructor(private backgroundService: BackgroundService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {

    this.model = backgroundService.resume.organization;
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    this.notifierService.notify('success', 'Configuración guardada correctamente.');
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#docket-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
