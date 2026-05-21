import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { BillingStore } from '../billing.store';
import { isProblemDetails } from '../../../core/http/problem-details';

/** Identifies the site the new dues period belongs to. */
export interface DuesPeriodFormDialogData {
  siteId: string;
}

/**
 * Opens a dues period for a month at a fixed per-apartment amount. Delegates to
 * BillingStore.openDues; surfaces API field errors inline.
 */
@Component({
  selector: 'app-dues-period-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './dues-period-form-dialog.html',
})
export class DuesPeriodFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(BillingStore);
  private readonly dialogRef = inject(MatDialogRef<DuesPeriodFormDialog, boolean>);
  private readonly data = inject<DuesPeriodFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);

  private readonly now = new Date();

  readonly form = this.fb.nonNullable.group({
    year: [this.now.getFullYear(), [Validators.required]],
    month: [this.now.getMonth() + 1, [Validators.required, Validators.min(1), Validators.max(12)]],
    perApartmentAmount: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();
    try {
      await this.store.openDues(this.data.siteId, {
        siteId: this.data.siteId,
        year: raw.year,
        month: raw.month,
        perApartmentAmount: raw.perApartmentAmount!,
      });
      this.dialogRef.close(true);
    } catch (error) {
      this.applyServerErrors(error);
    } finally {
      this.submitting.set(false);
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  private applyServerErrors(error: unknown): void {
    if (error instanceof HttpErrorResponse && isProblemDetails(error.error) && error.error.errors) {
      for (const [field, messages] of Object.entries(error.error.errors)) {
        const control = this.form.get(field.charAt(0).toLowerCase() + field.slice(1));
        control?.setErrors({ server: messages[0] });
      }
    }
  }
}
