import { Routes } from '@angular/router';
import { adminGuard } from './core/auth/auth.guards';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./layouts/admin-layout/admin-layout').then((m) => m.AdminLayout),
    children: [
      { path: '', redirectTo: 'sites', pathMatch: 'full' },
      {
        path: 'sites',
        loadComponent: () => import('./features/sites/site-list/site-list').then((m) => m.SiteList),
      },
      {
        path: 'residents',
        loadComponent: () =>
          import('./features/residents/resident-list/resident-list').then((m) => m.ResidentList),
      },
    ],
  },
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { path: '**', redirectTo: 'admin' },
];
