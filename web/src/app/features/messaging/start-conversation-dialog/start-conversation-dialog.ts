import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentsStore } from '../../residents/residents.store';

/**
 * Modal that opens a brand-new conversation. The admin picks a resident from
 * the residents list, types a subject + body, and on submit the result is
 * returned to the caller (the messaging page opens it).
 */
@Component({
  selector: 'app-start-conversation-dialog',
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    TranslatePipe,
  ],
  templateUrl: './start-conversation-dialog.html',
  styleUrl: './start-conversation-dialog.scss',
})
export class StartConversationDialog implements OnInit {
  private readonly residentsStore = inject(ResidentsStore);
  private readonly dialogRef = inject(MatDialogRef<StartConversationDialog, StartConversationResult>);

  readonly residents = this.residentsStore.residents;

  readonly residentId = signal<string>('');
  readonly subject = signal<string>('');
  readonly body = signal<string>('');

  readonly canSubmit = computed(
    () => this.residentId().length > 0 && this.subject().trim().length > 0 && this.body().trim().length > 0,
  );

  ngOnInit(): void {
    if (this.residents().length === 0) {
      void this.residentsStore.loadList();
    }
  }

  /** Submits the form and closes with the chosen values. */
  submit(): void {
    if (!this.canSubmit()) {
      return;
    }

    this.dialogRef.close({
      residentId: this.residentId(),
      subject: this.subject().trim(),
      body: this.body().trim(),
    });
  }

  /** Closes without picking. */
  cancel(): void {
    this.dialogRef.close();
  }
}

/** Result the dialog returns when the admin submits a new conversation. */
export interface StartConversationResult {
  residentId: string;
  subject: string;
  body: string;
}
