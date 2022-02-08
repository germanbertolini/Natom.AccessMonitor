import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guards";
import { ClientsCrudComponent } from "./views/clients/crud/clients-crud.component";
import { ClientsComponent } from "./views/clients/clients.component";
import { HomeComponent } from "./views/home/home.component";
import { LoginComponent } from "./views/login/login.component";
import { ABMUsuariosGuard } from "./guards/usuarios/abm.usuarios.guards";
import { UsersComponent } from "./views/users/users.component";
import { UserCrudComponent } from "./views/users/crud/user-crud.component";
import { ErrorPageComponent } from "./views/error-page/error-page.component";
import { UserConfirmComponent } from "./views/users/confirm/user-confirm.component";

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, pathMatch: 'full' },
    { canActivate: [ AuthGuard ], path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'error-page', component: ErrorPageComponent, data: { message: "Se ha producido un error" } },
    { path: 'forbidden', component: ErrorPageComponent, data: { message: "No tienes permisos" } },
    { path: 'not-found', component: ErrorPageComponent, data: { message: "No se encontró la ruta solicitada" } },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: 'users', component: UsersComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/new", component: UserCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/edit/:id", component: UserCrudComponent },
    { canActivate: [ AuthGuard ], path: "users/confirm/:data", component: UserConfirmComponent },
    { canActivate: [ AuthGuard ], path: 'clients', component: ClientsComponent },
    { canActivate: [ AuthGuard ], path: "clients/new", component: ClientsCrudComponent },
    { canActivate: [ AuthGuard ], path: "clients/edit/:id", component: ClientsCrudComponent }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})
export class AppRoutingModule {

}