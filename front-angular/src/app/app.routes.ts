import { Routes } from '@angular/router';
import { Users } from './users/users';
import { Transactions } from './transactions/transactions';
import { Accounts } from './accounts/accounts';
import { Reports } from './reports/reports';

export const routes: Routes = [
  {
    path: 'clients',
    component: Users,
  },
  {
    path: 'movements',
    component: Transactions,
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
