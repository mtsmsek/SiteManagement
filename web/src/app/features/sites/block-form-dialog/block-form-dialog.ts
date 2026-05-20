import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { isProblemDetails } from '../../../core/http/problem-details';

/** Identifies the site the new block is added to. */
export interface BlockFormDialogData {
  siteId: string;
}

/** Add-block dialog. Single name field; delegates to SitesStore.addBlock. */
@Component({
  selector: 'app-block-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './block-form-dialog.html',
})
export class BlockFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(SitesStore);
  private readonly dialogRef = inject(MatDialogRef<BlockFormDialog>);
  private readonly data = inject<BlockFormDialogData>(MAT_DIALOG_DATA);

  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    try {
      await this.store.addBlock(this.data.siteId, this.form.getRawValue());
      this.dialogRef.close(true);
    } catch (error) {
      if (error instanceof HttpErrorResponse && isProblemDetails(error.error) && error.error.errors?.['Name']) {
        this.form.controls.name.setErrors({ server: error.error.errors['Name'][0] });
      }
    } finally {
      this.submitting.set(false);
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
