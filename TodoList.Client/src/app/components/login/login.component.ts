import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { UserLogin } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
      CommonModule,
      ReactiveFormsModule,
      RouterLink
      ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loginError: string | null = null;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    this.loginError = null;
    this.loginForm.markAllAsTouched();

    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    const loginData: UserLogin = this.loginForm.value;

    this.authService.login(loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.succeeded && response.token) {
          console.log('Login successful, token:', response.token);
          this.router.navigate(['/tasks']);
        } else {
          this.loginError = response.errors?.join(', ') || 'Login failed. Please check credentials.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Login HTTP error:', err);
        this.loginError = 'An error occurred during login. Please try again.';
      }
    });
  }
}