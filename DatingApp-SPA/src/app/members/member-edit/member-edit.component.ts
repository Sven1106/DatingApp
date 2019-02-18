import { AlertifyService } from './../../_services/alertify.service';
import { User } from './../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  @ViewChild('editForm') editForm: NgForm;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  constructor(private activatedroute: ActivatedRoute, private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.activatedroute.data.subscribe(data => {
      this.user = data['user'];
    });
  }
  updateUser() {
    console.log(this.user);
    this.alertifyService.success('Profile updated successfully');
    this.editForm.reset(this.user);
  }
}
