import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  ConversationCreated,
  ConversationListItem,
  ConversationMessage,
  PayByCardRequest,
  ResidentBill,
  ResidentDashboard,
} from '../../core/api/api.models';

/**
 * Resident self-service API surface. Every path is token-scoped on the server
 * (`/api/me/*`) — the resident id is never sent, so a resident can only ever
 * reach their own bills and conversations.
 */
@Injectable({ providedIn: 'root' })
export class ResidentApi {
  private readonly api = inject(ApiClient);

  /** The current resident's portal summary (outstanding, credit, unread messages). */
  myDashboard(): Observable<ResidentDashboard> {
    return this.api.get<ResidentDashboard>('/me/dashboard');
  }

  /** The current resident's own bills (dues + utility). */
  myBills(): Observable<ResidentBill[]> {
    return this.api.get<ResidentBill[]>('/me/bills');
  }

  /** Pays one of the resident's own dues items by card. */
  payDuesItem(duesPeriodId: string, itemId: string, card: PayByCardRequest): Observable<void> {
    return this.api.post<void>(`/me/dues/${duesPeriodId}/items/${itemId}/pay-by-card`, card);
  }

  /** Pays one of the resident's own utility items by card. */
  payUtilityItem(utilityBillPeriodId: string, itemId: string, card: PayByCardRequest): Observable<void> {
    return this.api.post<void>(`/me/utility-bills/${utilityBillPeriodId}/items/${itemId}/pay-by-card`, card);
  }

  /** The current resident's own conversations. */
  myConversations(): Observable<ConversationListItem[]> {
    return this.api.get<ConversationListItem[]>('/me/conversations');
  }

  /** The messages of one of the resident's own conversations. */
  conversationMessages(conversationId: string): Observable<ConversationMessage[]> {
    return this.api.get<ConversationMessage[]>(`/me/conversations/${conversationId}/messages`);
  }

  /** Opens a new conversation for the resident with a first message. */
  startConversation(subject: string, body: string): Observable<ConversationCreated> {
    return this.api.post<ConversationCreated>('/me/conversations', { subject, body });
  }

  /** Appends the resident's reply to one of their own conversations. */
  replyToConversation(conversationId: string, body: string): Observable<void> {
    return this.api.post<void>(`/me/conversations/${conversationId}/messages`, { body });
  }

  /** Marks the admin's messages in the resident's own thread as read. */
  markConversationRead(conversationId: string): Observable<void> {
    return this.api.post<void>(`/me/conversations/${conversationId}/read`);
  }
}
