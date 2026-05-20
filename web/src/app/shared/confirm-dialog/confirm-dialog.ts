import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';

/** Translation keys for the confirm dialog's text. */
export interface ConfirmDialogData {
  titleKey: string;
  messageKey: string;
  confirmKey?: string;
}

/**
 * Generic yes/no confirmation dialog. Reused for destructive actions (delete
 * site, remove vehicle, …). Resolves to true when confirmed, false otherwise.
 */
@Component({
  selector: 'app-confirm-dialog',
  imports: [MatDialogModule, MatButtonModule, TranslatePipe],
  template: `
    <h2 mat-dialog-title>{{ data.titleKey | translate }}</h2>
    <mat-dialog-content>{{ data.messageKey | translate }}</mat-dialog-content>
    <mat-dialog-actions align="end">
      <button matButton (click)="dialogRef.close(false)">{{ 'common.cancel' | translate }}</button>
      <button matButton="filled" color="warn" (click)="dialogRef.close(true)">
        {{ (data.confirmKey ?? 'common.delete') | translate }}
      </button>
    </mat-dialog-actions>
  `,
})
export class ConfirmDialog {
  readonly dialogRef = inject(MatDialogRef<ConfirmDialog, boolean>);
  readonly data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
}
