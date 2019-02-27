import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

import { Router } from '@angular/router';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor(public authService: AuthService, private alertifyService: AlertifyService, private router: Router) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model)
      .subscribe( // returns an observable, and we always need to subscribe to them
        next => {
          this.alertifyService.success('Logged in');
          this.router.navigate(['/members']);
        }, error => {
          this.alertifyService.error(error);
        });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logOut() {
    localStorage.removeItem('token');
    this.authService.decodedToken = null;
    this.alertifyService.message('Logged out');
    this.router.navigate(['/home']);
  }
}
