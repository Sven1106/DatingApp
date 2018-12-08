import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';
  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model) // it return an Object, and this Object consists of Key = 'Token' & Value = 'JWT'
      .pipe( // This allows us to chain rxjs operators to our request
        map((response: any) => { // We use map here since we want to transform the received object.
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
          }
        })
      );
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'register',  model);
  }
}
