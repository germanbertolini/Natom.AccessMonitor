import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { GoalDTO } from "src/app/classes/dto/goal.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { UserDTO } from "src/app/classes/dto/user.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-me-profile',
  styleUrls: ['./me-profile.component.css'],
  templateUrl: './me-profile.component.html'
})

export class MeProfileComponent implements OnInit {
  crud: CRUDView<UserDTO>;
  dtDevices: DataTables.Settings = {};

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<UserDTO>(routeService);
    this.crud.model = authService.getCurrentUser();
  }

  onCancelClick() {
      window.history.back();
  }

  onChangePasswordClick() {
    let notifier = this.notifierService;
    let apiService = this.apiService;
    let authService = this.authService;

    this.confirmDialogService.showConfirm("¿Realmente desea cambiar su clave? Esto le inhabilitará su cuenta hasta que confirme el mail que se le enviará a su correo electrónico.", function() {
      apiService.DoPOST<ApiResult<Array<GoalDTO>>>("users/recover_me", {}, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {                          
                          notifier.notify('info', 'Se ha enviado un mail a su casilla de correo.');
                          setTimeout(function() { authService.logout(); }, 2500);
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
    });
  }

  ngOnInit(): void {
    setTimeout(function() {
      (<any>$("#device-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
  }

}
