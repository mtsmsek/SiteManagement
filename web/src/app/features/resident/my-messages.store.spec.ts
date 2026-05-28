import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { MyMessagesStore } from './my-messages.store';
import { ResidentApi } from './resident.api';
import {
  ConversationStartedPayload,
  MessageReadPayload,
  MessageReceivedPayload,
  MessagingHubService,
} from '../../core/realtime/messaging-hub.service';
import type { ConversationListItem, ConversationMessage } from '../../core/api/api.models';

describe('MyMessagesStore', () => {
  const conversation: ConversationListItem = {
    id: 'conv-1',
    residentId: 'res-1',
    residentName: 'Ada Lovelace',
    subject: 'Su kaçağı',
    messageCount: 1,
    lastMessageAtUtc: '2026-01-01T09:00:00Z',
    unreadForAdmin: 0,
    unreadForResident: 1,
  };
  const message: ConversationMessage = {
    id: 'msg-1',
    senderRole: 'Admin',
    body: 'Merhaba',
    sentAtUtc: '2026-01-01T09:00:00Z',
    readAtUtc: null,
  };

  type ApiMock = Record<
    'myConversations' | 'conversationMessages' | 'markConversationRead' | 'replyToConversation' | 'startConversation',
    ReturnType<typeof vi.fn>
  >;
  let api: ApiMock;
  let store: MyMessagesStore;
  let messageReceivedSubject: Subject<MessageReceivedPayload>;
  let conversationStartedSubject: Subject<ConversationStartedPayload>;
  let messageReadSubject: Subject<MessageReadPayload>;

  beforeEach(() => {
    // arrange
    api = {
      myConversations: vi.fn().mockReturnValue(of([conversation])),
      conversationMessages: vi.fn().mockReturnValue(of([message])),
      markConversationRead: vi.fn().mockReturnValue(of(void 0)),
      replyToConversation: vi.fn().mockReturnValue(of(void 0)),
      startConversation: vi.fn().mockReturnValue(of({ conversationId: 'conv-1' })),
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
        MyMessagesStore,
        { provide: ResidentApi, useValue: api },
        { provide: MessagingHubService, useValue: hub },
      ],
    });
    store = TestBed.inject(MyMessagesStore);
  });

  it('loads the conversation list', async () => {
    // act
    await store.loadConversations();

    // assert
    expect(store.conversations()).toEqual([conversation]);
  });

  it('opening a thread loads its messages and marks it read', async () => {
    // act
    await store.open('conv-1');

    // assert
    expect(store.selectedId()).toBe('conv-1');
    expect(store.messages()).toEqual([message]);
    expect(api.markConversationRead).toHaveBeenCalledWith('conv-1');
    expect(api.myConversations).toHaveBeenCalled();
  });

  it('replying posts to the open thread then refreshes its messages', async () => {
    // arrange
    await store.open('conv-1');

    // act
    await store.reply('Teşekkürler');

    // assert
    expect(api.replyToConversation).toHaveBeenCalledWith('conv-1', 'Teşekkürler');
    expect(api.conversationMessages).toHaveBeenCalledWith('conv-1');
  });

  it('starting a thread creates it then opens it', async () => {
    // act
    await store.start('Yeni', 'Konu metni');

    // assert
    expect(api.startConversation).toHaveBeenCalledWith('Yeni', 'Konu metni');
    expect(store.selectedId()).toBe('conv-1');
  });

  it('refreshes the inbox when the hub pushes ConversationStarted', async () => {
    // arrange
    api.myConversations.mockClear();

    // act — admin opened a brand-new thread for this resident
    conversationStartedSubject.next({ conversationId: 'conv-1', residentId: 'res-1' });
    await Promise.resolve();
    await Promise.resolve();

    // assert
    expect(api.myConversations).toHaveBeenCalled();
  });
});
