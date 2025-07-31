import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header';
import { SidebarComponent } from './sidebar/sidebar';
import { Modal } from './modal/modal';
import { RouterModule } from '@angular/router';
import { TableComponent } from './table/table';


@NgModule({
  declarations: [Modal, HeaderComponent, SidebarComponent, TableComponent],
  imports: [CommonModule, RouterModule],
  exports: [Modal, HeaderComponent, SidebarComponent, TableComponent],
})
export class SharedModule {}
