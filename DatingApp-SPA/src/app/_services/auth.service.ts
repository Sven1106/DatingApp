import { Photo } from '../_models/photo';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelperService = new JwtHelperService();
  decodedToken: any;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private httpClient: HttpClient) { }

  setGlobalPhotoUrl(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.httpClient
      .post(this.baseUrl + 'login', model) // it return an Object, and this Object consists of Key = 'Token' & Value = 'JWT'
      .pipe( // This allows us to chain rxjs operators to our request
        map((response: any) => { // We use map here since we want to transform the received object.
          const user = response;
          if (user) {
            this.setDecodedToken(user.token);
            this.setMainPhoto(user.mainPhoto);
          }
        })
      );
  }

  register(model: any) {
    return this.httpClient.post(this.baseUrl + 'register', model);
  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelperService.isTokenExpired(token);
  }

  getDecodedToken() {
    const token = localStorage.getItem('token');
    if (token) {
      this.decodedToken = this.jwtHelperService.decodeToken(token);
    }
  }
  setDecodedToken(token: string) {
    localStorage.setItem('token', token);
    this.decodedToken = this.jwtHelperService.decodeToken(token);
  }

  getMainPhoto() {
    const mainPhoto: string = localStorage.getItem('mainPhoto');
    if (mainPhoto) {
      this.setGlobalPhotoUrl(mainPhoto);
    }
  }
  setMainPhoto(photoUrl: string) {
    localStorage.setItem('mainPhoto', photoUrl);
    this.setGlobalPhotoUrl(photoUrl);
  }

}
