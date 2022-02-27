import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';

//Local imports
import localeEsAR from '@angular/common/locales/es-AR';

//Register local imports
import { DatePipe, registerLocaleData } from '@angular/common';
import { NgbDateCustomParserFormatter, NgbdDatepickerPopup } from './utils/datepicker/datepicker-popup';
registerLocaleData(localeEsAR, 'es-AR');

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './views/home/home.component';
import { DevicesComponent } from './views/devices/devices.component';
import { DataTablesModule } from 'angular-datatables';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { NotifierModule } from 'angular-notifier';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from './components/confirm-dialog/confirm-dialog.service';
import { MeProfileComponent } from './views/me/profile/me-profile.component';
import { DocketsComponent } from './views/dockets/dockets.component';
import { DocketCrudComponent } from './views/dockets/crud/docket-crud.component';
import { TitleCrudComponent } from './views/titles/crud/title-crud.component';
import { TitlesComponent } from './views/titles/titles.component';
import { DevicesSyncConfigComponent } from './views/devices/sync/config/devices-sync-config.component';
import { SidebarModule } from 'ng-sidebar';
import { NavSidebarComponent } from './components/nav-sidebar/nav-sidebar.component';
import { Query1AComponent } from './views/queries/1/A/query-1-a.component';
import { Query1BComponent } from './views/queries/1/B/query-1-b.component';
import { MeOrganizationComponent } from './views/me/organization/me-organization.component';
import { ReportsAttendanceByDeviceComponent } from './views/reports/attendance/reports-attendance-by-device.component';
import { ReportsWorkedHoursByDocketComponent } from './views/reports/worked-hours/reports-worked-hours-by-docket.component';
import { AppRoutingModule } from './app.routing.module';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { ErrorPageComponent } from './views/error-page/error-page.component';
import { LoginComponent } from './views/login/login.component';
import { CookieService } from 'ngx-cookie-service';
import { SpinnerLoadingComponent } from './components/spinner-loading/spinner-loading.component';
import { AppConfig } from './classes/app-config';
import { JsonAppConfigService } from './services/json-app-config.service';
import { NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SpinnerLoadingService } from './components/spinner-loading/spinner-loading.service';
import { ApiService } from './services/api.service';
import { UsersComponent } from './views/users/users.component';
import { UserCrudComponent } from './views/users/crud/user-crud.component';
import { UserConfirmComponent } from './views/users/confirm/user-confirm.component';
import { GoalsComponent } from './views/goals/goals.component';
import { PlaceCrudComponent } from './views/places/crud/place-crud.component';
import { GoalCrudComponent } from './views/goals/crud/goal-crud.component';
import { PlacesComponent } from './views/places/places.component';
import { BackgroundService } from './services/background.service';
import { HorarioCrudComponent } from './views/horarios/crud/horario-crud.component';
import { HorariosComponent } from './views/horarios/horarios.component';

export function OnInit(jsonAppConfigService: JsonAppConfigService) {
  return () => {
    return jsonAppConfigService.load();
  };
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NavSidebarComponent,
    LoginComponent,
    HomeComponent,
    ErrorPageComponent,
    DevicesComponent,
    UsersComponent,
    UserCrudComponent,
    UserConfirmComponent,
    MeProfileComponent,
    MeOrganizationComponent,
    DocketsComponent,
    DocketCrudComponent,
    TitlesComponent,
    TitleCrudComponent,
    PlacesComponent,
    PlaceCrudComponent,
    GoalsComponent,
    GoalCrudComponent,
    HorariosComponent,
    HorarioCrudComponent,
    DevicesSyncConfigComponent,
    Query1AComponent,
    Query1BComponent,
    ReportsAttendanceByDeviceComponent,
    ReportsWorkedHoursByDocketComponent,
    ConfirmDialogComponent,
    SpinnerLoadingComponent
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
    ChartsModule,
    NgbModule
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
    ApiService,
    BackgroundService,
    DatePipe ],
  bootstrap: [AppComponent]
})
export class AppModule { }
