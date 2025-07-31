import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: false,
  templateUrl: './modal.html',
  styleUrl: './modal.css'
})
export class Modal {
  @Input() title: string = '';
  @Input() description: string = '';
  @Input() open: boolean = false;
  @Output() onClose = new EventEmitter<void>();

  close(): void {
    this.onClose.emit();
  }
}
