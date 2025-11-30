import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="form-card">
      <h2>Dashboard</h2>
      <p>Welcome! You are signed in.</p>
      <div style="margin-top:12px">
        <button (click)="logout()">Logout</button>
      </div>
    </section>
  `,
  styles: [
    `
      @import '../../shared/forms.css';
    `,
  ],
})
export class Dashboard {
  constructor(private auth: AuthService, private router: Router) {}
  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
