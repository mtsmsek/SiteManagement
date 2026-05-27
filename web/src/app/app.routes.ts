import { Routes } from '@angular/router';
import { adminGuard, residentGuard } from './core/auth/auth.guards';

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
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/reports/admin-dashboard/admin-dashboard').then((m) => m.AdminDashboard),
      },
      {
        path: 'sites',
        loadComponent: () => import('./features/sites/site-list/site-list').then((m) => m.SiteList),
      },
      {
        path: 'sites/:siteId',
        loadComponent: () =>
          import('./features/sites/site-detail/site-detail').then((m) => m.SiteDetail),
      },
      {
        path: 'residents',
        loadComponent: () =>
          import('./features/residents/resident-list/resident-list').then((m) => m.ResidentList),
      },
      {
        path: 'residents/:residentId',
        loadComponent: () =>
          import('./features/residents/resident-detail/resident-detail').then((m) => m.ResidentDetail),
      },
    ],
  },
  {
    path: 'resident',
    canActivate: [residentGuard],
    loadComponent: () =>
      import('./layouts/resident-layout/resident-layout').then((m) => m.ResidentLayout),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/resident/dashboard/resident-dashboard').then((m) => m.ResidentDashboard),
      },
      {
        path: 'bills',
        loadComponent: () => import('./features/resident/my-bills/my-bills').then((m) => m.MyBills),
      },
      {
        path: 'messages',
        loadComponent: () =>
          import('./features/resident/my-messages/my-messages').then((m) => m.MyMessages),
      },
    ],
  },
  { path: '', redirectTo: 'admin', pathMatch: 'full' },
  { path: '**', redirectTo: 'admin' },
];
