import { Component, Input } from '@angular/core';
import { ErrorService } from "./error.service";
import { ServerError } from "./server-error";

@Component({
  selector: 'app-errors',
  templateUrl: './app-errors.component.html',
  styleUrls: ['./app-errors.component.css']
})
export class AppErrorsComponent {
  errors: string[] = [];
  @Input() public timeout = 2000;
  constructor(private errorService: ErrorService) {
    errorService.serverErrorOccurred.subscribe((e: ServerError) => {
      this.errors.push(e.text);
      if (this.timeout > 0) {
        setTimeout(() => this.clear(), this.timeout);
      }
    });
  }

  clear() {
    this.errors = [];
  }
}
