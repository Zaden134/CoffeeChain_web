import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { BranchesPage } from './features/branches/branches-page';
import { DashboardPage } from './features/dashboard/dashboard-page';
import { DashboardStore } from './features/dashboard/dashboard.store';
import { EmployeesPage } from './features/employees/employees-page';
import { InventoryPage } from './features/inventory/inventory-page';
import { LoginPage } from './features/login/login-page';
import { ProductsPage } from './features/products/products-page';
import { PromotionsPage } from './features/promotions/promotions-page';
import { RecruitmentRequestsPage } from './features/recruitment-requests/recruitment-requests-page';
import { ReportsPage } from './features/reports/reports-page';
import { MainLayout } from './layouts/main-layout';

// routes chia app thanh login route va shell route da duoc auth guard bao ve.
export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    component: LoginPage
  },
  {
    path: '',
    canActivate: [authGuard],
    component: MainLayout,
    children: [
      {
        path: '',
        component: DashboardPage,
        providers: [DashboardStore]
      },
      {
        path: 'branches',
        component: BranchesPage
      },
      {
        path: 'products',
        component: ProductsPage
      },
      {
        path: 'inventory',
        component: InventoryPage
      },
      {
        path: 'employees',
        component: EmployeesPage
      },
      {
        path: 'reports',
        component: ReportsPage,
        providers: [DashboardStore]
      },
      {
        path: 'promotions',
        component: PromotionsPage
      },
      {
        path: 'recruitment-requests',
        component: RecruitmentRequestsPage
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
