import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { TenancyStore } from '../tenancy.store';
import { ResidentsStore } from '../../residents/residents.store';
import { isProblemDetails } from '../../../core/http/problem-details';
import { TenantType, TenantTypeOptions } from '../../../core/api/api.models';

/** Identifies the site (for the occupant refresh) and the apartment being assigned. */
export interface AssignResidentDialogData {
  siteId: string;
  apartmentId: string;
}

/**
 * Assigns a resident to an apartment. Picks an existing resident, an ownership
 * type and a start date, then delegates to TenancyStore.assign — which occupies
 * the apartment as a side effect. Surfaces API field errors inline.
 */
@Component({
  selector: 'app-assign-resident-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './assign-resident-dialog.html',
})
export class AssignResidentDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(TenancyStore);
  private readonly residentsStore = inject(ResidentsStore);
  private readonly dialogRef = inject(MatDialogRef<AssignResidentDialog, boolean>);
  private readonly data = inject<AssignResidentDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);
  readonly tenantTypeOptions = TenantTypeOptions;

  /** Residents offered in the picker; loaded on open. */
  readonly residents = this.residentsStore.residents;

  readonly form = this.fb.nonNullable.group({
    residentId: ['', [Validators.required]],
    tenantType: [TenantType.owner as number, [Validators.required]],
    startDate: [new Date().toISOString().slice(0, 10), [Validators.required]],
  });

  ngOnInit(): void {
    void this.residentsStore.loadList();
  }

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const raw = this.form.getRawValue();
    try {
      await this.store.assign(this.data.siteId, {
        apartmentId: this.data.apartmentId,
        residentId: raw.residentId,
        tenantType: raw.tenantType,
        startDate: raw.startDate,
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
