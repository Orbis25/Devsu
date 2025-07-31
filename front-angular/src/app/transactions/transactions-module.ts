import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TransactionsComponent } from './transactions.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared-module';



@NgModule({
  declarations: [
    TransactionsComponent
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SharedModule],

 exports:[TransactionsComponent]
})
export class TransactionsModule { }
