import { effect, inject, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';

const HUB_PATH = '/hubs/messaging';

/**
 * SignalR client for the messaging hub. Connects automatically when the user
 * becomes authenticated and disconnects on logout, so consumers (stores)
 * don't manage lifecycle. Exposes one observable per server event:
 * <c>messageReceived</c>, <c>conversationStarted</c>, <c>messageRead</c>.
 * Payloads are intentionally minimal — consumers refetch the canonical state
 * from the read API, which keeps the push channel free of drift.
 */
@Injectable({ providedIn: 'root' })
export class MessagingHubService {
  private readonly auth = inject(AuthService);

  private connection: HubConnection | null = null;
  private connecting: Promise<void> | null = null;

  private readonly messageReceivedSubject = new Subject<MessageReceivedPayload>();
  private readonly conversationStartedSubject = new Subject<ConversationStartedPayload>();
  private readonly messageReadSubject = new Subject<MessageReadPayload>();

  /** Fired when a new message lands in any conversation visible to this client. */
  readonly messageReceived = this.messageReceivedSubject.asObservable();

  /** Fired when a brand-new conversation appears in this client's inbox. */
  readonly conversationStarted = this.conversationStartedSubject.asObservable();

  /** Fired when the other side reads this client's messages. */
  readonly messageRead = this.messageReadSubject.asObservable();

  constructor() {
    effect(() => {
      const user = this.auth.currentUser();
      if (user) {
        void this.ensureConnected();
      } else {
        void this.disconnect();
      }
    });
  }

  /** Opens (or reuses) the hub connection. Idempotent and race-safe. */
  async ensureConnected(): Promise<void> {
    if (this.connection) {
      return;
    }
    if (this.connecting) {
      await this.connecting;
      return;
    }

    this.connecting = this.openConnection();
    try {
      await this.connecting;
    } finally {
      this.connecting = null;
    }
  }

  /** Closes the connection if open. Idempotent. */
  async disconnect(): Promise<void> {
    const conn = this.connection;
    this.connection = null;
    if (conn) {
      await conn.stop();
    }
  }

  private async openConnection(): Promise<void> {
    const connection = new HubConnectionBuilder()
      .withUrl(`${environment.apiBaseUrl}${HUB_PATH}`, {
        // Pulled fresh on each (re)connect, so a refreshed access token is picked up.
        accessTokenFactory: () => this.auth.accessToken ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on('MessageReceived', (payload: MessageReceivedPayload) =>
      this.messageReceivedSubject.next(payload),
    );
    connection.on('ConversationStarted', (payload: ConversationStartedPayload) =>
      this.conversationStartedSubject.next(payload),
    );
    connection.on('MessageRead', (payload: MessageReadPayload) =>
      this.messageReadSubject.next(payload),
    );

    await connection.start();
    this.connection = connection;
  }
}

/** Server payload for the <c>MessageReceived</c> push. */
export interface MessageReceivedPayload {
  conversationId: string;
}

/** Server payload for the <c>ConversationStarted</c> push. */
export interface ConversationStartedPayload {
  conversationId: string;
  residentId: string;
}

/** Server payload for the <c>MessageRead</c> push. */
export interface MessageReadPayload {
  conversationId: string;
}
