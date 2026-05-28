import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';

/**
 * Consistent empty-state block used across every list/grid/table screen.
 * Centralises the icon + label visual + spacing so feature pages don't drift
 * into bespoke "no data" markup. Accepts an i18n key for the message so the
 * caller's translation file stays the source of truth.
 */
@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIconModule, TranslatePipe],
  template: `
    <div class="empty">
      <mat-icon class="empty-icon" aria-hidden="true">{{ icon() }}</mat-icon>
      <p class="empty-message">{{ messageKey() | translate }}</p>
    </div>
  `,
  styles: `
    .empty {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      padding: 2rem 1rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .empty-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      opacity: 0.5;
    }

    .empty-message {
      margin: 0;
      font-size: 0.9rem;
      text-align: center;
    }
  `,
})
export class EmptyState {
  /** Material icon ligature shown above the message (e.g. "people", "forum"). */
  readonly icon = input<string>('inbox');

  /** i18n key for the message body (e.g. "residents.empty"). */
  readonly messageKey = input.required<string>();
}
