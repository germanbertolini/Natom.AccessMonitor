import { Injectable } from '@angular/core';
import { HandledError } from '../classes/errors/handled.error';
import { LoginResult } from '../classes/models/auth/login-result.model';
import { ApiResult } from '../classes/models/shared/api-result.model';
import { User } from "../classes/models/user.model";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _current_user: User;
  private _current_token: string;
  private _current_permissions: Array<string>;
  
  constructor() {
    this._current_user = null;
    this._current_token = null;
    this._current_permissions = null;
  }

  public getCurrentUser() {
    return this._current_user;
  }

  public getCurrentToken() {
    return this._current_token;
  }

  public getCurrentPermissions() {
    return this._current_permissions;
  }

  public Login(email: string, password: string): LoginResult {
    //MOCK RESPUESTA API
    let response = new ApiResult<LoginResult>();
    if (email === "german.bertolini@gmail.com" && password === "1234") {
      response.success = true;
      response.message = null;
      
      response.data = new LoginResult();

      response.data.User = new User();
      response.data.User.encrypted_id = "adssdadas123e213132";
      response.data.User.first_name = "German";
      response.data.User.last_name = "Bertolini";
      response.data.User.picture_url = "https://lh3.googleusercontent.com/ogw/ADea4I77Za6iqEqbdUL2uqgk2F88wtfI43U8O3gxDBdbRg=s128-c-mo";
      response.data.User.email = "german.bertolini@gmail.com";
      response.data.User.registered_at = new Date('2020-12-28T00:00:00');
      response.data.User.business_name = "Natom";
      response.data.User.business_role_name = "Administrador";
      response.data.User.country_icon = "arg";
      response.data.Token = "98cb7b439xbx349c8273bc98b73c48927c9";

      response.data.Permissions = new Array<string>();
      response.data.Permissions.push("/");
      response.data.Permissions.push("/queries/1/a");
      response.data.Permissions.push("/queries/1/b");
    }
    else {
      response.success = false;
      response.message = "Usuario y/o clave inválida.";
      response.data = null;
    }
    //FIN MOCK RESPUESTA API

    if (!response.success)
      throw new HandledError(response.message);
      
    this._current_user = response.data.User;
    this._current_token = response.data.Token;
    this._current_permissions = response.data.Permissions.map(function(permission) {
      return permission.toLowerCase();
    });

    return response.data;
  }
}
