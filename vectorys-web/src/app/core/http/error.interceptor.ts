import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err) => {
      const status = err?.status as number | undefined;
      const isLogin = req.url.includes('/auth/login');

      if (status === 401 && !isLogin) {
        auth.logout();
        void router.navigateByUrl('/login');
      } else if (status === 403) {
        alert('Accès refusé');
      } else if (status === 500) {
        alert('Erreur serveur');
      }

      return throwError(() => err);
    })
  );
};
