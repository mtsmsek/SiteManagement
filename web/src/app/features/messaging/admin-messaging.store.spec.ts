import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { AdminMessagingStore } from './admin-messaging.store';
import { MessagingApi } from './messaging.api';
import {
  ConversationStartedPayload,
  MessageReadPayload,
  MessageReceivedPayload,
  MessagingHubService,
} from '../../core/realtime/messaging-hub.service';
import type { ConversationListItem, ConversationMessage } from '../../core/api/api.models';

describe('AdminMessagingStore', () => {
  const conversation: ConversationListItem = {
    id: 'conv-1',
    residentId: 'res-1',
    residentName: 'Ada Lovelace',
    subject: 'Aidat',
    messageCount: 1,
    lastMessageAtUtc: '2026-01-01T09:00:00Z',
    unreadForAdmin: 1,
    unreadForResident: 0,
  };
  const message: ConversationMessage = {
    id: 'msg-1',
    senderRole: 'Resident',
    body: 'Merhaba',
    sentAtUtc: '2026-01-01T09:00:00Z',
    readAtUtc: null,
  };

  type ApiMock = Record<
    'list' | 'messages' | 'markRead' | 'reply' | 'start',
    ReturnType<typeof vi.fn>
  >;
  let api: ApiMock;
  let store: AdminMessagingStore;
  let messageReceivedSubject: Subject<MessageReceivedPayload>;
  let conversationStartedSubject: Subject<ConversationStartedPayload>;
  let messageReadSubject: Subject<MessageReadPayload>;

  beforeEach(() => {
    // arrange
    api = {
      list: vi.fn().mockReturnValue(of([conversation])),
      messages: vi.fn().mockReturnValue(of([message])),
      markRead: vi.fn().mockReturnValue(of(void 0)),
      reply: vi.fn().mockReturnValue(of(void 0)),
      start: vi.fn().mockReturnValue(of({ conversationId: 'conv-1' })),
    };
    messageReceivedSubject = new Subject<MessageReceivedPayload>();
    conversationStartedSubject = new Subject<ConversationStartedPayload>();
    messageReadSubject = new Subject<MessageReadPayload>();
    const hub: Pick<MessagingHubService, 'messageReceived' | 'conversationStarted' | 'messageRead'> = {
      messageReceived: messageReceivedSubject.asObservable(),
      conversationStarted: conversationStartedSubject.asObservable(),
      messageRead: messageReadSubject.asObservable(),
    };
    TestBed.configureTestingModule({
      providers: [
        AdminMessagingStore,
        { provide: MessagingApi, useValue: api },
        { provide: MessagingHubService, useValue: hub },
      ],
    });
    store = TestBed.inject(AdminMessagingStore);
  });

  it('loads every conversation into the inbox', async () => {
    // act
    await store.loadConversations();

    // assert
    expect(store.conversations()).toEqual([conversation]);
  });

  it('opening a thread loads its messages and marks the resident-side read', async () => {
    // act
    await store.open('conv-1');

    // assert
    expect(store.selectedId()).toBe('conv-1');
    expect(store.messages()).toEqual([message]);
    expect(api.markRead).toHaveBeenCalledWith('conv-1');
    expect(api.list).toHaveBeenCalled();
  });

  it('replying posts to the open thread then refreshes its messages', async () => {
    // arrange
    await store.open('conv-1');

    // act
    await store.reply('Tamam');

    // assert
    expect(api.reply).toHaveBeenCalledWith('conv-1', 'Tamam');
    expect(api.messages).toHaveBeenCalledWith('conv-1');
  });

  it('starting a thread creates it then opens it', async () => {
    // act
    await store.start('res-1', 'Konu', 'İçerik');

    // assert
    expect(api.start).toHaveBeenCalledWith('res-1', 'Konu', 'İçerik');
    expect(store.selectedId()).toBe('conv-1');
  });

  it('refreshes the inbox when the hub pushes MessageReceived', async () => {
    // arrange
    api.list.mockClear();

    // act — server pushed a new message
    messageReceivedSubject.next({ conversationId: 'conv-1' });
    await Promise.resolve();
    await Promise.resolve();

    // assert
    expect(api.list).toHaveBeenCalled();
  });
});
