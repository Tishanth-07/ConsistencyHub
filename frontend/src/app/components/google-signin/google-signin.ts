import { Component, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';
import { environment } from '../../../environments/environment';

declare const google: any;

@Component({
  selector: 'app-google-signin',
  standalone: true,
  imports: [CommonModule],
  template: `<div #buttonDiv></div>`,
  styles: [
    `
      :host {
        display: block;
      }
    `,
  ],
})
export class GoogleSigninComponent implements AfterViewInit {
  constructor(private el: ElementRef, private auth: AuthService) {}

  ngAfterViewInit() {
    this.loadScript()
      .then(() => this.initButton())
      .catch(console.error);
  }

  private loadScript(): Promise<void> {
    return new Promise((resolve) => {
      if (document.getElementById('google-id-script')) {
        resolve();
        return;
      }
      const s = document.createElement('script');
      s.src = 'https://accounts.google.com/gsi/client';
      s.id = 'google-id-script';
      s.async = true;
      s.defer = true;
      s.onload = () => resolve();
      document.head.appendChild(s);
    });
  }

  private initButton() {
    const clientId = environment.googleClientId;
    if (!clientId) {
      console.error('Google client id not configured');
      return;
    }

    google.accounts.id.initialize({
      client_id: clientId,
      callback: (resp: any) => {
        const idToken = resp?.credential;
        if (!idToken) return console.error('No idToken returned');
        this.auth.googleLogin(idToken).subscribe({
          next: () => (window.location.href = '/dashboard'),
          error: (e) => console.error('Google login failed', e),
        });
      },
    });

    const container = this.el.nativeElement.querySelector('#buttonDiv');
    google.accounts.id.renderButton(container, { theme: 'outline', size: 'large' });
  }
}
