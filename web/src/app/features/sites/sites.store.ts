import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { SitesApi } from './sites.api';
import type {
  SiteListItem,
  SiteDetails,
  CreateSiteRequest,
  AddBlockRequest,
  AddApartmentRequest,
} from '../../core/api/api.models';

/**
 * Signal-based store for the sites feature. Holds the list + the currently
 * open detail, exposes loading/error as signals, and re-fetches after each
 * mutation so the views stay consistent without manual cache surgery.
 */
@Injectable({ providedIn: 'root' })
export class SitesStore {
  private readonly api = inject(SitesApi);

  private readonly sitesSignal = signal<SiteListItem[]>([]);
  private readonly detailSignal = signal<SiteDetails | null>(null);
  private readonly loadingSignal = signal(false);

  /** All sites for the list page. */
  readonly sites = this.sitesSignal.asReadonly();

  /** The currently loaded site detail, or null. */
  readonly detail = this.detailSignal.asReadonly();

  /** True while any request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** True when the list has loaded and is empty (drives the empty state). */
  readonly isEmpty = computed(() => !this.loadingSignal() && this.sitesSignal().length === 0);

  /** Loads the full sites list. */
  async loadList(): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.sitesSignal.set(await firstValueFrom(this.api.list()));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Loads one site's detail (blocks + apartments). */
  async loadDetail(siteId: string): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.detailSignal.set(await firstValueFrom(this.api.getById(siteId)));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Creates a site and refreshes the list. */
  async create(body: CreateSiteRequest): Promise<void> {
    await firstValueFrom(this.api.create(body));
    await this.loadList();
  }

  /** Deletes a site and refreshes the list. */
  async remove(siteId: string): Promise<void> {
    await firstValueFrom(this.api.remove(siteId));
    await this.loadList();
  }

  /** Adds a block to the open site and refreshes its detail. */
  async addBlock(siteId: string, body: AddBlockRequest): Promise<void> {
    await firstValueFrom(this.api.addBlock(siteId, body));
    await this.loadDetail(siteId);
  }

  /** Adds an apartment to a block and refreshes the open site detail. */
  async addApartment(siteId: string, blockId: string, body: AddApartmentRequest): Promise<void> {
    await firstValueFrom(this.api.addApartment(blockId, body));
    await this.loadDetail(siteId);
  }

  /** Toggles an apartment between occupied and empty, then refreshes detail. */
  async setApartmentOccupancy(siteId: string, apartmentId: string, occupied: boolean): Promise<void> {
    await firstValueFrom(
      occupied ? this.api.occupyApartment(apartmentId) : this.api.vacateApartment(apartmentId),
    );
    await this.loadDetail(siteId);
  }
}
