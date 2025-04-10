import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { UserRegister } from '../../models/auth.models';

function passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  if (password && confirmPassword && password.value !== confirmPassword.value) {
    return { passwordsMismatch: true };
  }
  return null;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  registerError: string | null = null;
  registerSuccess = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required, Validators.maxLength(256)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: passwordsMatchValidator });
  }

  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }

  onSubmit(): void {
    this.registerError = null;
    this.registerSuccess = false;
    this.registerForm.markAllAsTouched();

    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    const registerData: UserRegister = {
      userName: this.registerForm.value.userName,
      password: this.registerForm.value.password,
      confirmPassword: this.registerForm.value.confirmPassword
    };

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.succeeded) {
          this.registerSuccess = true;
          console.log('Registration successful');
        } else {
          this.registerError = response.errors?.join(', ') || 'Registration failed.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Register HTTP error:', err);
        this.registerError = 'An error occurred during registration. Please try again.';
      }
    });
  }
}