/**
 * Tiny MailHog HTTP client. The demo seeder routes welcome emails through
 * the production <c>IEmailSender</c> path, so the temporary password the
 * recorder needs to log in as a resident lives in MailHog. This helper
 * fetches the message and parses the <c>Password: ...</c> line out of the
 * plain-text body.
 */

interface MailHogMessage {
  Content: {
    Headers: Record<string, string[]>;
    Body: string;
  };
  To: Array<{
    Mailbox: string;
    Domain: string;
  }>;
}

const MAILHOG_API = 'http://localhost:8025/api/v2/messages';

/**
 * Reads the most recent welcome email for the given recipient and returns
 * the parsed password. Throws if the email or the password line is missing
 * — the recorder must not silently continue with stale credentials.
 */
export async function readWelcomePasswordFor(email: string): Promise<string> {
  const response = await fetch(`${MAILHOG_API}?limit=100`);
  if (!response.ok) {
    throw new Error(`MailHog API responded with ${response.status}`);
  }
  const payload = (await response.json()) as { items: MailHogMessage[] };

  const match = payload.items.find((m) =>
    m.To.some((t) => `${t.Mailbox}@${t.Domain}`.toLowerCase() === email.toLowerCase()),
  );
  if (!match) {
    throw new Error(`No welcome email found in MailHog for ${email}`);
  }

  // MailHog hands the body back wire-encoded (whatever Content-Transfer-Encoding
  // SmtpEmailSender / System.Net.Mail chose at send time). For plain-ASCII text
  // that's "quoted-printable" most of the time, but Turkish characters in the
  // resident's display name flip the .NET sender to base64. Handle both.
  const encoding = (match.Content.Headers['Content-Transfer-Encoding']?.[0] ?? '').toLowerCase();
  const body = decodeBody(match.Content.Body, encoding);

  // Body line we want: "Password: DemoPa55!". Leading whitespace varies.
  const line = body.split(/\r?\n/).find((l) => /^Password:\s+/i.test(l.trim()));
  if (!line) {
    throw new Error(`Welcome email for ${email} has no "Password:" line`);
  }

  return line.replace(/^\s*Password:\s+/i, '').trim();
}

/** Decodes a MailHog body string according to its Content-Transfer-Encoding. */
function decodeBody(raw: string, encoding: string): string {
  switch (encoding) {
    case 'base64':
      return Buffer.from(raw.replace(/\s+/g, ''), 'base64').toString('utf-8');
    case 'quoted-printable':
      return decodeQuotedPrintable(raw);
    default:
      return raw;
  }
}

function decodeQuotedPrintable(input: string): string {
  // Drop soft line breaks (=<CRLF>), then turn =XX hex escapes into bytes.
  const stripped = input.replace(/=\r?\n/g, '');
  const bytes: number[] = [];
  for (let i = 0; i < stripped.length; i++) {
    const ch = stripped[i];
    if (ch === '=' && i + 2 < stripped.length) {
      const hex = stripped.substring(i + 1, i + 3);
      if (/^[0-9a-fA-F]{2}$/.test(hex)) {
        bytes.push(parseInt(hex, 16));
        i += 2;
        continue;
      }
    }
    bytes.push(ch.charCodeAt(0));
  }
  return Buffer.from(bytes).toString('utf-8');
}
