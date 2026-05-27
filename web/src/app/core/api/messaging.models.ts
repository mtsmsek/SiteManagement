/**
 * Hand-authored mirrors of the Messaging API DTOs. These schemas were added
 * after the last `npm run gen:api` run, so they aren't in `api-types.ts` yet —
 * this file is a temporary bridge that matches the backend DTOs exactly
 * (camelCase, as the API serialises them). Once `gen:api` is re-run against the
 * updated OpenAPI, replace these with aliases in `api.models.ts` and delete this
 * file.
 */

/** One conversation row in an inbox, with per-side unread counts. */
export interface ConversationListItem {
  id: string;
  residentId: string;
  subject: string;
  messageCount: number;
  lastMessageAtUtc: string;
  unreadForAdmin: number;
  unreadForResident: number;
}

/** One message within a conversation thread. */
export interface ConversationMessage {
  id: string;
  senderRole: string;
  body: string;
  sentAtUtc: string;
  readAtUtc: string | null;
}

/** Result of opening a new conversation. */
export interface ConversationCreated {
  conversationId: string;
}

/** Message sender role values, as the API serialises them. */
export const MessageSenderRole = {
  admin: 'Admin',
  resident: 'Resident',
} as const;
