import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

interface IProblemDetail {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
  errors: { [key: string]: string[] };
}

function isProblemDetail(obj: any): obj is IProblemDetail {
  return obj && typeof obj === 'object' && 'errors' in obj;
}

export const globalInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 400) {
        const badResponse = error.error;
        console.error('Bad Request Error:', badResponse);

        if (isProblemDetail(badResponse)) {
          const _bad = badResponse as IProblemDetail;
          const keys = Object.keys(_bad.errors);
          const messages = keys
            .map((key) => `${key}: ${_bad.errors[key].join(', ')}`)
            .join('\n');
          console.error('Validation Errors:', messages);
          alert(messages);
        } else if (badResponse && badResponse.message) {
          console.error('Error Message:', badResponse.message);
          alert(badResponse.message);
        }
      } else if (error.status === 404) {
        const { message } = error.error;
        console.warn('Not Found Error:', message);
        alert(message);
      }
      return throwError(() => error);
    })
  );
};
