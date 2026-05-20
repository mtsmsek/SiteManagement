import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { isProblemDetails } from '../../../core/http/problem-details';
import { ApartmentTypeOptions, APARTMENT_TYPE_PATTERN } from '../../../core/api/api.models';

/** Identifies the site + block the new apartment is added to. */
export interface ApartmentFormDialogData {
  siteId: string;
  blockId: string;
}

/**
 * Add-apartment dialog. Number + floor + type ("N+M" picklist). Delegates to
 * SitesStore.addApartment, surfacing API field errors inline.
 */
@Component({
  selector: 'app-apartment-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './apartment-form-dialog.html',
})
export class ApartmentFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(SitesStore);
  private readonly dialogRef = inject(MatDialogRef<ApartmentFormDialog>);
  private readonly data = inject<ApartmentFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);
  readonly typeOptions = ApartmentTypeOptions;

  readonly form = this.fb.nonNullable.group({
    number: [null as number | null, [Validators.required, Validators.min(1)]],
    floor: [null as number | null, [Validators.required]],
    type: ['2+1', [Validators.required, Validators.pattern(APARTMENT_TYPE_PATTERN)]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();
    try {
      await this.store.addApartment(this.data.siteId, this.data.blockId, {
        number: raw.number!,
        floor: raw.floor!,
        type: raw.type,
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
