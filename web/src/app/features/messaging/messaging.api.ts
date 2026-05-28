import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  ConversationCreated,
  ConversationListItem,
  ConversationMessage,
} from '../../core/api/api.models';

/**
 * Admin side of the Messaging context. Sees every conversation; opens a thread
 * with a chosen resident, reads + replies, and clears the unread badge on read.
 * The resident self-service equivalent lives in <c>ResidentApi</c>.
 */
@Injectable({ providedIn: 'root' })
export class MessagingApi {
  private readonly api = inject(ApiClient);

  /** Every conversation, most recently active first. */
  list(): Observable<ConversationListItem[]> {
    return this.api.get<ConversationListItem[]>('/conversations');
  }

  /** Messages of one thread, in send order. */
  messages(conversationId: string): Observable<ConversationMessage[]> {
    return this.api.get<ConversationMessage[]>(`/conversations/${conversationId}/messages`);
  }

  /** Opens a new thread with the given resident, posting the first message. */
  start(residentId: string, subject: string, body: string): Observable<ConversationCreated> {
    return this.api.post<ConversationCreated>('/conversations', { residentId, subject, body });
  }

  /** Appends an admin reply to the thread. */
  reply(conversationId: string, body: string): Observable<void> {
    return this.api.post<void>(`/conversations/${conversationId}/messages`, { body });
  }

  /** Marks the resident's messages in the thread as read by the admin. */
  markRead(conversationId: string): Observable<void> {
    return this.api.post<void>(`/conversations/${conversationId}/read`);
  }
}
