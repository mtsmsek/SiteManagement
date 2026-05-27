import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { MyBillsStore } from './my-bills.store';
import { ResidentApi } from './resident.api';
import type { PayByCardRequest, ResidentBill } from '../../core/api/api.models';

describe('MyBillsStore', () => {
  const duesBill: ResidentBill = {
    itemId: 'dues-item',
    periodId: 'dues-period',
    kind: 'Dues',
    month: '2026-01',
    detail: 'Dues',
    amount: 500,
    status: 'Unpaid',
  };
  const utilityBill: ResidentBill = {
    itemId: 'util-item',
    periodId: 'util-period',
    kind: 'Utility',
    month: '2026-01',
    detail: 'Electricity',
    amount: 300,
    status: 'Paid',
  };
  const card: PayByCardRequest = { cardNumber: '4242424242424242', cvv: '123', expiryYear: 2030, expiryMonth: 12 };

  type ApiMock = Record<'myBills' | 'payDuesItem' | 'payUtilityItem', ReturnType<typeof vi.fn>>;
  let api: ApiMock;
  let store: MyBillsStore;

  beforeEach(() => {
    // arrange — a stub API returning one unpaid dues + one paid utility bill
    api = {
      myBills: vi.fn().mockReturnValue(of([duesBill, utilityBill])),
      payDuesItem: vi.fn().mockReturnValue(of(void 0)),
      payUtilityItem: vi.fn().mockReturnValue(of(void 0)),
    };
    TestBed.configureTestingModule({
      providers: [MyBillsStore, { provide: ResidentApi, useValue: api }],
    });
    store = TestBed.inject(MyBillsStore);
  });

  it('loads the bills and sums only the unpaid ones', async () => {
    // act
    await store.load();

    // assert
    expect(store.bills()).toHaveLength(2);
    expect(store.totalOutstanding()).toBe(500);
    expect(store.loading()).toBe(false);
  });

  it('routes a dues bill to the dues pay endpoint then reloads', async () => {
    // act
    await store.pay(duesBill, card);

    // assert
    expect(api.payDuesItem).toHaveBeenCalledWith('dues-period', 'dues-item', card);
    expect(api.payUtilityItem).not.toHaveBeenCalled();
    expect(api.myBills).toHaveBeenCalled();
  });

  it('routes a utility bill to the utility pay endpoint', async () => {
    // act
    await store.pay(utilityBill, card);

    // assert
    expect(api.payUtilityItem).toHaveBeenCalledWith('util-period', 'util-item', card);
    expect(api.payDuesItem).not.toHaveBeenCalled();
  });
});
