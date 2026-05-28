import { Component, inject, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import { MyBillsStore } from '../my-bills.store';
import {
  CardPaymentDialog,
  CardPaymentDialogData,
} from '../../billing/card-payment-dialog/card-payment-dialog';
import { BillingItemStatus, BillKind, type PayByCardRequest, type ResidentBill } from '../../../core/api/api.models';

/**
 * Resident's own bills page. Lists every dues/utility line the resident owes,
 * shows the outstanding total, and lets them pay an unpaid item by card (the
 * shared card dialog). All data is token-scoped server-side — this page can
 * only ever show the signed-in resident's bills.
 */
@Component({
  selector: 'app-my-bills',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressBarModule,
    DecimalPipe,
    TranslatePipe,
    EmptyState,
  ],
  templateUrl: './my-bills.html',
  styleUrl: './my-bills.scss',
})
export class MyBills implements OnInit {
  private readonly store = inject(MyBillsStore);
  private readonly dialog = inject(MatDialog);

  readonly bills = this.store.bills;
  readonly loading = this.store.loading;
  readonly totalOutstanding = this.store.totalOutstanding;
  readonly unpaid = BillingItemStatus.unpaid;
  readonly columns = ['month', 'detail', 'amount', 'status', 'actions'] as const;

  ngOnInit(): void {
    void this.store.load();
  }

  /** i18n label for a bill's kind/detail: "Aidat" for dues, the utility type otherwise. */
  detailKey(bill: ResidentBill): string {
    return bill.kind === BillKind.dues ? 'resident.bills.dues' : `billing.utilityType.${bill.detail}`;
  }

  /** Opens the card dialog for an unpaid bill and pays it on confirm. */
  async pay(bill: ResidentBill): Promise<void> {
    const card = await this.collectCard(bill.amount);
    if (card) {
      await this.store.pay(bill, card);
    }
  }

  private collectCard(amount: number | string): Promise<PayByCardRequest | undefined> {
    return firstValueFrom(
      this.dialog
        .open<CardPaymentDialog, CardPaymentDialogData, PayByCardRequest>(CardPaymentDialog, {
          width: '420px',
          data: { amount },
        })
        .afterClosed(),
    );
  }
}
