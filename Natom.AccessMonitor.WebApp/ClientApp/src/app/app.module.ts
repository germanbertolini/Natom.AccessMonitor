import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { DevicesComponent } from './views/devices/devices.component';
import { DeviceCrudComponent } from './views/devices/crud/device-crud.component';
import { DataTablesModule } from 'angular-datatables';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { NotifierModule } from 'angular-notifier';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { CommonModule } from '@angular/common';
import { ConfirmDialogService } from './components/confirm-dialog/confirm-dialog.service';
import { MeProfileComponent } from './views/me/profile/me-profile.component';
import { UsersComponent } from './views/users/users.component';
import { UserCrudComponent } from './views/users/crud/user-crud.component';
import { DocketsComponent } from './views/dockets/dockets.component';
import { DocketCrudComponent } from './views/dockets/crud/docket-crud.component';
import { TitleCrudComponent } from './views/titles/crud/title-crud.component';
import { TitlesComponent } from './views/titles/titles.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    DevicesComponent,
    DeviceCrudComponent,
    UsersComponent,
    UserCrudComponent,
    MeProfileComponent,
    DocketsComponent,
    DocketCrudComponent,
    TitlesComponent,
    TitleCrudComponent,
    ConfirmDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    DataTablesModule,
    AngularFontAwesomeModule,
    NotifierModule,
    BrowserModule,
    CommonModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'devices', component: DevicesComponent },
      { path: "devices/new", component: DeviceCrudComponent },
      { path: "devices/edit/:id", component: DeviceCrudComponent },
      { path: 'users', component: UsersComponent },
      { path: "users/new", component: UserCrudComponent },
      { path: "users/edit/:id", component: UserCrudComponent },
      { path: "me/profile", component: MeProfileComponent },
      { path: 'dockets', component: DocketsComponent },
      { path: "dockets/new", component: DocketCrudComponent },
      { path: "dockets/edit/:id", component: DocketCrudComponent },
      { path: 'titles', component: TitlesComponent },
      { path: "titles/new", component: TitleCrudComponent },
      { path: "titles/edit/:id", component: TitleCrudComponent }
    ])
  ],
  exports: [  
    ConfirmDialogComponent  
  ], 
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ],
  providers: [ ConfirmDialogService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
