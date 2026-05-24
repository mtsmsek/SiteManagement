import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  DuesPeriodListItem,
  OpenDuesPeriodRequest,
  OpenDuesPeriodResponse,
  OpenUtilityBillRequest,
  OpenUtilityBillResponse,
  PayByCardRequest,
  PeriodItem,
  SiteDebtSummary,
  UtilityBillPeriodListItem,
} from '../../core/api/api.models';

/**
 * HTTP surface for the Billing bounded context (dues + utility bill periods,
 * their distributed items, and the per-site debt summary). Stateless — returns
 * Observables; the BillingStore owns the resulting state.
 */
@Injectable({ providedIn: 'root' })
export class BillingApi {
  private readonly api = inject(ApiClient);

  duesPeriods(siteId: string): Observable<DuesPeriodListItem[]> {
    return this.api.get<DuesPeriodListItem[]>(`/dues/sites/${siteId}`);
  }

  utilityPeriods(siteId: string): Observable<UtilityBillPeriodListItem[]> {
    return this.api.get<UtilityBillPeriodListItem[]>(`/utility-bills/sites/${siteId}`);
  }

  debtSummary(siteId: string): Observable<SiteDebtSummary> {
    return this.api.get<SiteDebtSummary>(`/dues/sites/${siteId}/debt-summary`);
  }

  duesItems(duesPeriodId: string): Observable<PeriodItem[]> {
    return this.api.get<PeriodItem[]>(`/dues/${duesPeriodId}/items`);
  }

  utilityItems(utilityBillPeriodId: string): Observable<PeriodItem[]> {
    return this.api.get<PeriodItem[]>(`/utility-bills/${utilityBillPeriodId}/items`);
  }

  openDues(body: OpenDuesPeriodRequest): Observable<OpenDuesPeriodResponse> {
    return this.api.post<OpenDuesPeriodResponse>('/dues', body);
  }

  distributeDues(duesPeriodId: string): Observable<void> {
    return this.api.post<void>(`/dues/${duesPeriodId}/distribute`);
  }

  closeDues(duesPeriodId: string): Observable<void> {
    return this.api.post<void>(`/dues/${duesPeriodId}/close`);
  }

  openUtility(body: OpenUtilityBillRequest): Observable<OpenUtilityBillResponse> {
    return this.api.post<OpenUtilityBillResponse>('/utility-bills', body);
  }

  distributeUtility(utilityBillPeriodId: string): Observable<void> {
    return this.api.post<void>(`/utility-bills/${utilityBillPeriodId}/distribute`);
  }

  closeUtility(utilityBillPeriodId: string): Observable<void> {
    return this.api.post<void>(`/utility-bills/${utilityBillPeriodId}/close`);
  }

  payDuesItem(duesPeriodId: string, itemId: string): Observable<void> {
    return this.api.post<void>(`/dues/${duesPeriodId}/items/${itemId}/pay`);
  }

  payUtilityItem(utilityBillPeriodId: string, itemId: string): Observable<void> {
    return this.api.post<void>(`/utility-bills/${utilityBillPeriodId}/items/${itemId}/pay`);
  }

  payDuesItemByCard(duesPeriodId: string, itemId: string, card: PayByCardRequest): Observable<void> {
    return this.api.post<void>(`/dues/${duesPeriodId}/items/${itemId}/pay-by-card`, card);
  }

  payUtilityItemByCard(
    utilityBillPeriodId: string,
    itemId: string,
    card: PayByCardRequest,
  ): Observable<void> {
    return this.api.post<void>(`/utility-bills/${utilityBillPeriodId}/items/${itemId}/pay-by-card`, card);
  }
}
