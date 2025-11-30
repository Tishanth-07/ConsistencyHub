import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth';
import { GoogleSigninComponent } from '../../../components/google-signin/google-signin';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, GoogleSigninComponent],
  templateUrl: './register.html',
  styles: [
    `
      @import '../../../shared/forms.css';
      .title {
        margin-bottom: 10px;
      }
    `,
  ],
})
export class Register {
  private fb = inject(FormBuilder);  
  form = this.fb.nonNullable.group({
    firstname: ['', Validators.required],
    lastname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{6,}$/)],
    ],
  });

  loading = false;
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.error = '';
    if (this.form.invalid) {
      this.error = 'Please fill all fields correctly.';
      return;
    }
    this.loading = true;
    
    const payload = this.form.getRawValue();
    this.auth.register(payload).subscribe({
      next: () => {
        this.loading = false;
        const email = payload.email;
        this.router.navigate(['/verify-email'], { queryParams: { email } });
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error ? JSON.stringify(err.error) : 'Registration failed';
      },
    });
  }
}
