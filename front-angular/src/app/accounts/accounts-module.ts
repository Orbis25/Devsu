import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Accounts } from './accounts';
import { SharedModule } from '../shared/shared-module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [Accounts],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SharedModule],
  exports: [Accounts],
})
export class AccountsModule {}
