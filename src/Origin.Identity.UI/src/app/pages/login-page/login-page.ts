import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-page',
  imports: [FormsModule, RouterLink],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  userName = '';
  password = '';

  isLoading = signal(false);
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    if (this.route.snapshot.queryParamMap.get('registered') === 'true') {
      this.successMessage = 'Account created successfully. Please sign in.';
    }
  }

  switchToSignUp() {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/connect/authorize';

    this.router.navigate(['/register'], {
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
      .post('/api/auth/login', {
        userName: this.userName,
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
