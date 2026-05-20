import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentsStore } from '../residents.store';
import { isProblemDetails } from '../../../core/http/problem-details';

/**
 * Register-resident dialog. Captures identity + contact; the server generates
 * and emails a temporary password and links an AppUser. TcNo is checked here
 * only for the 11-digit shape — the real Turkish checksum is validated
 * server-side and surfaced inline if it fails.
 */
@Component({
  selector: 'app-resident-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe,
  ],
  templateUrl: './resident-form-dialog.html',
})
export class ResidentFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(ResidentsStore);
  private readonly dialogRef = inject(MatDialogRef<ResidentFormDialog>);

  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    tcNo: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
    firstName: ['', [Validators.required, Validators.maxLength(60)]],
    lastName: ['', [Validators.required, Validators.maxLength(60)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    try {
      await this.store.register(this.form.getRawValue());
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
