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

@Component({
  selector: 'app-estadistica-asistencia',
  styleUrls: ['./estadistica-asistencia.component.css'],
  templateUrl: './estadistica-asistencia.component.html'
})

export class ReportesEstadisticaAsistenciaComponent implements OnInit {
  filtroFechaDesdeValue: string;
  filtroFechaHastaValue: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filtroFechaDesdeValue = "";
    this.filtroFechaHastaValue = "";
  }
  ngOnInit(): void {
    
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

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
    
    this.apiService.OpenNewTab("reportes/listados/estadistica-asistencia?desde=" + encodeURIComponent(this.filtroFechaDesdeValue) + "&hasta=" + encodeURIComponent(this.filtroFechaHastaValue));
  }
}
