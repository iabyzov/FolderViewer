import { Response } from "@angular/http";
import { ApiError } from "../../apiReference/Errors_ApiError";

export class ServerError {
  public text: string;

  public static createFromResponse(response: Response): ServerError[] {
    let apiError: ApiError;
    
    let body = response.text();
    if (body) {
      try {
        apiError = JSON.parse(body);
      } catch (e) {
        let errorMessage =
          `The request body does not contains valid json:\n${body}`;

        throw new Error(errorMessage);
      }
    }
    return apiError.errors.map(v => {
      return { text: v.message };
    });
  }
}
