import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
 import { ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { inject } from '@angular/core';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './verify-email.html',
  styles: [
    `
      @import '../../../shared/forms.css';
    `,
  ],
})
export class VerifyEmail implements OnInit {
  private fb = inject(NonNullableFormBuilder);
  email = '';
  message = '';
  error = '';
  loading = false;
  form = this.fb.group({
    code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
  });

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.queryParamMap.subscribe((q) => {
      this.email = q.get('email') ?? '';
    });
  }

  verify() {
    this.error = '';
    if (this.form.invalid) return;
    this.loading = true;
    const code = this.form.getRawValue().code;
    this.auth.verifyEmail(this.email, code).subscribe({
      next: () => {
        this.loading = false;
        this.message = 'Email verified. Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 1300);
      },
      error: (e) => {
        this.loading = false;
        this.error = e?.error ?? 'Invalid or expired code.';
      },
    });
  }

  resend() {
    this.auth.resendCode(this.email, 'verify').subscribe({
      next: () => (this.message = 'Verification code resent.'),
      error: (e) => (this.error = e?.error ?? 'Could not resend.'),
    });
  }
}
