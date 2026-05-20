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

/** Identifies the resident the vehicle is registered on. */
export interface VehicleFormDialogData {
  residentId: string;
}

/** Add-vehicle dialog. Plate (required) + optional note. */
@Component({
  selector: 'app-vehicle-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './vehicle-form-dialog.html',
})
export class VehicleFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(ResidentsStore);
  private readonly dialogRef = inject(MatDialogRef<VehicleFormDialog>);
  private readonly data = inject<VehicleFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    plate: ['', [Validators.required, Validators.maxLength(15)]],
    note: [''],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();
    try {
      await this.store.addVehicle(this.data.residentId, {
        plate: raw.plate,
        note: raw.note || null,
      });
      this.dialogRef.close(true);
    } catch (error) {
      if (error instanceof HttpErrorResponse && isProblemDetails(error.error) && error.error.errors?.['Plate']) {
        this.form.controls.plate.setErrors({ server: error.error.errors['Plate'][0] });
      }
    } finally {
      this.submitting.set(false);
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
