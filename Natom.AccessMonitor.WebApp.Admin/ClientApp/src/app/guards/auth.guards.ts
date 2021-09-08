import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {

    constructor(private _authService: AuthService,
                private _router: Router) {
        
    }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean | UrlTree> |
        Promise<boolean | UrlTree> | boolean | UrlTree {
        
        if (this._authService.getCurrentUser() == null)
        {
            this._router.navigate(['/login']);
            return false;
        }

        //POR AHORA EN LA APLICACIÓN DE ADMIN CON QUE HAYA LOGUEADO LO DEJAMOS OPERAR!
        // if (this._authService.getCurrentPermissions().indexOf(state.url.toLowerCase()) < 0)
        // {
        //     this._router.navigate(['/forbidden']);
        //     return false;
        // }

        return true;

    }
}