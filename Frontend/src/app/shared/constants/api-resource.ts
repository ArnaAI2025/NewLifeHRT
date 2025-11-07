import { AppSettingsService } from '../services/app-settings.service';

export class ApiResource {
  /**
   * Constructs a full API URI using configured base URI and path segments.
   * 
   * @param base      Base path (e.g., 'users')
   * @param endPoint  Endpoint (e.g., 'get-data')
   * @param param     Optional route param (e.g., '123' → users/get-data/123)
   * @param query     Optional query object (e.g., { id: 123, active: true })
   */
  static getURI(base: string, endPoint: string, param?: string, query?: Record<string, any>): string {
    let apiEndPoint = `${base}/${endPoint}`;
    if (param) {
      apiEndPoint += `/${param}`;
    }

    // Query string
    let queryString = '';
    if (query && Object.keys(query).length > 0) {
      const params = new URLSearchParams();
      Object.entries(query).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          params.append(key, value.toString());
        }
      });
      queryString = `?${params.toString()}`;
    }

    // Base URI from local/session/config fallback
    const localSettings = localStorage.getItem('newLifeHRT:settings');
    const sessionSettings = sessionStorage.getItem('settingsUrls');

    let baseURI = '';
    if (localSettings) {
      baseURI = JSON.parse(localSettings)['baseURI'] ?? '';
    } else if (sessionSettings) {
      baseURI = JSON.parse(sessionSettings)['baseURI'] ?? '';
    } else if (AppSettingsService.settings) {
      baseURI = AppSettingsService.settings.apiBaseUrl ?? '';
    } else {
      const origin = location.origin;
      baseURI = origin.includes('localhost')
        ? 'http://localhost:5141/api/'
        : `${origin}/`;
    }

    return `${baseURI}${apiEndPoint}${queryString}`;
  }
}
