import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminMessagingStore } from '../admin-messaging.store';
import { MessageSenderRole } from '../../../core/api/api.models';
import {
  StartConversationDialog,
  StartConversationResult,
} from '../start-conversation-dialog/start-conversation-dialog';

/**
 * Admin messaging page. Two-pane view: every conversation in the inbox
 * (resident name + unread badge), and the open thread's messages with a reply
 * box. The admin can open a brand-new thread via the "New thread" dialog.
 * Mirrors the resident-side <c>MyMessages</c> page shape so the operator
 * experience is consistent.
 */
@Component({
  selector: 'app-admin-messaging',
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
  ],
  templateUrl: './admin-messaging.html',
  styleUrl: './admin-messaging.scss',
})
export class AdminMessaging implements OnInit {
  private readonly store = inject(AdminMessagingStore);
  private readonly dialog = inject(MatDialog);

  readonly conversations = this.store.conversations;
  readonly messages = this.store.messages;
  readonly selectedId = this.store.selectedId;
  readonly loading = this.store.loading;
  readonly adminRole = MessageSenderRole.admin;

  readonly replyBody = signal('');

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

  /** Opens the "new thread" dialog; on submit, opens the new conversation. */
  openStartDialog(): void {
    this.dialog
      .open<StartConversationDialog, void, StartConversationResult>(StartConversationDialog)
      .afterClosed()
      .subscribe((result) => {
        if (!result) {
          return;
        }
        void this.store.start(result.residentId, result.subject, result.body);
      });
  }
}
