import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Users } from './users';
import { SharedModule } from '../shared/shared-module';

@NgModule({
  declarations: [Users],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [Users]
})
export class UsersModule { }
