import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { BillingStore } from './billing.store';
import { BillingApi } from './billing.api';
import {
  UtilityType,
  type DuesPeriodListItem,
  type PeriodItem,
  type SiteDebtSummary,
  type UtilityBillPeriodListItem,
} from '../../core/api/api.models';

describe('BillingStore', () => {
  const siteId = 'site-1';
  const duesPeriod: DuesPeriodListItem = {
    id: 'dues-1',
    month: '2026-01',
    perApartmentAmount: 500,
    itemCount: 1,
    paidCount: 0,
    isClosed: false,
  };
  const utilityPeriod: UtilityBillPeriodListItem = {
    id: 'util-1',
    month: '2026-01',
    utilityType: 'Electricity',
    totalAmount: 300,
    itemCount: 1,
    paidCount: 0,
    isClosed: false,
  };
  const debt: SiteDebtSummary = {
    siteId,
    totalAccrued: 800,
    totalCollected: 0,
    totalOutstanding: 800,
  };
  const item: PeriodItem = {
    itemId: 'item-1',
    apartmentId: 'apt-1',
    apartment: 'A-1',
    residentId: 'res-1',
    residentFullName: 'Ada Lovelace',
    amount: 500,
    status: 'Unpaid',
  };

  type ApiMock = Record<
    | 'duesPeriods'
    | 'utilityPeriods'
    | 'debtSummary'
    | 'duesItems'
    | 'utilityItems'
    | 'openDues'
    | 'distributeDues'
    | 'closeDues'
    | 'openUtility'
    | 'distributeUtility'
    | 'closeUtility'
    | 'payDuesItem'
    | 'payUtilityItem',
    ReturnType<typeof vi.fn>
  >;
  let api: ApiMock;
  let store: BillingStore;

  beforeEach(() => {
    // arrange — a stub API returning the seeded period/summary/item data
    api = {
      duesPeriods: vi.fn().mockReturnValue(of([duesPeriod])),
      utilityPeriods: vi.fn().mockReturnValue(of([utilityPeriod])),
      debtSummary: vi.fn().mockReturnValue(of(debt)),
      duesItems: vi.fn().mockReturnValue(of([item])),
      utilityItems: vi.fn().mockReturnValue(of([item])),
      openDues: vi.fn().mockReturnValue(of({ duesPeriodId: 'dues-1' })),
      distributeDues: vi.fn().mockReturnValue(of(void 0)),
      closeDues: vi.fn().mockReturnValue(of(void 0)),
      openUtility: vi.fn().mockReturnValue(of({ utilityBillPeriodId: 'util-1' })),
      distributeUtility: vi.fn().mockReturnValue(of(void 0)),
      closeUtility: vi.fn().mockReturnValue(of(void 0)),
      payDuesItem: vi.fn().mockReturnValue(of(void 0)),
      payUtilityItem: vi.fn().mockReturnValue(of(void 0)),
    };
    TestBed.configureTestingModule({
      providers: [BillingStore, { provide: BillingApi, useValue: api }],
    });
    store = TestBed.inject(BillingStore);
  });

  it('loads dues periods, utility periods and the debt summary for a site', async () => {
    // act
    await store.loadForSite(siteId);

    // assert
    expect(store.duesPeriods()).toEqual([duesPeriod]);
    expect(store.utilityPeriods()).toEqual([utilityPeriod]);
    expect(store.debtSummary()).toEqual(debt);
    expect(store.loading()).toBe(false);
  });

  it('opens a dues period then reloads the site billing', async () => {
    // act
    await store.openDues(siteId, { siteId, year: 2026, month: 1, perApartmentAmount: 500 });

    // assert
    expect(api.openDues).toHaveBeenCalledOnce();
    expect(api.duesPeriods).toHaveBeenCalledWith(siteId);
  });

  it('distributes a dues period then reloads its items and the debt summary', async () => {
    // act
    await store.distributeDues(siteId, 'dues-1');

    // assert
    expect(api.distributeDues).toHaveBeenCalledWith('dues-1');
    expect(api.duesItems).toHaveBeenCalledWith('dues-1');
    expect(api.debtSummary).toHaveBeenCalledWith(siteId);
    expect(store.duesItems('dues-1')).toEqual([item]);
  });

  it('pays a dues item then reloads its items, the period list and the debt summary', async () => {
    // act
    await store.payDuesItem(siteId, 'dues-1', 'item-1');

    // assert
    expect(api.payDuesItem).toHaveBeenCalledWith('dues-1', 'item-1');
    expect(api.duesItems).toHaveBeenCalledWith('dues-1');
    expect(api.duesPeriods).toHaveBeenCalledWith(siteId);
    expect(api.debtSummary).toHaveBeenCalledWith(siteId);
  });

  it('opens a utility bill period then reloads the site billing', async () => {
    // act
    await store.openUtility(siteId, {
      siteId,
      year: 2026,
      month: 1,
      utilityType: UtilityType.electricity,
      totalAmount: 300,
    });

    // assert
    expect(api.openUtility).toHaveBeenCalledOnce();
    expect(api.utilityPeriods).toHaveBeenCalledWith(siteId);
  });
});
