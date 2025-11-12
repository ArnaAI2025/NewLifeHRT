// src/app/app.config.ts
import {
  ApplicationConfig,
  APP_INITIALIZER,
  provideZonelessChangeDetection,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { routes } from './app.routes';
import { encryptionInterceptor } from './shared/interceptors/encryption.interceptor';
import { AppSettingsService } from './shared/services/app-settings.service';
import { importProvidersFrom } from '@angular/core';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { provideAnimations } from '@angular/platform-browser/animations';

export function initializeApp(settings: AppSettingsService): () => Promise<void> {
  return () => settings.load();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes,withInMemoryScrolling({
      scrollPositionRestoration: 'enabled',
      anchorScrolling: 'enabled'
    })),
    provideAnimations(),
    provideHttpClient(
      withInterceptors([
        encryptionInterceptor
      ])
    ),

    importProvidersFrom(
  CalendarModule.forRoot({
    provide: DateAdapter,
    useFactory: adapterFactory
  })
),

    AppSettingsService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AppSettingsService],
      multi: true
    }
  ]
};
