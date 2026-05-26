import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';

/** Payload handed to the {@link ErrorSnackbar} by the HTTP error interceptor. */
export interface ErrorSnackbarData {
  /** The localized, human-readable error message to display. */
  message: string;
}

/**
 * Snackbar shown for failed actions. Unlike the plain text snackbar, it leads
 * with a warning icon and (via the `error-snackbar` panel class) is painted in
 * the theme's error colour with an attention-drawing entry animation, so a
 * declined payment or server error is impossible to miss. Opened through
 * <c>MatSnackBar.openFromComponent</c> from the error interceptor; the close
 * action dismisses it before the auto-hide timeout.
 */
@Component({
  selector: 'app-error-snackbar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatIconModule, TranslatePipe],
  template: `
    <mat-icon class="error-snackbar__icon" aria-hidden="true">error</mat-icon>
    <span class="error-snackbar__message">{{ data.message }}</span>
    <button type="button" class="error-snackbar__action" (click)="dismiss()">
      {{ 'common.close' | translate }}
    </button>
  `,
  styleUrl: './error-snackbar.scss',
})
export class ErrorSnackbar {
  /** The localized error message handed in by the interceptor. */
  readonly data = inject<ErrorSnackbarData>(MAT_SNACK_BAR_DATA);

  private readonly snackBarRef = inject(MatSnackBarRef<ErrorSnackbar>);

  /** Dismisses the snackbar in response to the close action. */
  dismiss(): void {
    this.snackBarRef.dismiss();
  }
}
