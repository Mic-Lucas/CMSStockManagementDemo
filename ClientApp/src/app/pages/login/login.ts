import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { AuthService, LoginDto, RegisterDto, LoginResponse } from '../../services/auth.service';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatInputModule,
    MatButtonModule,
    RouterModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class Login implements OnInit {
  loginForm: FormGroup;
  registerForm: FormGroup;
  selectedTab = 0;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      role: ['User'] // optional default role
    });
  }

  ngOnInit() {
    const path = this.route.snapshot.routeConfig?.path;
    this.selectedTab = path === 'register' ? 1 : 0;
  }

  onLogin() {
    if (!this.loginForm.valid) return;

    const payload: LoginDto = this.loginForm.value;

    this.authService.login(payload).subscribe({
      next: (res: LoginResponse) => {
        localStorage.setItem('token', res.token);
        this.snackBar.open('Login successful!', '', { duration: 1500 });
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open('Invalid username or password', '', { duration: 2000 });
      }
    });
  }

  onRegister() {
    if (!this.registerForm.valid) return;

    const payload: RegisterDto = this.registerForm.value;

    this.authService.register(payload).subscribe({
      next: (res: LoginResponse) => {
        localStorage.setItem('token', res.token);
        this.snackBar.open('Registration successful!', '', { duration: 1500 });
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Registration failed', '', { duration: 2000 });
      }
    });
  }

  tabChanged(index: number) {
    this.selectedTab = index;
  }
}
