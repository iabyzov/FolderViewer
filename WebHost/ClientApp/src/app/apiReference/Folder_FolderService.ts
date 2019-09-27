// ! Generated Code !

import {Http, Response} from "@angular/http";
import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/map';
import {ErrorService} from '../infrastructure/errorHandling/error.service';
import {ServerError} from '../infrastructure/errorHandling/server-error';

import {FolderDto} from "./Folder_FolderDto";

@Injectable()
export class FolderService {
    constructor(private http: Http, private errorService: ErrorService) {
    }

    public getSortedFolders(folder: string, limit: number):Observable<FolderDto[]> {
        return this.http.get(`api/Folder/${encodeURIComponent(folder)}/orderBySize?limit=${limit}`)
            .map((res:Response) => <FolderDto[]>(res.text() ? res.json() : null))
            .catch((error: Response) => this.handleError(error));
    }

    private handleError(error: Response): Observable<any> {
        const serverErrors = ServerError.createFromResponse(error);
        this.errorService.handleServerErrors(serverErrors);
        return Observable.throw(error.status);
    }
}
