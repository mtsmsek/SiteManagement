import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AdminMessagingStore } from './admin-messaging.store';
import { MessagingApi } from './messaging.api';
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

  beforeEach(() => {
    // arrange
    api = {
      list: vi.fn().mockReturnValue(of([conversation])),
      messages: vi.fn().mockReturnValue(of([message])),
      markRead: vi.fn().mockReturnValue(of(void 0)),
      reply: vi.fn().mockReturnValue(of(void 0)),
      start: vi.fn().mockReturnValue(of({ conversationId: 'conv-1' })),
    };
    TestBed.configureTestingModule({
      providers: [AdminMessagingStore, { provide: MessagingApi, useValue: api }],
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
});
