// src/app/shared/services/app-settings.service.ts

import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppSettings } from '../models/app-settings.model';
import { AppConfig } from '../models/app-config.model';
import { UserAccountService } from '../services/user-account.service';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class AppSettingsService {
  private readonly SETTINGS_KEY = 'settings';
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  public apiBaseUrl: string = '';
  public authToken: string | null = null;
  public refreshToken: string | null = null;
  public useEncryption: boolean = true;
  public tenantName:string|null = "";
  public inactivityTimeLimit:number = 0;
  public refreshTokenInterval:number = 0;
  private loggedIn$ = new BehaviorSubject<boolean>(false);
  constructor(
    private http: HttpClient,
    private readonly userAccountService: UserAccountService
  ) {}

  async load(): Promise<void> {
    try {
      const config = await this.http
        .get<AppSettings>('assets/settings.json')
        .toPromise();

      const baseSettings = {
        baseURI: config?.apiBaseUrl ?? 'http://localhost:5141/api/',
        useEncryption: config?.useEncryption ?? true,
        tenantName:config?.tenantName ?? ""
      };

      // Store basic settings only (not tokens or user)
      localStorage.setItem("newLifeHRT:" + this.SETTINGS_KEY, JSON.stringify(baseSettings));
      this.applySettings(baseSettings);
      AppConfig.settings = config ?? AppConfig.settings;

      const accessToken = localStorage.getItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`);
      const refreshToken = localStorage.getItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`);

      this.authToken = accessToken;
      this.refreshToken = refreshToken;

      if (accessToken) {
        this.userAccountService.setUser(accessToken);
      }

      this.inactivityTimeLimit = config?.inactivityTimeLimit;
      this.refreshTokenInterval = config?.tokenRefreshBufferTime;
    } catch (error) {
      this.loadFromStorage();
    }
  }

  public loadFromStorage(): void {
    const settings = this.getLocalSettings();

    this.apiBaseUrl = settings.baseURI ?? 'http://localhost:5141/api/';
    this.useEncryption = settings.useEncryption ?? true;
    this.authToken = localStorage.getItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`);
    this.refreshToken = localStorage.getItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`);

    if (this.authToken) {
      this.userAccountService.setUser(this.authToken);
    }
  }

  setTokens(token: string, refreshToken: string): void {
    this.authToken = token;
    this.refreshToken = refreshToken;

    localStorage.setItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`, token);
    localStorage.setItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`, refreshToken);

    this.userAccountService.setUser(token);
    this.loggedIn$.next(true);
  }

  clearAll(): void {
    localStorage.removeItem("newLifeHRT:"+this.SETTINGS_KEY);
    localStorage.removeItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`);
    localStorage.removeItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`);
    this.authToken = null;
    this.refreshToken = null;

    this.userAccountService.clear();
    this.loggedIn$.next(false);
  }

  private getLocalSettings(): any {
    try {
      return JSON.parse(localStorage.getItem("newLifeHRT:"+this.SETTINGS_KEY) ?? '{}');
    } catch {
      return {};
    }
  }

  private applySettings(settings: any): void {
    this.apiBaseUrl = settings.baseURI;
    this.useEncryption = settings.useEncryption;
    this.tenantName =settings.tenantName
  }

  static get settings(): AppSettings {
    return AppConfig.settings;
  }

  isUserLoggedIn(): boolean {
    return !!this.userAccountService.getUserAccount();
  }

  isAdmin(): boolean {
    return this.userAccountService.getUserAccount()?.role === 'Admin';
  }

  getLocalStorage(key: string): string | null {
    return localStorage.getItem(key);
  }

  setLocalStorage(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  public get ValidAuthToken(): string {
    let encodedToken = atob(localStorage.getItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`) ?? '');
    return encodedToken;
  }

  public get ValidRefreshToken(): string {
    let encodedToken = atob(localStorage.getItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`) ?? '');
    return encodedToken;
  }

  getDecodedToken(): any | null {
  const token = this.getLocalStorage(`${this.tenantName}:access_token`);
  if (!token) return null;

  try {
    return jwtDecode(token);
  } catch {
    return null;
  }
}

getExpirationTime(): number | null {
  const token = this.getDecodedToken();
  if (!token || !token.exp) return null;

  return token.exp * 1000;
}

isTokenExpired(): boolean {
  const expiryTime = this.getExpirationTime();
  if (!expiryTime) return true;

  return Date.now() > expiryTime;
}


  getClaim(claim: string): any {
    const decoded = this.getDecodedToken();
    return decoded ? decoded[claim] : null;
  }

  getInactiveTimeLimit(){
    return this.inactivityTimeLimit;
  }

  getRefreshTokenBufferTime(){
    return this.refreshTokenInterval;
  }

  isLoggedIn$() {
    return this.loggedIn$.asObservable();
  }

  getAuthTokenFromStorage(){
    return localStorage.getItem(`${this.tenantName}:${this.ACCESS_TOKEN_KEY}`);
  }

  getRefreshTokenFromStorage(){
    return localStorage.getItem(`${this.tenantName}:${this.REFRESH_TOKEN_KEY}`);
  }

  getPatientUserId(): string {
    const decodedToken = this.getDecodedToken();
    return decodedToken?.isPatient as string;
  }

  isUserPatient():boolean{
    const patientId =  this.getPatientUserId();
    return !!patientId && patientId.trim() !== '';
  }

}
