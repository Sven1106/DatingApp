import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: any;

  constructor(private http: HttpClient) {
    this.getValues();
  } // the constructor executes too early for the application to get data from an api

  ngOnInit() { // happens after the component is initialized.

  }
  getValues() {
    this.http.get('http://localhost:5000/api/values/').subscribe(
      response => { // the first parameter takes a callback/function that tells what to do when the result comes back
        this.values = response;
      }, error => { // the second parameter takes a callback/function that tells what to do if an error occurred
        console.log(error);
      });
  }
}
