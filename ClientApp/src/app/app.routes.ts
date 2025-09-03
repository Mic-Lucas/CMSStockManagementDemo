import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Dashboard } from './pages/stock/dashboard';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'login', component: Login },
  { path: 'register', component: Login },
  { path: 'dashboard', component: Dashboard }
];
