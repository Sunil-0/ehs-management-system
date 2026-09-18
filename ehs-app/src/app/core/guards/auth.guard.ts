import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

// authGuard: just requires being logged in at all (any role)
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.currentUser) return true;

  router.navigate(['/login']);
  return false;
};

// roleGuard(role): a guard FACTORY -- returns a guard function configured
// for a specific role, so we can write canActivate: [roleGuard('Manager')]
// in the routes. This mirrors [Authorize(Roles = "Manager")] on the backend --
// but remember, this is a UX convenience only. The backend is the real security
// boundary; a user could bypass this guard by calling the API directly, which
// is exactly why the [Authorize(Roles=...)] checks on the controller still matter.
export function roleGuard(role: string): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.hasRole(role)) return true;

    router.navigate(['/incidents']);
    return false;
  };
}