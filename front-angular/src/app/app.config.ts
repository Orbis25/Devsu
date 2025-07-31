import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { SharedModule } from './shared/shared-module';
import { UsersModule } from './users/users-module';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { globalInterceptor } from './interceptors/global-interceptor';
import { TransactionsModule } from './transactions/transactions-module';
import { AccountsModule } from './accounts/accounts-module';
import { ReportsModule } from './reports/reports-module';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    importProvidersFrom([
      SharedModule,
      UsersModule,
      TransactionsModule,
      AccountsModule,
      ReportsModule,
    ]),
    provideHttpClient(withInterceptors([globalInterceptor])),
    provideHttpClient(withInterceptorsFromDi()),
  ],
};
