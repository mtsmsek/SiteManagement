import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TenancyApi } from './tenancy.api';
import type { ApartmentOccupant, AssignResidentRequest } from '../../core/api/api.models';

/**
 * Signal-based store for the tenancy feature. Holds the active occupants of the
 * site currently open in the detail view and re-fetches them after each
 * assignment change so the apartment table reflects who lives where. Occupancy
 * itself is a consequence of assignment — the store never toggles it directly.
 */
@Injectable({ providedIn: 'root' })
export class TenancyStore {
  private readonly api = inject(TenancyApi);

  private readonly occupantsSignal = signal<ApartmentOccupant[]>([]);
  private readonly loadingSignal = signal(false);

  /** Active occupants of the open site. */
  readonly occupants = this.occupantsSignal.asReadonly();

  /** True while any request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** Occupants keyed by apartment id, so the apartment table can look one up in O(1). */
  readonly occupantByApartmentId = computed(
    () => new Map(this.occupantsSignal().map((o) => [o.apartmentId, o] as const)),
  );

  /** Loads the active occupants for a site. */
  async loadOccupants(siteId: string): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.occupantsSignal.set(await firstValueFrom(this.api.occupantsForSite(siteId)));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Assigns a resident to an apartment, then refreshes the site's occupants. */
  async assign(siteId: string, body: AssignResidentRequest): Promise<void> {
    await firstValueFrom(this.api.assign(body));
    await this.loadOccupants(siteId);
  }

  /** Ends an assignment (move-out), then refreshes the site's occupants. */
  async endAssignment(siteId: string, assignmentId: string, endDate: string): Promise<void> {
    await firstValueFrom(this.api.endAssignment(assignmentId, { endDate }));
    await this.loadOccupants(siteId);
  }
}
