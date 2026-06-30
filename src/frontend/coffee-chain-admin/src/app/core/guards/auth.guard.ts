import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router } from '@angular/router';

import { AuthStore } from '../services/auth.store';

// authGuard chan route noi bo neu user chua dang nhap.
export const authGuard: CanActivateFn = async (route: ActivatedRouteSnapshot) => {
  return authorizeRoute(route);
};

// authChildGuard kiem tra role tren tung route con trong shell.
export const authChildGuard: CanActivateChildFn = async (route: ActivatedRouteSnapshot) => {
  return authorizeRoute(route);
};

async function authorizeRoute(route: ActivatedRouteSnapshot) {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (authStore.isAuthenticated()) {
    return authorizeRole(route, authStore, router);
  }

  const profile = await authStore.hydrateProfile();
  if (!profile) {
    return router.createUrlTree(['/login']);
  }

  return authorizeRole(route, authStore, router);
}

function authorizeRole(route: ActivatedRouteSnapshot, authStore: AuthStore, router: Router) {
  const requiredRoles = route.data['roles'] as string[];
  if (requiredRoles && requiredRoles.length > 0) {
    const userRole = authStore.role();
    if (!userRole || !requiredRoles.includes(userRole)) {
      return router.createUrlTree(['/']);
    }
  }

  return true;
}
