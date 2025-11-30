import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forgot-password.html',
  styles: [
    `
      @import '../../../shared/forms.css';
    `,
  ],
})
export class ForgotPassword {
  private fb = inject(FormBuilder);

  form = this.fb.group({ email: ['', [Validators.required, Validators.email]] });
  message = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    if (this.form.invalid) return;
    const email = this.form.value.email!;
    this.auth.forgotPassword(email).subscribe({
      next: () => {
        this.message = 'Reset code sent. Check your email.';
        this.router.navigate(['/reset-password'], { queryParams: { email } });
      },
      error: (e) => (this.error = e?.error ?? 'Error sending reset code.'),
    });
  }
}
