import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reports } from './reports';
import { TransactionsModule } from '../transactions/transactions-module';



@NgModule({
  declarations: [
    Reports
  ],
  imports: [
    CommonModule, TransactionsModule
  ],
  exports:[Reports]
})
export class ReportsModule { }
