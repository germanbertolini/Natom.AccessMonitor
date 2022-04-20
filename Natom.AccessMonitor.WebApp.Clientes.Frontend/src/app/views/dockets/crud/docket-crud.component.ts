import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { DocketRangeDTO } from "src/app/classes/dto/dockets/docket-range.dto";
import { DocketDTO } from "src/app/classes/dto/dockets/docket.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { TitleDTO } from "src/app/classes/dto/title.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTablesResponse } from "../../../classes/data-tables-response";

@Component({
  selector: 'app-docket-crud',
  styleUrls: ['./docket-crud.component.css'],
  templateUrl: './docket-crud.component.html'
})

export class DocketCrudComponent implements OnInit {
  crud: CRUDView<DocketDTO>;
  presencePending: boolean;
  Titles: Array<TitleDTO>;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<DocketDTO>(routeService);
    this.crud.model = new DocketDTO();
    this.crud.model.title_encrypted_id = "";
    this.crud.model.ranges = new Array<DocketRangeDTO>();
    this.crud.model.apply_ranges = true;
    this.presencePending = this.crud.isNewMode;
    
    this.setRangeDefault();

  }

  setRangeDefault() {
    //DIAS DE LA SEMANA
    for (let i = 0; i < 7; i ++) {
      this.crud.model.ranges.push({
        encrypted_id: "",
        dayOfWeek: i == 6 ? 0 : i + 1,
        from: i < 5 ? "08:00" : null,
        to: i < 5 ? "17:00" : null
      });
      this.crud.model.ranges.push({
        encrypted_id: "",
        dayOfWeek: i == 6 ? 0 : i + 1,
        from: null,
        to: null
      });
    }
  }

  removePresencePending() {
    this.presencePending = false;
  }

  fixTimeFormat(time: string): string {
    if (time === undefined || time === null || (time !== null && time.length === 0))
      return null;

    let parts = time.split(':');
    let final = parts[0].substring(0, 2).padStart(2, '0');
    if (parts.length > 1) {
      parts[1] = parts[1].substring(0, 2).padStart(2, '0');
      final = final + ":" + parts[1];
    }
    else {
      final = final + ":00";
    }
    return final;
  }

  getTimeNumber(time:string): number {
    let parts = time.split(':');
    return parseInt(parts[0]) * 60 + parseInt(parts[1]);
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    for (let i = 0; i < this.crud.model.ranges.length; i ++) {
      this.crud.model.ranges[i].from = this.fixTimeFormat(this.crud.model.ranges[i].from);
      this.crud.model.ranges[i].to = this.fixTimeFormat(this.crud.model.ranges[i].to);
    }

    
    if (this.crud.model.docket_number === undefined || this.crud.model.docket_number.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un N° de legajo.");
      return;
    }

    if (this.crud.model.first_name === undefined || this.crud.model.first_name.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Nombre.");
      return;
    }

    if (this.crud.model.last_name === undefined || this.crud.model.last_name.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Apellido.");
      return;
    }

    if (!(/^[0-9]{8}$/.test(this.crud.model.dni)))
    {
      this.confirmDialogService.showError("Debes ingresar un DNI válido sin puntos.");
      return;
    }

    if (this.crud.model.title_encrypted_id === undefined || this.crud.model.title_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar un Cargo");
      return;
    }

    if (this.presencePending) {
      this.presencePending = false;
      <any>$("#nav-profile-tab").trigger("click");
      return;
    }

    if (this.crud.model.apply_ranges) {
      for (let i = 0; i < this.crud.model.ranges.length; i ++) {
        this.crud.model.ranges[i].from = this.fixTimeFormat(this.crud.model.ranges[i].from);
        this.crud.model.ranges[i].to = this.fixTimeFormat(this.crud.model.ranges[i].to);

        if (this.crud.model.ranges[i].from !== null && this.crud.model.ranges[i].to === null)
        {
          this.confirmDialogService.showError("Presencialidad: Hay un campo 'Desde' sin valor 'Hasta'");
          return;
        }

        if (this.crud.model.ranges[i].from === null && this.crud.model.ranges[i].to !== null)
        {
          this.confirmDialogService.showError("Presencialidad: Hay un campo 'Hasta' sin valor 'Desde'");
          return;
        }

        if (this.crud.model.ranges[i].from !== null && this.crud.model.ranges[i].to !== null) {
          if (this.getTimeNumber(this.crud.model.ranges[i].from) >= this.getTimeNumber(this.crud.model.ranges[i].to))
          {
            let diff1 = this.getTimeNumber("24:00") - this.getTimeNumber(this.crud.model.ranges[i].from);
            let diff2 = this.getTimeNumber(this.crud.model.ranges[i].to);
            if ((diff1 + diff2) / 60 > 9)
            {
              this.confirmDialogService.showError("Presencialidad: Campo 'Desde' debe ser inferior de 'Hasta'");
              return;
            }
          }
        }
      }
    }

    this.apiService.DoPOST<ApiResult<DocketDTO>>("dockets/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Legajo guardado correctamente.');
          this.routerService.navigate(['/dockets']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });

  }



  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("dockets/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
    (response) => {
      if (!response.success) {
        this.confirmDialogService.showError(response.message);
      }
      else {
        if (response.data.entity !== null)
          this.crud.model = response.data.entity;
        
        this.Titles = <Array<TitleDTO>>response.data.cargos;

        if (this.crud.model.ranges.length === 0)
          this.setRangeDefault();

        setTimeout(function() {
          (<any>$("#docket-crud").find('[data-toggle="tooltip"]')).tooltip();
        }, 300);
      }
    },
    (errorMessage) => {
      this.confirmDialogService.showError(errorMessage);
    });
    
  }

}
