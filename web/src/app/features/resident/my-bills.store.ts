import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ResidentApi } from './resident.api';
import { BillingItemStatus, BillKind, type PayByCardRequest, type ResidentBill } from '../../core/api/api.models';

/**
 * Signal-based store for the current resident's own bills. Loads the
 * token-scoped list and pays a single item by card (routing to the dues or
 * utility endpoint by the bill's kind), then refreshes so the paid line and the
 * outstanding total stay consistent.
 */
@Injectable({ providedIn: 'root' })
export class MyBillsStore {
  private readonly api = inject(ResidentApi);

  private readonly billsSignal = signal<ResidentBill[]>([]);
  private readonly loadingSignal = signal(false);

  /** The resident's bills, most recent month first (as the API returns them). */
  readonly bills = this.billsSignal.asReadonly();

  /** True while a request is in flight. */
  readonly loading = this.loadingSignal.asReadonly();

  /** Sum of the still-unpaid bills (amounts may arrive as strings — coerced). */
  readonly totalOutstanding = computed(() =>
    this.billsSignal()
      .filter((b) => b.status === BillingItemStatus.unpaid)
      .reduce((sum, b) => sum + Number(b.amount), 0),
  );

  /** Loads the resident's own bills. */
  async load(): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.billsSignal.set(await firstValueFrom(this.api.myBills()));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  /** Pays one bill by card (dues or utility, by kind), then reloads the list. */
  async pay(bill: ResidentBill, card: PayByCardRequest): Promise<void> {
    if (bill.kind === BillKind.dues) {
      await firstValueFrom(this.api.payDuesItem(bill.periodId, bill.itemId, card));
    } else {
      await firstValueFrom(this.api.payUtilityItem(bill.periodId, bill.itemId, card));
    }

    await this.load();
  }
}
