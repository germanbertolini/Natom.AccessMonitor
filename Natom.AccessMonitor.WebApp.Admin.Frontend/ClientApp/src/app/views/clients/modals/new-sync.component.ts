import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { Syncronizer } from "src/app/classes/models/syncronizer.model";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";

declare var $: any;

@Component({
    selector: 'modal-new-sync',
    templateUrl: './new-sync.component.html',
    styleUrls: ['./new-sync.component.css']
})
export class NewSyncComponent implements OnInit {  
    dtSyncs: DataTables.Settings = {};
    encrypted_id: String;    
    PendingSyncs: Syncronizer[];

    constructor(private httpClientService: HttpClient,
                private routerService: Router,
                private notifierService: NotifierService,
                private confirmDialogService: ConfirmDialogService) {
                  
    }
  
    ngOnInit(): void {
        this.PendingSyncs = [
            {
              encrypted_id: "231987213987987",
              name: "Sincronizador 1 Buquebus",
              state: "SOLICITANDO ENROLAR"
            },
            {
                encrypted_id: "231987213987987",
                name: "Sincronizador 1 Carrefour",
                state: "SOLICITANDO ENROLAR"
            },
            {
                encrypted_id: "231987213987987",
                name: "Sincronizador 2 Carrefour",
                state: "SOLICITANDO ENROLAR"
            },
            {
                encrypted_id: "231987213987987",
                name: "Sincronizador 1 Techint SA",
                state: "SOLICITANDO ENROLAR"
            }
          ];
    }

    public Show(encrypted_id: String) {
        this.encrypted_id = encrypted_id;
        $("#new-sync-modal").modal('show');
    }

    public Hide() {
        $("#new-sync-modal").modal('hide');
    }

    onAddClick(syncId) {
        let _clientId = this.encrypted_id;
        let _syncId = syncId;

        this.confirmDialogService.showConfirm("¿Desea enrolar al sincronizador?",
            () => {
                this.notifierService.notify('success', 'Sincronizador enrolado correctamente.');
                this.Hide();
            },
            () => {
                this.Hide();
            });
    }
}