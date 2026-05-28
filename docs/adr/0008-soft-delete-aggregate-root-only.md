# 0008 — Soft delete only on aggregate roots (one root carries it: Site)

- **Status:** Accepted
- **Date:** 2026-03-21

## Context

The admin can delete a site, but "delete" here has to be reversible — a site
holds dues periods, utility bills, conversations, and historised tenancy
data; an accidental DELETE can't take all of that with it. At the same
time, a *real* purge has to be available for the day a site truly leaves.

Soft delete is the standard answer ("set `IsDeleted = true` instead of
DELETE"). The standard mistakes are:
- Putting the flag on every entity in the aggregate (Block, Apartment, ...)
  and then forgetting one — the orphans show up in reports months later.
- Forgetting the EF query filter, so soft-deleted rows leak into reads
  no one expected.
- Conflating "soft delete this" with "remove this child" — the inner-entity
  remove operations need their own semantics.

## Decision

**Only the aggregate root carries soft delete.** The aggregate is reached
through the root in every read path, so hiding the root hides the whole
tree — Blocks and Apartments inside an archived Site become unreachable for
free. No `IsDeleted` on Block, no `IsDeleted` on Apartment.

Concretely: `Site` implements `ISoftDeletable` and has an
`IQueryFilter` registered on the EF model so every `_dbContext.Sites`
read excludes archived rows by default. Hard delete uses
`FindIncludingArchivedAsync` to bypass the filter on purpose. Three
distinct endpoints make the intent visible:

- `DELETE /api/sites/{id}` → `Site.Archive(now)` (soft).
- `POST /api/sites/{id}/restore` → `Site.Restore()` (un-archive).
- `DELETE /api/sites/{id}/permanent` → `PurgeSiteCommand` (hard).

Soft-deleting is an explicit domain operation. The system never auto-converts
a `Remove()` call into a flag flip — a child remove from a loaded aggregate
still calls `RemoveBlock` / `RemoveApartment` and the row goes away.

## Alternatives considered

- **Soft delete on every entity** — duplicates the work, gives every
  query filter to keep in sync, and makes "the apartment is archived but
  the site isn't" a state the domain has to model. Not worth it.
- **No soft delete; rely on backups** — recovery becomes operations
  work, not a UI affordance. Hostile to "I clicked the wrong site".
- **Tombstone rows in a sibling table (`ArchivedSites`)** — adds a write
  on every archive and a join on every read. The query filter on the
  primary table is simpler.

## Consequences

- **Positive:** archive / restore / purge are three obvious commands
  with three obvious URLs. Reads "just work" — the query filter is
  invisible to the rest of the application. Adding a new soft-deletable
  aggregate is exactly: implement `ISoftDeletable`, add the flag, register
  the filter; the guardrail catches the third step.
- **Negative:** a contributor reading `DELETE` on an inner entity must
  remember the rule — the root is soft, the children are hard. The
  domain itself enforces it (the inner-entity remove methods don't go
  through soft delete) but the rule is one of those things that needs
  to be in the contributor's head.
- **Guardrail:** an architecture test (`SoftDeleteConventionTests`)
  asserts every type implementing `ISoftDeletable` has an
  `IQueryFilter` registered on its EF entity type; forgetting the
  filter fails the build, so an "I added soft delete but the rows
  still show up" bug can't ship.
