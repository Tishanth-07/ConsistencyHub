import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { GoogleSigninComponent } from '../../../components/google-signin/google-signin';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GoogleSigninComponent],
  templateUrl: './login.html',
  styles: [
    `
      @import '../../../shared/forms.css';
      .links {
        margin-top: 10px;
      }
    `,
  ],
})
export class Login {
  private fb = inject(FormBuilder);
  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  loading = false;
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.error = '';
    if (this.form.invalid) return;
    this.loading = true;
    const { email, password } = this.form.value;
    const payload = { email: email ?? '', password: password ?? '' };
    this.auth.login(payload).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (e) => {
        this.loading = false;
        this.error = e?.error ?? 'Login failed';
      },
    });
  }
}
