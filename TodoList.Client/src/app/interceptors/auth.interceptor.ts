import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {

  const authService = inject(AuthService);
  const authToken = authService.getToken();
  const apiBaseUrl = 'https://localhost:7271/api'; 

  if (authToken && req.url.startsWith(apiBaseUrl)) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${authToken}`
      }
    });
    console.log('Interceptor: Adding auth header', authReq.url); 
    return next(authReq);
  } else {
    return next(req);
  }
};