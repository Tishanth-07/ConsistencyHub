import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styles: [
    `
      @import '../../../shared/forms.css';
    `,
  ],
})
export class ResetPassword implements OnInit {
  private fb = inject(FormBuilder);
  form = this.fb.group({
    code: ['', [Validators.required]],
    newPassword: [
      '',
      [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{6,}$/)],
    ],
  });

  email = '';
  message = '';
  error = '';

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.queryParamMap.subscribe((q) => (this.email = q.get('email') ?? ''));
  }

  submit() {
    if (this.form.invalid) return;
    const code = this.form.get('code')?.value ?? '';
    const newPassword = this.form.get('newPassword')?.value ?? '';
    this.auth.resetPassword(this.email ?? '', code, newPassword).subscribe({
      next: () => {
        this.message = 'Password reset successful. Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 1400);
      },
      error: (e) => (this.error = e?.error ?? 'Reset failed'),
    });
  }
}
