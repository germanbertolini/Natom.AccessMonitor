import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { UserDTO } from "src/app/classes/dto/user.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-user-crud',
  styleUrls: ['./user-crud.component.css'],
  templateUrl: './user-crud.component.html'
})

export class UserCrudComponent implements OnInit {

  crud: CRUDView<UserDTO>;

  constructor(private httpClientService: HttpClient,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<UserDTO>(routeService);
    this.crud.model = new UserDTO();
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    this.notifierService.notify('success', 'Usuario guardado correctamente.');
    this.routerService.navigate(['/users']);
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#user-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
