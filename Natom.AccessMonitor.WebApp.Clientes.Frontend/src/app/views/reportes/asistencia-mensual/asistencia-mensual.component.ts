import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { fromEvent } from 'rxjs';
import { map, distinctUntilChanged, debounceTime, mergeMap } from 'rxjs/operators';
import { NotifierService } from "angular-notifier";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { DocketDTO } from "src/app/classes/dto/dockets/docket.dto";
import { AutocompleteResultDTO } from "src/app/classes/dto/shared/autocomplete-result.dto";

@Component({
  selector: 'app-asistencia-mensual',
  styleUrls: ['./asistencia-mensual.component.css'],
  templateUrl: './asistencia-mensual.component.html'
})

export class ReportesAsistenciaMensualComponent implements OnInit {
  filtroFechaDesdeValue: string;
  filtroFechaHastaValue: string;
  legajosSearch: DocketDTO[];
  legajoSearchTexto: string;
  legajoSelectedId: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filtroFechaDesdeValue = "";
    this.filtroFechaHastaValue = "";
    this.legajoSelectedId = "";
  }

  ngOnInit(): void {
    
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onLegajoSearchSelectItem (legajo: DocketDTO) {
    this.legajoSearchTexto = legajo.docket_number + " /// " + legajo.first_name + " " + legajo.last_name + " /// DNI: " + legajo.dni;
    this.legajoSelectedId = legajo.encrypted_id;
    this.closeLegajoSearchPopUp();
  }

  closeLegajoSearchPopUp() {
    setTimeout(() => { this.legajosSearch = undefined; }, 200);    
  }

  onLegajoSearchKeyUp(event) {
    let observable = fromEvent(event.target, 'keyup')
      .pipe (
        map(value => event.target.value),
        debounceTime(500),
        distinctUntilChanged(),
        mergeMap((search) => {
          return this.apiService.DoGETWithObservable("dockets/search?filter=" + encodeURIComponent(search), /* headers */ null);
        })
      )
    observable.subscribe((data) => {
      var result = <ApiResult<AutocompleteResultDTO<DocketDTO>>>data;
      if (!result.success) {
        this.confirmDialogService.showError("Se ha producido un error interno.");
      }
      else {
        this.legajosSearch = result.data.results;
      }
    });
  }

  onFechaDesdeChange(newValue) {
    this.filtroFechaDesdeValue = newValue.day + "/" + newValue.month + "/" + newValue.year;
  }

  onFechaHastaChange(newValue) {
    this.filtroFechaHastaValue = newValue.day + "/" + newValue.month + "/" + newValue.year;
  }

  onConsultar() {
    if (this.filtroFechaDesdeValue === undefined || this.filtroFechaDesdeValue.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar fecha 'Desde'.");
      return;
    }

    if (this.filtroFechaHastaValue === undefined || this.filtroFechaHastaValue.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar fecha 'Hasta'.");
      return;
    }

    if (this.legajoSelectedId === undefined || this.legajoSelectedId.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar el Legajo.");
      return;
    }
    
    this.apiService.OpenNewTab("reportes/listados/mensual-asistencia?desde=" + encodeURIComponent(this.filtroFechaDesdeValue) + "&hasta=" + encodeURIComponent(this.filtroFechaHastaValue) + "&encryptedId=" + encodeURIComponent(this.legajoSelectedId));
  }
}
