import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password-page',
  imports: [FormsModule],
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.css',
})
export class ForgotPasswordPage {
  private readonly http = inject(HttpClient);

  email = '';
  isLoading = signal(false);
  successMessage = '';
  errorMessage = '';

  sendMail(): void {
    if (this.isLoading()) return;

    this.isLoading.set(true);

    this.http
      .post<{ message: string }>('/api/auth/forgot', {
        email: this.email,
      })
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          this.successMessage = res.message;
        },
        error: () => {
          this.isLoading.set(false);

          this.errorMessage = 'Something went wrong while sending in. Please try again.';
        },
      });
  }
}
