import { AppSettings } from './app-settings.model';

export class AppConfig {
  static settings: AppSettings = {
    apiBaseUrl: '',
    encryptionIV: '',
    encryptionKey: '',
    useEncryption: false,
    appTitle: ''
  };
}
