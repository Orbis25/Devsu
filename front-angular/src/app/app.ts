import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SharedModule } from './shared/shared-module';
import { UsersModule } from './users/users-module';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SharedModule,UsersModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('front-angular');
}
