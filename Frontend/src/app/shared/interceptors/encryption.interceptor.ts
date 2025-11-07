import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpEvent,
  HttpResponse
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { CryptoService } from '../services/crypto.service';
import { AppSettingsService } from '../services/app-settings.service';

export const encryptionInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const crypto = inject(CryptoService);
  const settings = inject(AppSettingsService);

  const isEncrypted = settings.useEncryption;
  const shouldEncrypt = isEncrypted && req.method !== 'GET' && !req.headers.has('X-No-Encrypt');

  // 🔐 Encrypt request
  let modifiedReq = shouldEncrypt
    ? (() => {
        try {
          const encryptedBody = crypto.encrypt(req.body);
          return req.clone({
            body: { data: encryptedBody },
            headers: req.headers.set('X-Encrypted', 'true')
          });
        } catch (err) {
          console.error('[EncryptionInterceptor] Request encryption failed:', err);
          return req;
        }
      })()
    : req;

  return next(modifiedReq).pipe(
    map(event => {
      if (
        isEncrypted &&
        event instanceof HttpResponse &&
        event.body &&
        typeof event.body === 'object' &&
        'data' in event.body &&
        typeof event.body.data === 'string'
      ) {
        try {
          const decrypted = crypto.decrypt(event.body.data);
          return event.clone({ body: decrypted });
        } catch (err) {
          console.error('[Decryption failed]:', err);
        }
      }

      return event;
    })
  );
};
