import { Injectable } from "@angular/core";
import { Subject } from 'rxjs/Subject';
import { ServerError } from "./server-error";


@Injectable()
export class ErrorService {
  private serverErrorOccurred$ = new Subject<ServerError>();

  public get serverErrorOccurred(): Subject<ServerError> {
    return this.serverErrorOccurred$;
  }

  public handleServerErrors(responses: ServerError[]): void {
    responses.forEach(r => {
       this.serverErrorOccurred$.next(r);
    });
  }
}
