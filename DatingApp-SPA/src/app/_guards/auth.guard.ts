import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private _auth: AuthService, private router: Router, private _alertify: AlertifyService) { }

  canActivate(): boolean {
    if (this._auth.loggedIn()) {
      return true;
    }
    this._alertify.error('You shall not PASS!!!!');
    this.router.navigate(['/home']);
    return false;
  }
}
