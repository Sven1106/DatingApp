import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor(public _auth: AuthService, private _alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this._auth.login(this.model)
      .subscribe( // the _auth.login returns an observable, and we always need to subscribe to them
        next => {
          this._alertify.success('Logged in');
        }, error => {
          this._alertify.error(error);
        });
  }

  loggedIn() {
    // return this._auth.loggedIn();
    const token = localStorage.getItem('token');
    return !!token;
  }

  logOut() {
    localStorage.removeItem('token');
    this._alertify.message('Logged out');
  }
}
