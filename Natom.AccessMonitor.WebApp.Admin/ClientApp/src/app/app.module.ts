import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

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

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    LoginComponent,
    HomeComponent,
    ClientsComponent,
    ClientsCrudComponent,
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
    ConfirmDialogComponent  
  ], 
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ],
  providers: [ ConfirmDialogService, ThemeService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
