import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
  providers: [AuthService]
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor(private _auth: AuthService) { }

  ngOnInit() {
  }

  login() {
    this._auth.login(this.model)
      .subscribe( // the _auth.login returns an observable, and we always need to subscribe to them
        next => {
          console.log('Logged in');
        }, error => {
          console.log(error);
        });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; // shorthand for returning true of false.
  }

  logOut() {
    localStorage.removeItem('token');
    console.log('Logged out');
  }
}
