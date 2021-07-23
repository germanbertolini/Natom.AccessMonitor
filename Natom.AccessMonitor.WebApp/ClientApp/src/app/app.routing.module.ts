import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DeviceCrudComponent } from "./views/devices/crud/device-crud.component";
import { DevicesComponent } from "./views/devices/devices.component";
import { DevicesSyncConfigComponent } from "./views/devices/sync/config/devices-sync-config.component";
import { DocketCrudComponent } from "./views/dockets/crud/docket-crud.component";
import { DocketsComponent } from "./views/dockets/dockets.component";
import { HomeComponent } from "./views/home/home.component";
import { MeOrganizationComponent } from "./views/me/organization/me-organization.component";
import { MeProfileComponent } from "./views/me/profile/me-profile.component";
import { Query1AComponent } from "./views/queries/1/A/query-1-a.component";
import { Query1BComponent } from "./views/queries/1/B/query-1-b.component";
import { ReportsAttendanceByDeviceComponent } from "./views/reports/attendance/reports-attendance-by-device.component";
import { ReportsWorkedHoursByDocketComponent } from "./views/reports/worked-hours/reports-worked-hours-by-docket.component";
import { ErrorPageComponent } from "./views/error-page/error-page.component";
import { TitleCrudComponent } from "./views/titles/crud/title-crud.component";
import { TitlesComponent } from "./views/titles/titles.component";
import { UserCrudComponent } from "./views/users/crud/user-crud.component";
import { UsersComponent } from "./views/users/users.component";

const appRoutes: Routes = [
    { path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'error-page', component: ErrorPageComponent, data: { message: "Se ha producido un error" } },
    { path: 'forbidden', component: ErrorPageComponent, data: { message: "No tienes permisos" } },
    { path: 'not-found', component: ErrorPageComponent, data: { message: "No se encontró la ruta solicitada" } },
    { path: 'devices', component: DevicesComponent },
    { path: "devices/new", component: DeviceCrudComponent },
    { path: "devices/edit/:id", component: DeviceCrudComponent },
    { path: "devices/sync/config", component: DevicesSyncConfigComponent },
    { path: 'users', component: UsersComponent },
    { path: "users/new", component: UserCrudComponent },
    { path: "users/edit/:id", component: UserCrudComponent },
    { path: "me/profile", component: MeProfileComponent },
    { path: "me/organization", component: MeOrganizationComponent },
    { path: 'dockets', component: DocketsComponent },
    { path: "dockets/new", component: DocketCrudComponent },
    { path: "dockets/edit/:id", component: DocketCrudComponent },
    { path: 'titles', component: TitlesComponent },
    { path: "titles/new", component: TitleCrudComponent },
    { path: "titles/edit/:id", component: TitleCrudComponent },
    { path: "queries/1/a", component: Query1AComponent },
    { path: "queries/1/b", component: Query1BComponent },
    { path: "reports/attendance/by_device", component: ReportsAttendanceByDeviceComponent },
    { path: "reports/worked-hours/by_docket/:id", component: ReportsWorkedHoursByDocketComponent }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})
export class AppRoutingModule {

}