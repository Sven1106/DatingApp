import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';

import { AuthService } from './../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';

import { User } from '../_models/user';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(
        private userService: UserService,
        private authService: AuthService,
        private alertifyService: AlertifyService,
        private router: Router) { }

    resolve(): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertifyService.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })

        );
    }
}
