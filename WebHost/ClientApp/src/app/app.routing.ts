import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FolderViewerComponent } from './folder/folder-viewer.component';

const appRoutes: Routes = [
  { path: '', component: FolderViewerComponent, pathMatch: 'full' }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
