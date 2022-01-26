import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './views/home/home.component';
import { DataTablesModule } from 'angular-datatables';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { NotifierModule } from 'angular-notifier';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from './components/confirm-dialog/confirm-dialog.service';
import { ClientsComponent } from './views/clients/clients.component';
import { ClientsCrudComponent } from './views/clients/crud/clients-crud.component';
import { SidebarModule } from 'ng-sidebar';
import { AppRoutingModule } from './app.routing.module';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { LoginComponent } from './views/login/login.component';
import { NewSyncComponent } from './views/clients/modals/new-sync.component';
import { SpinnerLoadingComponent } from './components/spinner-loading/spinner-loading.component';
import { AppConfig } from './classes/app-config';
import { JsonAppConfigService } from './services/json-app-config.service';
import { ApiService } from './services/api.service';
import { CookieService } from 'ngx-cookie-service';
import { SpinnerLoadingService } from './components/spinner-loading/spinner-loading.service';
import { NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateCustomParserFormatter } from './utils/datepicker/datepicker-popup';



export function OnInit(jsonAppConfigService: JsonAppConfigService) {
  return () => {
    return jsonAppConfigService.load();
  };
}



@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    SpinnerLoadingComponent,
    LoginComponent,
    HomeComponent,
    ClientsComponent,
    ClientsCrudComponent,
    NewSyncComponent,
    ConfirmDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    SidebarModule.forRoot(),
    HttpClientModule,
    FormsModule,
    DataTablesModule,
    AngularFontAwesomeModule,
    NotifierModule,
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    ChartsModule
  ],
  exports: [  
    ConfirmDialogComponent,
    SpinnerLoadingComponent
  ], 
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ],
  providers: [
    {
      provide: LOCALE_ID,
      useValue: 'es-AR'
    },
    {
      provide: AppConfig,
      deps: [HttpClient],
      useExisting: JsonAppConfigService
    },
    {
      provide: APP_INITIALIZER,
      multi: true,
      deps: [JsonAppConfigService],
      useFactory: OnInit
    },
    { provide: NgbDateParserFormatter,
      useClass: NgbDateCustomParserFormatter
    },
    ConfirmDialogService,
    SpinnerLoadingService,
    ThemeService,
    CookieService,
    ApiService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
