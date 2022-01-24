import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guards";
import { ClientsCrudComponent } from "./views/clients/crud/clients-crud.component";
import { ClientsComponent } from "./views/clients/clients.component";
import { HomeComponent } from "./views/home/home.component";
import { LoginComponent } from "./views/login/login.component";

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, pathMatch: 'full' },
    { canActivate: [ AuthGuard ], path: '', component: HomeComponent, pathMatch: 'full' },
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