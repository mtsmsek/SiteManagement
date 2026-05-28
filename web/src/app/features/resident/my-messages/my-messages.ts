import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslatePipe } from '@ngx-translate/core';
import { MyMessagesStore } from '../my-messages.store';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { MessageSenderRole } from '../../../core/api/api.models';

/**
 * Resident messaging page. A two-pane view: the resident's conversation list
 * (with unread badges) and the open thread's messages, where the resident can
 * reply or start a new thread. Opening a thread clears its unread badge. All
 * data is token-scoped — the resident only ever sees their own conversations.
 */
@Component({
  selector: 'app-my-messages',
  imports: [
    FormsModule,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatBadgeModule,
    MatProgressBarModule,
    TranslatePipe,
    EmptyState,
  ],
  templateUrl: './my-messages.html',
  styleUrl: './my-messages.scss',
})
export class MyMessages implements OnInit {
  private readonly store = inject(MyMessagesStore);

  readonly conversations = this.store.conversations;
  readonly messages = this.store.messages;
  readonly selectedId = this.store.selectedId;
  readonly loading = this.store.loading;
  readonly residentRole = MessageSenderRole.resident;

  /** Whether the "new conversation" form is open. */
  readonly composing = signal(false);

  readonly newSubject = signal('');
  readonly newBody = signal('');
  readonly replyBody = signal('');

  /** True when the new-thread form is ready to submit. */
  readonly canStart = computed(() => this.newSubject().trim().length > 0 && this.newBody().trim().length > 0);

  ngOnInit(): void {
    void this.store.loadConversations();
  }

  /** Opens a thread. */
  select(conversationId: string): void {
    void this.store.open(conversationId);
  }

  /** Sends the reply in the open thread and clears the box. */
  async sendReply(): Promise<void> {
    const body = this.replyBody().trim();
    if (body.length === 0) {
      return;
    }

    await this.store.reply(body);
    this.replyBody.set('');
  }

  /** Opens a new conversation from the inline form, then resets it. */
  async startThread(): Promise<void> {
    if (!this.canStart()) {
      return;
    }

    await this.store.start(this.newSubject().trim(), this.newBody().trim());
    this.newSubject.set('');
    this.newBody.set('');
    this.composing.set(false);
  }
}
