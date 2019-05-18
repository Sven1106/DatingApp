import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter<Boolean>();
  model: any = {};
  registerForm: FormGroup;
  constructor(private authService: AuthService, private alertifyService: AlertifyService, private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    },
      { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(formGroup: FormGroup) {
    return formGroup.get('password').value === formGroup.get('confirmPassword').value ? null : { 'mismatch': true };
  }
  register() {
    // this.authService.register(this.model).subscribe(() => {

    //   this.alertifyService.success('registration successful');
    // }, error => {
    //   this.alertifyService.error(error);
    // });
    console.log(this.registerForm.value);
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
