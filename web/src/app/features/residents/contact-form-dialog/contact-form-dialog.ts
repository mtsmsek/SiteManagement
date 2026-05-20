import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentsStore } from '../residents.store';
import { isProblemDetails } from '../../../core/http/problem-details';

/** Pre-fills the form with the resident's id + current contact values. */
export interface ContactFormDialogData {
  residentId: string;
  email: string;
  phone: string;
}

/** Edit-contact dialog. Replaces the resident's email + phone. */
@Component({
  selector: 'app-contact-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './contact-form-dialog.html',
})
export class ContactFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(ResidentsStore);
  private readonly dialogRef = inject(MatDialogRef<ContactFormDialog>);
  private readonly data = inject<ContactFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: [this.data.email, [Validators.required, Validators.email]],
    phone: [this.data.phone, [Validators.required]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    try {
      await this.store.updateContact(this.data.residentId, this.form.getRawValue());
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
