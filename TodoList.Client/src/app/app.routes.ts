import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { TaskListComponent } from './components/task-list/task-list.component';

export const routes: Routes = [
  { path: 'tasks', component: TaskListComponent },

  { path: 'login', component: LoginComponent },

  { path: 'register', component: RegisterComponent },

  { path: '', redirectTo: '/tasks', pathMatch: 'full' },

];