import { Routes } from '@angular/router';
import { LoginPage } from './pages/login-page/login-page';
import { RegisterPage } from './pages/register-page/register-page';
import { ForgotPasswordPage } from './pages/forgot-password-page/forgot-password-page';
import { ResetPasswordPage } from './pages/reset-password-page/reset-password-page';

export const routes: Routes = [
  {
    title: 'Sign In',
    path: 'login',
    component: LoginPage,
  },
  {
    title: 'Sign Up',
    path: 'register',
    component: RegisterPage,
  },
  {
    title: 'Forgot Password',
    path: 'forgot-password',
    component: ForgotPasswordPage,
  },
  {
    title: 'Reset Password',
    path: 'reset-password',
    component: ResetPasswordPage,
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
