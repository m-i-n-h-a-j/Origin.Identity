import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-register-page',
  imports: [FormsModule],
  templateUrl: './register-page.html',
  styleUrl: './register-page.css',
})
export class RegisterPage {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  firstName = '';
  lastName = '';
  userName = '';
  email = '';
  password = '';

  isLoading = signal(false);
  errorMessage = '';

  switchToSignIn() {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/connect/authorize';

    this.router.navigate(['/login'], {
      queryParams: {
        returnUrl,
      },
    });
  }

  login(): void {
    if (this.isLoading()) return;

    this.isLoading.set(true);
    this.errorMessage = '';

    this.http
      .post('/api/auth/register', {
        firstName: this.firstName,
        lastName: this.lastName,
        userName: this.userName,
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: () => {
          const returnUrl =
            this.route.snapshot.queryParamMap.get('returnUrl') ?? '/connect/authorize';

          this.router.navigate(['/login'], {
            queryParams: {
              returnUrl,
              registered: 'true',
            },
          });
        },
        error: (error) => {
          this.isLoading.set(false);

          if (error.status === 400) {
            this.errorMessage = error.error;
            return;
          }

          this.errorMessage = 'Something went wrong while signing in. Please try again.';
        },
      });
  }
}
