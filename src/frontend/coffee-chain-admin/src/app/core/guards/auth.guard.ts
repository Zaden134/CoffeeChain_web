import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';

import { AuthStore } from '../services/auth.store';

// authGuard chan route noi bo neu user chua dang nhap.
export const authGuard: CanActivateFn = async (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (authStore.isAuthenticated()) {
    return true;
  }

  const profile = await authStore.hydrateProfile();
  if (!profile) {
    return router.createUrlTree(['/login']);
  }

  // Check roles in route data
  const requiredRoles = route.data['roles'] as string[];
  if (requiredRoles && requiredRoles.length > 0) {
    const userRole = authStore.role();
    if (!userRole || !requiredRoles.includes(userRole)) {
      return router.createUrlTree(['/']); // Redirect to dashboard if not authorized
    }
  }

  return true;
};
