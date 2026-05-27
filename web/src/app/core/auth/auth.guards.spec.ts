import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { residentGuard } from './auth.guards';
import { AuthService } from './auth.service';

describe('residentGuard', () => {
  const urlTree = {} as UrlTree;

  function setup(auth: Partial<AuthService>) {
    const router = { createUrlTree: vi.fn().mockReturnValue(urlTree) };
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: auth },
        { provide: Router, useValue: router },
      ],
    });
    return router;
  }

  const run = () =>
    TestBed.runInInjectionContext(() =>
      residentGuard({} as ActivatedRouteSnapshot, { url: '/resident/bills' } as RouterStateSnapshot),
    );

  it('allows a signed-in resident', () => {
    // arrange
    setup({ isAuthenticated: () => true, isResident: () => true, isAdmin: () => false, homeUrl: () => '/resident' } as Partial<AuthService>);

    // act + assert
    expect(run()).toBe(true);
  });

  it('redirects a signed-in non-resident to their own home', () => {
    // arrange
    const router = setup({ isAuthenticated: () => true, isResident: () => false, isAdmin: () => true, homeUrl: () => '/admin' } as Partial<AuthService>);

    // act
    const result = run();

    // assert
    expect(result).toBe(urlTree);
    expect(router.createUrlTree).toHaveBeenCalledWith(['/admin']);
  });

  it('redirects an anonymous user to login with the return url', () => {
    // arrange
    const router = setup({ isAuthenticated: () => false, isResident: () => false, isAdmin: () => false, homeUrl: () => '/admin' } as Partial<AuthService>);

    // act
    run();

    // assert
    expect(router.createUrlTree).toHaveBeenCalledWith(['/login'], { queryParams: { returnUrl: '/resident/bills' } });
  });
});
