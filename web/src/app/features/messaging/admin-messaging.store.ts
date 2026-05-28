import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MessagingApi } from './messaging.api';
import type { ConversationListItem, ConversationMessage } from '../../core/api/api.models';

/**
 * Signal-based store for the admin messaging inbox. Mirrors
 * <c>MyMessagesStore</c> on the resident side: holds every conversation, the
 * open thread's messages, and refreshes the badge after a read. Opening a
 * thread marks the resident's messages read for the admin.
 */
@Injectable({ providedIn: 'root' })
export class AdminMessagingStore {
  private readonly api = inject(MessagingApi);

  private readonly conversationsSignal = signal<ConversationListItem[]>([]);
  private readonly messagesSignal = signal<ConversationMessage[]>([]);
  private readonly selectedIdSignal = signal<string | null>(null);
  private readonly loadingSignal = signal(false);

  /** Every conversation, most recently active first. */
  readonly conversations = this.conversationsSignal.asReadonly();

  /** Messages of the open thread (empty when none is selected). */
  readonly messages = this.messagesSignal.asReadonly();

  /** Id of the open conversation, or null. */
  readonly selectedId = this.selectedIdSignal.asReadonly();

  /** True while a request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** Loads the admin inbox. */
  async loadConversations(): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.conversationsSignal.set(await firstValueFrom(this.api.list()));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Opens a thread: loads its messages, marks the resident's as read, refreshes the badge. */
  async open(conversationId: string): Promise<void> {
    this.selectedIdSignal.set(conversationId);
    this.messagesSignal.set(await firstValueFrom(this.api.messages(conversationId)));
    await firstValueFrom(this.api.markRead(conversationId));
    await this.refreshConversations();
  }

  /** Posts a reply to the open thread, then refreshes its messages + the inbox. */
  async reply(body: string): Promise<void> {
    const conversationId = this.selectedIdSignal();
    if (!conversationId) {
      return;
    }

    await firstValueFrom(this.api.reply(conversationId, body));
    this.messagesSignal.set(await firstValueFrom(this.api.messages(conversationId)));
    await this.refreshConversations();
  }

  /** Opens a brand-new thread with the chosen resident, then selects it. */
  async start(residentId: string, subject: string, body: string): Promise<void> {
    const created = await firstValueFrom(this.api.start(residentId, subject, body));
    await this.refreshConversations();
    await this.open(created.conversationId);
  }

  private async refreshConversations(): Promise<void> {
    this.conversationsSignal.set(await firstValueFrom(this.api.list()));
  }
}
