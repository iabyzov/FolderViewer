import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { FolderService } from '../apiReference/Folder_FolderService';
import { FolderDto } from '../apiReference/Folder_FolderDto';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'folder-viewer',
  templateUrl: './folder-viewer.component.html',
  styleUrls: ['./folder-viewer.component.css']
})
export class FolderViewerComponent implements OnInit{
  folderForm: FormGroup;
  folders: Observable<FolderDto[]>;
  protected ngUnsubscribe: Subject<void> = new Subject<void>();
  ngOnInit() {
    
  }

  constructor(private folderService: FolderService, private ngxService: NgxUiLoaderService) {
    this.folderForm = new FormGroup({
      'path': new FormControl('', [Validators.required, Validators.pattern(/^[a-zA-Z]:\\(\w+\\)*\w*$/)])
    });
  }

  get path() {
    return this.folderForm.get('path');
  }

  submitForm() {
    this.ngxService.startBackground();

    this.folders = this.folderService.getSortedFolders(this.path.value, 5)
      .finally(() => {
        this.ngxService.stopBackground();
      })
      .takeUntil(this.ngUnsubscribe);
  }

  get isLoading(): boolean {
    return this.ngxService.hasBackground();
  }

  cancel() {
    this.ngUnsubscribe.next();
  }
}
