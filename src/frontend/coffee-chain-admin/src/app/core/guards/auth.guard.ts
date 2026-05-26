import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthStore } from '../services/auth.store';

// authGuard chan route noi bo neu user chua dang nhap.
export const authGuard: CanActivateFn = async () => {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (authStore.isAuthenticated()) {
    return true;
  }

  const profile = await authStore.hydrateProfile();
  return profile ? true : router.createUrlTree(['/login']);
};
