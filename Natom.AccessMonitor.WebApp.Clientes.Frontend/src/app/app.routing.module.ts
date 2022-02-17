import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guards";
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
import { LoginComponent } from "./views/login/login.component";
import { UsersComponent } from "./views/users/users.component";
import { UserCrudComponent } from "./views/users/crud/user-crud.component";
import { UserConfirmComponent } from "./views/users/confirm/user-confirm.component";
import { ABMUsuariosGuard } from "./guards/usuarios/abm.usuarios.guards";
import { TitlesComponent } from "./views/titles/titles.component";
import { TitleCrudComponent } from "./views/titles/crud/title-crud.component";
import { ABMTitlesGuard } from "./guards/titles/abm.titles.guards";
import { SyncAdminGuard } from "./guards/syncs/sync.admin.guards";
import { ABMDocketsGuard } from "./guards/dockets/abm.dockets.guards";
import { ABMPlacesAndGoalsGuard } from "./guards/placesgoals.guards";
import { PlaceCrudComponent } from "./views/places/crud/place-crud.component";
import { PlacesComponent } from "./views/places/places.component";
import { GoalCrudComponent } from "./views/goals/crud/goal-crud.component";
import { GoalsComponent } from "./views/goals/goals.component";

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, pathMatch: 'full' },
    { canActivate: [ AuthGuard ], path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'error-page', component: ErrorPageComponent, data: { message: "Se ha producido un error" } },
    { path: 'forbidden', component: ErrorPageComponent, data: { message: "No tienes permisos" } },
    { path: 'not-found', component: ErrorPageComponent, data: { message: "No se encontró la ruta solicitada" } },
    { canActivate: [ AuthGuard, SyncAdminGuard ], path: 'devices', component: DevicesComponent },
    { canActivate: [ AuthGuard, SyncAdminGuard ], path: "devices/sync/config", component: DevicesSyncConfigComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: 'users', component: UsersComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/new", component: UserCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/edit/:id", component: UserCrudComponent },
    { canActivate: [ AuthGuard ], path: "users/confirm/:data", component: UserConfirmComponent },
    { canActivate: [ AuthGuard ], path: "me/profile", component: MeProfileComponent },
    { canActivate: [ AuthGuard ], path: "me/organization", component: MeOrganizationComponent },
    { canActivate: [ AuthGuard, ABMDocketsGuard ], path: 'dockets', component: DocketsComponent },
    { canActivate: [ AuthGuard, ABMDocketsGuard ], path: "dockets/new", component: DocketCrudComponent },
    { canActivate: [ AuthGuard, ABMDocketsGuard ], path: "dockets/edit/:id", component: DocketCrudComponent },
    { canActivate: [ AuthGuard, ABMTitlesGuard ], path: 'titles', component: TitlesComponent },
    { canActivate: [ AuthGuard, ABMTitlesGuard ], path: "titles/new", component: TitleCrudComponent },
    { canActivate: [ AuthGuard, ABMTitlesGuard ], path: "titles/edit/:id", component: TitleCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: 'places', component: PlacesComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "places/new", component: PlaceCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "places/edit/:id", component: PlaceCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: 'goals', component: GoalsComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "goals/new", component: GoalCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "goals/edit/:id", component: GoalCrudComponent },
    { canActivate: [ AuthGuard ], path: "queries/1/a", component: Query1AComponent },
    { canActivate: [ AuthGuard ], path: "queries/1/b", component: Query1BComponent },
    { canActivate: [ AuthGuard ], path: "reports/attendance/by_device", component: ReportsAttendanceByDeviceComponent },
    { canActivate: [ AuthGuard ], path: "reports/worked-hours/by_docket/:id", component: ReportsWorkedHoursByDocketComponent }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})

export class AppRoutingModule {

}