import {Injectable} from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {LoginService} from "../login/login.service";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private loginService: LoginService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(err => {

      if (err.status === 401) {
        this.loginService.logout();
        location.reload();
      }

      if (err instanceof HttpErrorResponse) {
        if (!err.error.message) {
          return throwError(err.error);
        }

        return throwError(err.error.message);
      }

      return throwError(err);
    }))
  }
}
