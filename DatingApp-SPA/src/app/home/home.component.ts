import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode: Boolean = false;
  values: any;

  constructor(private http: HttpClient) {
  } // the constructor executes too early for the application to get data from an api

  ngOnInit() { // happens after the component is initialized.
  }

  registerToggle() {
    this.registerMode = true;
  }


  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
