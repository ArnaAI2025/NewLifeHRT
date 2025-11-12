import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders,
  HttpErrorResponse,
  HttpParams
} from '@angular/common/http';
import {
  Observable,
  throwError,
  timeout,
  catchError,
  switchMap,
  of
} from 'rxjs';
import { AppSettingsService } from './app-settings.service';
import { HttpRequestOptions } from '../models/http-request-options.model';
import { Router } from '@angular/router';
import { ApiResource } from '../constants/api-resource';
import { ApiHelper } from '../constants/api.helper';
import { authModel } from '../models/auth-request-response.model';

@Injectable({ providedIn: 'root' })
export class HttpService {
  private readonly DEFAULT_TIMEOUT = 30000;

  constructor(
    private http: HttpClient,
    private settings: AppSettingsService,
    private router: Router,
  ) {}

  get<T>(url: string, options?: HttpRequestOptions): Observable<T> {
    return this.request<T>('GET', url, null, options);
  }

  post<T>(url: string, body: any, options?: HttpRequestOptions): Observable<T> {
    return this.request<T>('POST', url, body, options);
  }

  put<T>(url: string, body: any, options?: HttpRequestOptions): Observable<T> {
    return this.request<T>('PUT', url, body, options);
  }

  patch<T>(url: string, body: any, options?: HttpRequestOptions): Observable<T> {
    return this.request<T>('PATCH', url, body, options);
  }

  delete<T>(url: string, options?: HttpRequestOptions): Observable<T> {
    return this.request<T>('DELETE', url, null, options);
  }

  private request<T>(
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE',
    url: string,
    body: any,
    options?: HttpRequestOptions
  ): Observable<T> {
    const mergedOptions: HttpRequestOptions = {
      attachToken: true,
      timeout: this.DEFAULT_TIMEOUT,
      observe: 'body',
      responseType: 'json',
      ...options
    };

    const headers = this.getHeaders(mergedOptions.attachToken!, mergedOptions.headers, body);
    const params = mergedOptions.params
      ? new HttpParams({ fromObject: mergedOptions.params })
      : undefined;

    const requestOptions: any = {
      headers,
      params,
      observe: mergedOptions.observe,
      responseType: mergedOptions.responseType
    };

    if (method !== 'GET' && method !== 'DELETE') {
      requestOptions.body = body;
    }

    return this.http.request<T>(method, url, requestOptions).pipe(
      timeout(mergedOptions.timeout!),
      catchError(err => this.handleError<T>(err, method, url, body, mergedOptions))
    );
  }

  private getHeaders(
    attachToken: boolean,
    customHeaders?: Record<string, string>,
     body?: any
  ): HttpHeaders {
    let headers = new HttpHeaders();

  if (!(body instanceof FormData)) {
    headers = headers.set('Content-Type', 'application/json');
  }

    if (!attachToken) {
      headers = headers.set('X-No-Auth', 'true');
    } else {
      const token = this.settings.authToken;
      if (!token || this.settings.isUserLoggedIn() === false) {
      throw new Error('No token available or user logged out.');
    }
      if (token) {
        headers = headers.set('Authorization', `Bearer ${token}`);
      } else {
        console.warn('[HttpService] No auth token found!');
      }
    }

    if (customHeaders) {
      for (const [key, value] of Object.entries(customHeaders)) {
        headers = headers.set(key, value);
      }
    }

    return headers;
  }

private handleError<T>(
  err: any,
  method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE',
  url: string,
  body: any,
  options: HttpRequestOptions
): Observable<any> {
  console.error(`[HttpService] ${method} ${url} failed:`, err);
    if (err?.status === 401 && this.settings.isUserLoggedIn()) {
    return this.refreshToken().pipe(
      switchMap(success => {
        if (success) {
          console.log('[HttpService] Retrying request after token refresh...');
          return this.request<T>(method, url, body, options);
        } else {
          this.router.navigate(['/logout']);
          return throwError(() => err);
        }
      }),
      catchError(refreshErr => {
        console.error('[HttpService] Refresh failed during retry:', refreshErr);
        this.router.navigate(['/logout']);
        return throwError(() => refreshErr);
      })
    );  
  }

  return throwError(() => err);
}


 refreshToken(): Observable<boolean> {
  const refreshToken = this.settings.getRefreshTokenFromStorage();
  const accessToken = this.settings.getAuthTokenFromStorage();

  if (!refreshToken || !accessToken) {
    console.warn('[HttpService] No refresh or access token available.');
    this.router.navigate(['/logout']);
    return of(false);
  }

  const url = ApiResource.getURI(ApiHelper.auth.base, ApiHelper.auth.refreshToken);

  const request: authModel = {
    accessToken,
    refreshToken,
  };

  return this.http
    .post<authModel>( 
      url,
      request,
      {
        headers: new HttpHeaders({
          'X-No-Auth': 'true',
          'Content-Type': 'application/json'
        })
      }
    )
    .pipe(
      switchMap(res => {
        if (res?.accessToken && res?.refreshToken) {
          this.settings.setTokens(res.accessToken, res.refreshToken);
          return of(true);
        }
        return of(false);
      }),
      catchError(err => {
        console.error('[HttpService] Token refresh failed:', err);
        this.router.navigate(['/logout']);
        return of(false);
      })
    );
}

}
