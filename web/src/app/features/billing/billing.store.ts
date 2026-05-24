import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { BillingApi } from './billing.api';
import type {
  DuesPeriodListItem,
  OpenDuesPeriodRequest,
  OpenUtilityBillRequest,
  PayByCardRequest,
  PeriodItem,
  SiteDebtSummary,
  UtilityBillPeriodListItem,
} from '../../core/api/api.models';

/**
 * Signal-based store for the billing feature of the open site. Holds both
 * period kinds (dues + utility), the per-site debt summary, and the distributed
 * items lazily loaded per period. Mutations re-fetch the affected slices so the
 * period tables, item tables and debt card stay consistent.
 */
@Injectable({ providedIn: 'root' })
export class BillingStore {
  private readonly api = inject(BillingApi);

  private readonly duesPeriodsSignal = signal<DuesPeriodListItem[]>([]);
  private readonly utilityPeriodsSignal = signal<UtilityBillPeriodListItem[]>([]);
  private readonly debtSummarySignal = signal<SiteDebtSummary | null>(null);
  private readonly duesItemsSignal = signal(new Map<string, PeriodItem[]>());
  private readonly utilityItemsSignal = signal(new Map<string, PeriodItem[]>());
  private readonly loadingSignal = signal(false);

  /** Dues periods of the open site (most recent month first). */
  readonly duesPeriods = this.duesPeriodsSignal.asReadonly();

  /** Utility bill periods of the open site (most recent month first). */
  readonly utilityPeriods = this.utilityPeriodsSignal.asReadonly();

  /** Accrued / collected / outstanding totals for the open site. */
  readonly debtSummary = this.debtSummarySignal.asReadonly();

  /** True while any request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** The loaded items of one dues period (empty until that period is expanded). */
  duesItems(duesPeriodId: string): PeriodItem[] {
    return this.duesItemsSignal().get(duesPeriodId) ?? [];
  }

  /** The loaded items of one utility bill period. */
  utilityItems(utilityBillPeriodId: string): PeriodItem[] {
    return this.utilityItemsSignal().get(utilityBillPeriodId) ?? [];
  }

  /** Loads both period kinds and the debt summary for a site in parallel. */
  async loadForSite(siteId: string): Promise<void> {
    this.loadingSignal.set(true);
    try {
      const [dues, utilities, debt] = await Promise.all([
        firstValueFrom(this.api.duesPeriods(siteId)),
        firstValueFrom(this.api.utilityPeriods(siteId)),
        firstValueFrom(this.api.debtSummary(siteId)),
      ]);
      this.duesPeriodsSignal.set(dues);
      this.utilityPeriodsSignal.set(utilities);
      this.debtSummarySignal.set(debt);
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Opens a dues period, then refreshes the site billing. */
  async openDues(siteId: string, body: OpenDuesPeriodRequest): Promise<void> {
    await firstValueFrom(this.api.openDues(body));
    await this.loadForSite(siteId);
  }

  /** Distributes a dues period, then refreshes its items, the period list and the debt summary. */
  async distributeDues(siteId: string, duesPeriodId: string): Promise<void> {
    await firstValueFrom(this.api.distributeDues(duesPeriodId));
    await Promise.all([
      this.loadDuesItems(duesPeriodId),
      this.refreshDuesPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  /** Closes a dues period, then refreshes the site billing. */
  async closeDues(siteId: string, duesPeriodId: string): Promise<void> {
    await firstValueFrom(this.api.closeDues(duesPeriodId));
    await this.loadForSite(siteId);
  }

  /** Loads (or refreshes) the distributed items of one dues period. */
  async loadDuesItems(duesPeriodId: string): Promise<void> {
    const items = await firstValueFrom(this.api.duesItems(duesPeriodId));
    this.duesItemsSignal.update((map) => new Map(map).set(duesPeriodId, items));
  }

  /** Opens a utility bill period, then refreshes the site billing. */
  async openUtility(siteId: string, body: OpenUtilityBillRequest): Promise<void> {
    await firstValueFrom(this.api.openUtility(body));
    await this.loadForSite(siteId);
  }

  /** Distributes a utility bill period, then refreshes its items, the period list and the debt summary. */
  async distributeUtility(siteId: string, utilityBillPeriodId: string): Promise<void> {
    await firstValueFrom(this.api.distributeUtility(utilityBillPeriodId));
    await Promise.all([
      this.loadUtilityItems(utilityBillPeriodId),
      this.refreshUtilityPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  /** Closes a utility bill period, then refreshes the site billing. */
  async closeUtility(siteId: string, utilityBillPeriodId: string): Promise<void> {
    await firstValueFrom(this.api.closeUtility(utilityBillPeriodId));
    await this.loadForSite(siteId);
  }

  /** Loads (or refreshes) the distributed items of one utility bill period. */
  async loadUtilityItems(utilityBillPeriodId: string): Promise<void> {
    const items = await firstValueFrom(this.api.utilityItems(utilityBillPeriodId));
    this.utilityItemsSignal.update((map) => new Map(map).set(utilityBillPeriodId, items));
  }

  /** Marks a dues item paid, then refreshes its items, the period list and the debt summary. */
  async payDuesItem(siteId: string, duesPeriodId: string, itemId: string): Promise<void> {
    await firstValueFrom(this.api.payDuesItem(duesPeriodId, itemId));
    await Promise.all([
      this.loadDuesItems(duesPeriodId),
      this.refreshDuesPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  /** Marks a utility bill item paid, then refreshes its items, the period list and the debt summary. */
  async payUtilityItem(siteId: string, utilityBillPeriodId: string, itemId: string): Promise<void> {
    await firstValueFrom(this.api.payUtilityItem(utilityBillPeriodId, itemId));
    await Promise.all([
      this.loadUtilityItems(utilityBillPeriodId),
      this.refreshUtilityPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  /** Charges a dues item by card via the payment gateway, then refreshes views. */
  async payDuesItemByCard(
    siteId: string,
    duesPeriodId: string,
    itemId: string,
    card: PayByCardRequest,
  ): Promise<void> {
    await firstValueFrom(this.api.payDuesItemByCard(duesPeriodId, itemId, card));
    await Promise.all([
      this.loadDuesItems(duesPeriodId),
      this.refreshDuesPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  /** Charges a utility bill item by card via the payment gateway, then refreshes views. */
  async payUtilityItemByCard(
    siteId: string,
    utilityBillPeriodId: string,
    itemId: string,
    card: PayByCardRequest,
  ): Promise<void> {
    await firstValueFrom(this.api.payUtilityItemByCard(utilityBillPeriodId, itemId, card));
    await Promise.all([
      this.loadUtilityItems(utilityBillPeriodId),
      this.refreshUtilityPeriods(siteId),
      this.refreshDebtSummary(siteId),
    ]);
  }

  private async refreshDuesPeriods(siteId: string): Promise<void> {
    this.duesPeriodsSignal.set(await firstValueFrom(this.api.duesPeriods(siteId)));
  }

  private async refreshUtilityPeriods(siteId: string): Promise<void> {
    this.utilityPeriodsSignal.set(await firstValueFrom(this.api.utilityPeriods(siteId)));
  }

  private async refreshDebtSummary(siteId: string): Promise<void> {
    this.debtSummarySignal.set(await firstValueFrom(this.api.debtSummary(siteId)));
  }
}
