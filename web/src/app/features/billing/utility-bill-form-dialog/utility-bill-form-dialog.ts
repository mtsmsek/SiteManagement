import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { BillingStore } from '../billing.store';
import { isProblemDetails } from '../../../core/http/problem-details';
import { UtilityType, UtilityTypeOptions } from '../../../core/api/api.models';

/** Identifies the site the new utility bill period belongs to. */
export interface UtilityBillFormDialogData {
  siteId: string;
}

/**
 * Opens a utility bill period for a month with an invoice total to split across
 * the occupied apartments. Delegates to BillingStore.openUtility; surfaces API
 * field errors inline.
 */
@Component({
  selector: 'app-utility-bill-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './utility-bill-form-dialog.html',
})
export class UtilityBillFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(BillingStore);
  private readonly dialogRef = inject(MatDialogRef<UtilityBillFormDialog, boolean>);
  private readonly data = inject<UtilityBillFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);
  readonly utilityTypeOptions = UtilityTypeOptions;

  private readonly now = new Date();

  readonly form = this.fb.nonNullable.group({
    year: [this.now.getFullYear(), [Validators.required]],
    month: [this.now.getMonth() + 1, [Validators.required, Validators.min(1), Validators.max(12)]],
    utilityType: [UtilityType.electricity as number, [Validators.required]],
    totalAmount: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();
    try {
      await this.store.openUtility(this.data.siteId, {
        siteId: this.data.siteId,
        year: raw.year,
        month: raw.month,
        utilityType: raw.utilityType,
        totalAmount: raw.totalAmount!,
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
