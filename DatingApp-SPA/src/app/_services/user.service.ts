import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.httpClient.get<User[]>(this.baseUrl + 'users');
  }

  getUser(id: number): Observable<User> {
    return this.httpClient.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(user: User) {
    return this.httpClient.put(this.baseUrl + 'users', user);
  }

  setMainPhoto(id: number) {
    return this.httpClient.post(this.baseUrl + 'users/photos/' + id + '/setMain', {});
  }

  deletePhoto(id: number) {
    return this.httpClient.delete(this.baseUrl + 'users/photos/' + id);
  }
}
