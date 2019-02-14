import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelperService = new JwtHelperService();
  decodedToken: any;

  constructor(private httpClient: HttpClient) { }

  login(model: any) {
    return this.httpClient
      .post(this.baseUrl + 'login', model) // it return an Object, and this Object consists of Key = 'Token' & Value = 'JWT'
      .pipe( // This allows us to chain rxjs operators to our request
        map((response: any) => { // We use map here since we want to transform the received object.
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            this.decodedToken = this.jwtHelperService.decodeToken(user.token);
            console.log(this.decodedToken);
          }
        })
      );
  }

  register(model: any) {
    return this.httpClient.post(this.baseUrl + 'register', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelperService.isTokenExpired(token);
  }

  getDecodedToken() {
    const token = localStorage.getItem('token');
    if (token) {
      this.decodedToken = this.jwtHelperService.decodeToken(token);
    }
  }
}
