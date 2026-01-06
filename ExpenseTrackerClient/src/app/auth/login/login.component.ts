import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';

import { inject } from '@angular/core';
import { SignalRService } from '../../core/services/signalr.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [AuthService],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(
    private authService: AuthService,
    private signalRService: SignalRService,
  private router: Router) {}

  login() {
  this.authService.login(this.email, this.password)
    .subscribe({
      next: (res) => {
        console.log('Login success response:', res);
        this.signalRService.startConnection(res.token);
        this.router.navigate(['/dashboard']); // navigate after login
      },
      error: (err) => {
        console.error('Login error:', err);
        this.error = err?.error?.message ?? 'Login failed';
      }
    });
  }

}
