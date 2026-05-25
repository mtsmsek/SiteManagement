import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';

/**
 * Inputs for the amount-edit dialog. The caller supplies the i18n key for the
 * field label (e.g. dues "per apartment" vs utility "total") and the current
 * value so the field opens pre-filled; the dialog is otherwise billing-kind
 * agnostic.
 */
export interface AmountEditDialogData {
  titleKey: string;
  labelKey: string;
  hintKey: string;
  currentAmount: number | string;
}

/**
 * Collects a single corrected amount for a billing period. Closes with the new
 * number, or undefined when cancelled. Surface validation (required, positive)
 * lives here; the domain re-validates and re-rates the items server-side.
 */
@Component({
  selector: 'app-amount-edit-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './amount-edit-dialog.html',
})
export class AmountEditDialog {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<AmountEditDialog, number>);

  /** Title/label/hint keys + the current amount to pre-fill. */
  readonly data = inject<AmountEditDialogData>(MAT_DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({
    amount: [Number(this.data.currentAmount), [Validators.required, Validators.min(0.01)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue().amount);
  }

  cancel(): void {
    this.dialogRef.close(undefined);
  }
}
