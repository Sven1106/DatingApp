import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { Photo } from '../../_models/photo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  baseUrl = environment.apiUrl;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  hasAnotherDropZoneOver = false;
  currentMainPhoto: Photo;

  constructor(private userService: UserService, private alertifyService: AlertifyService, private authService: AuthService) { }

  ngOnInit() {
    this.initializeUploader();
  }
  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo: Photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) { // since we are parsing an object from the Photo[] to the function it can deter what object it is handling
    return this.userService.setMainPhoto(photo.id).subscribe(next => {

      this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
      this.currentMainPhoto.isMain = false;
      photo.isMain = true;

      this.authService.setMainPhoto(photo.url);
    },
      error => {
        this.alertifyService.error(error);
      });
  }

  deletePhoto(photo: Photo) {
    this.alertifyService.confirm('Are you sure you want to delete this photo?', () => {
      return this.userService.deletePhoto(photo.id).subscribe(next => {
        const indexInPhotoArray = this.photos.findIndex(p => p.id === photo.id);
        this.photos.splice(indexInPhotoArray, 1);
        this.alertifyService.success('Photo has been deleted');
      }, error => {
        this.alertifyService.error('Failed to delete the photo');
      });
    }
    );

  }
}
