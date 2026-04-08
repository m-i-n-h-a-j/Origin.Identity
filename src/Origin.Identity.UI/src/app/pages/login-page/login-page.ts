import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-page',
  imports: [FormsModule, RouterLink],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  email = '';
  password = '';

  isLoading = signal(false);
  errorMessage = '';

  login(): void {
    if (this.isLoading()) return;

    this.isLoading.set(true);
    this.errorMessage = '';

    this.http
      .post('/api/auth/login', {
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: () => {
          const returnUrl =
            this.route.snapshot.queryParamMap.get('returnUrl') ?? '/connect/authorize';

          window.location.href = returnUrl;
        },
        error: (error) => {
          this.isLoading.set(false);

          if (error.status === 401) {
            this.errorMessage = 'Invalid email or password.';
            return;
          }

          this.errorMessage = 'Something went wrong while signing in. Please try again.';
        },
      });
  }
}
