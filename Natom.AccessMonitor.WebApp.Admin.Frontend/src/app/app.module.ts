import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';

//Local imports
import localeEsAR from '@angular/common/locales/es-AR';

//Register local imports
import { registerLocaleData } from '@angular/common';
import { NgbDateCustomParserFormatter, NgbdDatepickerPopup } from './utils/datepicker/datepicker-popup';
registerLocaleData(localeEsAR, 'es-AR');

//Components
import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './views/home/home.component';
import { DataTablesModule } from 'angular-datatables';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { NotifierModule } from 'angular-notifier';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from './components/confirm-dialog/confirm-dialog.service';
import { SidebarModule } from 'ng-sidebar';
import { AppRoutingModule } from './app.routing.module';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { LoginComponent } from './views/login/login.component';
import { SpinnerLoadingComponent } from './components/spinner-loading/spinner-loading.component';
import { AppConfig } from './classes/app-config';
import { JsonAppConfigService } from './services/json-app-config.service';
import { ApiService } from './services/api.service';
import { CookieService } from 'ngx-cookie-service';
import { SpinnerLoadingService } from './components/spinner-loading/spinner-loading.service';
import { NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { ErrorPageComponent } from './views/error-page/error-page.component';
import { UsersComponent } from './views/users/users.component';
import { UserCrudComponent } from './views/users/crud/user-crud.component';
import { UserConfirmComponent } from './views/users/confirm/user-confirm.component';
import { ClientesComponent } from './views/clientes/clientes.component';
import { ClienteCrudComponent } from './views/clientes/crud/cliente-crud.component';
import { UsuarioClientesCrudComponent } from './views/clientes/usuarios/crud/usuario-clientes-crud.component';
import { UsuariosClientesComponent } from './views/clientes/usuarios/usuarios-clientes.component';
import { SyncsClientesComponent } from './views/clientes/syncs/syncs-clientes.component';
import { DevicesSyncsClientesComponent } from './views/clientes/syncs/devices/devices-syncs-clientes.component';
import { SyncsClientesNewComponent } from './views/clientes/syncs/crud/syncs-clientes-new.component';



export function OnInit(jsonAppConfigService: JsonAppConfigService) {
  return () => {
    return jsonAppConfigService.load();
  };
}



@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ErrorPageComponent,
    SpinnerLoadingComponent,
    LoginComponent,
    HomeComponent,
    ClientesComponent,
    ClienteCrudComponent,
    UsersComponent,
    UserCrudComponent,
    UserConfirmComponent,
    UsuariosClientesComponent,
    UsuarioClientesCrudComponent,
    SyncsClientesComponent,
    SyncsClientesNewComponent,
    DevicesSyncsClientesComponent,
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