import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: `
    <nav>
      <a routerLink="/incidents">All Incidents</a>
      <a routerLink="/incidents/new">Report Incident</a>
      <span *ngIf="authService.currentUser as user">
        Logged in as {{ user.name }} ({{ user.role }})
        <button (click)="logout()">Log Out</button>
      </span>
      <a *ngIf="!authService.currentUser" routerLink="/login">Log In</a>
    </nav>
    <!-- router-outlet is where the matched route's component renders -->
    <router-outlet></router-outlet>
  `
})
export class AppComponent {
  constructor(public authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}