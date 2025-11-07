// src/app/shared/models/app-settings.model.ts
export interface AppSettings {
  useEncryption: boolean;
  encryptionKey: string;
  encryptionIV: string;
  appTitle: string;
  apiBaseUrl: string;
  token?: string;
  user?: any;
  tenantName?:any;
  inactivityTimeLimit?:any;
  tokenRefreshBufferTime?: any;
}
