import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app.component';
import { FolderViewerComponent } from './folder/folder-viewer.component';
import { routing } from './app.routing';
import { ErrorService } from "./infrastructure/errorHandling/error.service";
import { FolderService } from "./apiReference/Folder_FolderService";
import { AppErrorsComponent } from "./infrastructure/errorHandling/app-errors.component";
import { NgxUiLoaderModule, NgxUiLoaderConfig, POSITION } from 'ngx-ui-loader';

const ngxUiLoaderConfig: NgxUiLoaderConfig = {
  bgsPosition: POSITION.centerCenter
};

@NgModule({
  declarations: [
    AppComponent,
    FolderViewerComponent,
    AppErrorsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    NgxUiLoaderModule.forRoot(ngxUiLoaderConfig),
    routing
  ],
  providers: [ErrorService, FolderService],
  bootstrap: [AppComponent]
})
export class AppModule { }
