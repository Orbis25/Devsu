import { Routes } from '@angular/router';
import { Users } from './users/users';
import { TransactionsComponent } from './transactions/transactions.component';
import { Accounts } from './accounts/accounts';
import { Reports } from './reports/reports';

export const routes: Routes = [
  {
    path: 'clients',
    component: Users,
  },
  {
    path: 'movements',
    component: TransactionsComponent,
  },
  {
    path: 'accounts',
    component: Accounts,
  },
  {
    path: 'reports',
    component: Reports,
  },
];
