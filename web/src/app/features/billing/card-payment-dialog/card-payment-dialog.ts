import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import type { PayByCardRequest } from '../../../core/api/api.models';

/**
 * Shows the amount being charged; the dialog only collects card details. The
 * amount may arrive as number or string (OpenAPI maps decimal to either), so
 * it's typed loosely and coerced for display.
 */
export interface CardPaymentDialogData {
  amount: number | string;
}

/**
 * Collects card details for paying a billing item. The dialog is payment-kind
 * agnostic — it just gathers a valid card and closes with a
 * {@link PayByCardRequest}; the caller decides which store method to invoke
 * (dues vs utility). Card shape is checked here for fast feedback; the real
 * validity (Luhn, balance, expiry) is decided by the payment gateway.
 */
@Component({
  selector: 'app-card-payment-dialog',
  imports: [
    ReactiveFormsModule,
    DecimalPipe,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe,
  ],
  templateUrl: './card-payment-dialog.html',
})
export class CardPaymentDialog {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<CardPaymentDialog, PayByCardRequest>);

  /** The amount being charged, shown read-only in the header. */
  readonly data = inject<CardPaymentDialogData>(MAT_DIALOG_DATA);

  /** The charge amount coerced to a number for display formatting. */
  readonly amount = Number(this.data.amount);

  /** Submit guard (the actual charge happens in the caller). */
  readonly submitting = signal(false);

  /** Year options for the expiry picker (current year + next 10). */
  readonly years = Array.from({ length: 11 }, (_, i) => new Date().getFullYear() + i);

  /** Month options 1..12. */
  readonly months = Array.from({ length: 12 }, (_, i) => i + 1);

  readonly form = this.fb.nonNullable.group({
    cardNumber: ['', [Validators.required, Validators.pattern(/^\d{13,19}$/)]],
    cvv: ['', [Validators.required, Validators.pattern(/^\d{3,4}$/)]],
    expiryYear: [this.years[0], [Validators.required]],
    expiryMonth: [this.months[0], [Validators.required]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.dialogRef.close({
      cardNumber: raw.cardNumber,
      cvv: raw.cvv,
      expiryYear: raw.expiryYear,
      expiryMonth: raw.expiryMonth,
    });
  }

  cancel(): void {
    this.dialogRef.close(undefined);
  }
}
