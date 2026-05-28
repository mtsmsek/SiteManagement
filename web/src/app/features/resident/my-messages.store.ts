import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom, merge } from 'rxjs';
import { ResidentApi } from './resident.api';
import { MessagingHubService } from '../../core/realtime/messaging-hub.service';
import type { ConversationListItem, ConversationMessage } from '../../core/api/api.models';

/**
 * Signal-based store for the resident's own conversations. Holds the inbox list
 * and the messages of the currently open thread. Opening a thread also marks the
 * admin's messages read and refreshes the inbox so the unread badge clears.
 * SignalR pushes trigger a server refetch — no payload merging — so the read
 * projection stays the single source of truth.
 */
@Injectable({ providedIn: 'root' })
export class MyMessagesStore {
  private readonly api = inject(ResidentApi);
  private readonly hub = inject(MessagingHubService);

  constructor() {
    merge(this.hub.messageReceived, this.hub.conversationStarted, this.hub.messageRead).subscribe(
      () => void this.refreshOnPush(),
    );
  }

  private async refreshOnPush(): Promise<void> {
    await this.refreshConversations();
    const selected = this.selectedIdSignal();
    if (selected) {
      this.messagesSignal.set(await firstValueFrom(this.api.conversationMessages(selected)));
    }
  }

  private readonly conversationsSignal = signal<ConversationListItem[]>([]);
  private readonly messagesSignal = signal<ConversationMessage[]>([]);
  private readonly selectedIdSignal = signal<string | null>(null);
  private readonly loadingSignal = signal(false);

  /** The resident's conversations, most recently active first. */
  readonly conversations = this.conversationsSignal.asReadonly();

  /** Messages of the open thread (empty when none is selected). */
  readonly messages = this.messagesSignal.asReadonly();

  /** Id of the open conversation, or null. */
  readonly selectedId = this.selectedIdSignal.asReadonly();

  /** True while a request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** Loads the resident's conversation list. */
  async loadConversations(): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.conversationsSignal.set(await firstValueFrom(this.api.myConversations()));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Opens a thread: loads its messages, marks the admin's as read, refreshes the badge. */
  async open(conversationId: string): Promise<void> {
    this.selectedIdSignal.set(conversationId);
    this.messagesSignal.set(await firstValueFrom(this.api.conversationMessages(conversationId)));
    await firstValueFrom(this.api.markConversationRead(conversationId));
    await this.refreshConversations();
  }

  /** Posts a reply to the open thread, then refreshes its messages + the inbox. */
  async reply(body: string): Promise<void> {
    const conversationId = this.selectedIdSignal();
    if (!conversationId) {
      return;
    }

    await firstValueFrom(this.api.replyToConversation(conversationId, body));
    this.messagesSignal.set(await firstValueFrom(this.api.conversationMessages(conversationId)));
    await this.refreshConversations();
  }

  /** Opens a brand-new conversation, then selects it. */
  async start(subject: string, body: string): Promise<void> {
    const created = await firstValueFrom(this.api.startConversation(subject, body));
    await this.refreshConversations();
    await this.open(created.conversationId);
  }

  private async refreshConversations(): Promise<void> {
    this.conversationsSignal.set(await firstValueFrom(this.api.myConversations()));
  }
}
