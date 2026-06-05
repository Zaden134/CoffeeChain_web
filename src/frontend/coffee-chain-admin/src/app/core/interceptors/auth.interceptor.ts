import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthStore } from '../services/auth.store';

// authInterceptor tu dong gan bearer token vao request API sau khi login.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStore = inject(AuthStore);
  const token = authStore.token();

  if (!token) {
    return next(req);
  }

  const authorizedRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  if (req.url.includes('/auth/refresh') || req.url.includes('/auth/login')) {
    return next(authorizedRequest);
  }

  return next(authorizedRequest).pipe(
    catchError((error) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      return from(authStore.refreshSession()).pipe(
        switchMap((response) => {
          if (!response) {
            return throwError(() => error);
          }

          return next(
            req.clone({
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`
              }
            })
          );
        })
      );
    })
  );
};
