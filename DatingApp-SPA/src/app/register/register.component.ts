import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter<Boolean>();
  model: any = {};
  constructor(private _auth: AuthService, private _alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    this._auth.register(this.model).subscribe(() => {

      this._alertify.success('registration successful');
    }, error => {
      this._alertify.error(error);
    });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
