import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/authentication/register/register').then((m) => m.Register),
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./pages/authentication/verify-email/verify-email').then(
        (m) => m.VerifyEmail
      ),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/authentication/login/login').then((m) => m.Login),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/authentication/forgot-password/forgot-password').then(
        (m) => m.ForgotPassword
      ),
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./pages/authentication/reset-password/reset-password').then(
        (m) => m.ResetPassword
      ),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard/dashboard').then((m) => m.Dashboard),
    canMatch: [() => import('./guards/auth.guard').then((m) => m.authGuard)],
  },
];
