import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-reset-password-page',
  imports: [FormsModule],
  templateUrl: './reset-password-page.html',
  styleUrl: './reset-password-page.css',
})
export class ResetPasswordPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);

  userId = signal('');
  token = signal('');
  newPassword = '';

  isLoading = signal(false);
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.userId.set(this.route.snapshot.queryParamMap.get('userId') ?? '');
    this.token.set(this.route.snapshot.queryParamMap.get('token') ?? '');

    if (this.userId() === '' || this.token() === '') {
      this.errorMessage = 'Invalid or expired password reset link.';
      return;
    }
  }

  reset(): void {
    if (this.isLoading()) return;
    if (this.userId() === '' || this.token() === '') {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage = '';

    this.http
      .post('/api/auth/reset', {
        userId: this.userId(),
        token: this.token(),
        newPassword: this.newPassword,
      })
      .subscribe({
        next: () => {
          window.location.href = 'http://localhost:4200';
        },
        error: (error) => {
          this.isLoading.set(false);

          if (error.error?.error) {
            this.errorMessage = error.error?.error;
            return;
          }

          this.errorMessage = 'Something went wrong while resetting in. Please try again.';
        },
      });
  }
}
