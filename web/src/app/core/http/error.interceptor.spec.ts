import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { lastValueFrom, throwError } from 'rxjs';
import { errorInterceptor } from './error.interceptor';
import { ErrorSnackbar } from './error-snackbar';

describe('errorInterceptor', () => {
  let snackBar: { openFromComponent: ReturnType<typeof vi.fn> };
  let translate: { instant: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    // arrange — stub the snackbar + translation so we can assert what gets shown
    snackBar = { openFromComponent: vi.fn() };
    translate = { instant: vi.fn((key: string) => key) };
    TestBed.configureTestingModule({
      providers: [
        { provide: MatSnackBar, useValue: snackBar },
        { provide: TranslateService, useValue: translate },
      ],
    });
  });

  /** Runs the interceptor for a request that fails with the given error. */
  function intercept(error: HttpErrorResponse): Promise<unknown> {
    const next: HttpHandlerFn = () => throwError(() => error);
    return TestBed.runInInjectionContext(() =>
      lastValueFrom(errorInterceptor(new HttpRequest('GET', '/api/x'), next)),
    );
  }

  it('shows the localized problem detail in a prominent error snackbar', async () => {
    // arrange
    const error = new HttpErrorResponse({ status: 402, error: { detail: 'Ödeme reddedildi.' } });

    // act
    await expect(intercept(error)).rejects.toBe(error);

    // assert
    expect(snackBar.openFromComponent).toHaveBeenCalledWith(
      ErrorSnackbar,
      expect.objectContaining({
        data: { message: 'Ödeme reddedildi.' },
        panelClass: 'error-snackbar',
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 8000,
      }),
    );
  });

  it('falls back to the network message when the server is unreachable', async () => {
    // arrange
    const error = new HttpErrorResponse({ status: 0 });

    // act
    await expect(intercept(error)).rejects.toBe(error);

    // assert
    expect(snackBar.openFromComponent).toHaveBeenCalledWith(
      ErrorSnackbar,
      expect.objectContaining({ data: { message: 'errors.network' } }),
    );
  });

  it('leaves validation 400s for the form to render inline', async () => {
    // arrange
    const error = new HttpErrorResponse({ status: 400, error: { errors: { name: ['required'] } } });

    // act
    await expect(intercept(error)).rejects.toBe(error);

    // assert
    expect(snackBar.openFromComponent).not.toHaveBeenCalled();
  });

  it('leaves 401s to the auth flow', async () => {
    // arrange
    const error = new HttpErrorResponse({ status: 401 });

    // act
    await expect(intercept(error)).rejects.toBe(error);

    // assert
    expect(snackBar.openFromComponent).not.toHaveBeenCalled();
  });
});
