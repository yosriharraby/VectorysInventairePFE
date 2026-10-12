import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'monitoring',
    loadComponent: () =>
      import('./features/monitoring/agent-monitoring.component').then((m) => m.AgentMonitoringComponent),
    canActivate: [authGuard]
  },
  {
    path: 'ecarts/recomptages/:inventaireId',
    loadComponent: () =>
      import('./features/ecarts/recomptages-dashboard.component').then((m) => m.RecomptagesDashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'ecarts',
    loadComponent: () =>
      import('./features/ecarts/ecart-management.component').then((m) => m.EcartManagementComponent),
    canActivate: [authGuard]
  },
  {
    path: 'kpi',
    loadComponent: () =>
      import('./features/kpi-analytics/kpi-dashboard.component').then((m) => m.KpiDashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'inventaires/create',
    loadComponent: () =>
      import('./features/inventaires/create/create-inventaire.component').then(
        (m) => m.CreateInventaireComponent
      ),
    canActivate: [authGuard]
  },
  {
    path: 'settings',
    loadComponent: () =>
      import('./features/settings/settings.component').then((m) => m.SettingsComponent),
    canActivate: [authGuard]
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' }
];
