import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { isProblemDetails } from '../../../core/http/problem-details';

/**
 * Create-site dialog. Reactive form for name + address; delegates the write to
 * SitesStore (which refreshes the list) and closes on success. Field-level 400
 * validation errors from the API are surfaced inline.
 */
@Component({
  selector: 'app-site-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './site-form-dialog.html',
})
export class SiteFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(SitesStore);
  private readonly dialogRef = inject(MatDialogRef<SiteFormDialog>);

  /** True while the create request is in flight. */
  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(120)]],
    address: ['', [Validators.required, Validators.maxLength(250)]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    try {
      await this.store.create(this.form.getRawValue());
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

  /** Maps API field validation errors onto the matching form controls. */
  private applyServerErrors(error: unknown): void {
    if (error instanceof HttpErrorResponse && isProblemDetails(error.error) && error.error.errors) {
      for (const [field, messages] of Object.entries(error.error.errors)) {
        const control = this.form.get(field.charAt(0).toLowerCase() + field.slice(1));
        control?.setErrors({ server: messages[0] });
      }
    }
  }
}
