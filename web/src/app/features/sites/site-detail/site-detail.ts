import { Component, inject, input, OnInit, signal } from '@angular/core';
import { DecimalPipe, Location } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { TenancyStore } from '../../tenancy/tenancy.store';
import { BillingStore } from '../../billing/billing.store';
import { BlockFormDialog, BlockFormDialogData } from '../block-form-dialog/block-form-dialog';
import {
  ApartmentFormDialog,
  ApartmentFormDialogData,
} from '../apartment-form-dialog/apartment-form-dialog';
import {
  AssignResidentDialog,
  AssignResidentDialogData,
} from '../../tenancy/assign-resident-dialog/assign-resident-dialog';
import {
  DuesPeriodFormDialog,
  DuesPeriodFormDialogData,
} from '../../billing/dues-period-form-dialog/dues-period-form-dialog';
import {
  UtilityBillFormDialog,
  UtilityBillFormDialogData,
} from '../../billing/utility-bill-form-dialog/utility-bill-form-dialog';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import {
  ApartmentStatus,
  BillingItemStatus,
  type ApartmentSummary,
  type BlockSummary,
  type DuesPeriodListItem,
  type PeriodItem,
  type UtilityBillPeriodListItem,
} from '../../../core/api/api.models';

/**
 * Site detail page. Shows the site header, its blocks/apartments, who occupies
 * each apartment (with assign / move-out actions), and the site's billing:
 * dues and utility periods that the admin can open, distribute and close, plus
 * a debt summary card. The siteId comes from the route via input binding.
 */
@Component({
  selector: 'app-site-detail',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatExpansionModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTabsModule,
    MatCardModule,
    DecimalPipe,
    TranslatePipe,
  ],
  templateUrl: './site-detail.html',
  styleUrl: './site-detail.scss',
})
export class SiteDetail implements OnInit {
  private readonly store = inject(SitesStore);
  private readonly tenancy = inject(TenancyStore);
  private readonly billing = inject(BillingStore);
  private readonly dialog = inject(MatDialog);
  private readonly location = inject(Location);

  /** Bound from the :siteId route param (withComponentInputBinding). */
  readonly siteId = input.required<string>();

  readonly site = this.store.detail;
  readonly loading = this.store.loading;
  readonly occupied = ApartmentStatus.occupied;

  readonly occupantByApartmentId = this.tenancy.occupantByApartmentId;
  readonly duesPeriods = this.billing.duesPeriods;
  readonly utilityPeriods = this.billing.utilityPeriods;
  readonly debtSummary = this.billing.debtSummary;

  readonly apartmentColumns = ['number', 'floor', 'type', 'status', 'occupant', 'actions'] as const;
  readonly itemColumns = ['apartment', 'resident', 'amount', 'status', 'actions'] as const;
  readonly unpaid = BillingItemStatus.unpaid;

  // Which period cards have their item table revealed (loaded lazily on first open).
  private readonly expandedDuesIds = signal(new Set<string>());
  private readonly expandedUtilityIds = signal(new Set<string>());

  ngOnInit(): void {
    void this.store.loadDetail(this.siteId());
    void this.tenancy.loadOccupants(this.siteId());
    void this.billing.loadForSite(this.siteId());
  }

  back(): void {
    this.location.back();
  }

  addBlock(): void {
    this.dialog.open<BlockFormDialog, BlockFormDialogData>(BlockFormDialog, {
      width: '420px',
      data: { siteId: this.siteId() },
    });
  }

  addApartment(block: BlockSummary): void {
    this.dialog.open<ApartmentFormDialog, ApartmentFormDialogData>(ApartmentFormDialog, {
      width: '420px',
      data: { siteId: this.siteId(), blockId: block.id },
    });
  }

  /** Opens the assign dialog; on success the apartment occupancy changed, so refresh the detail. */
  assignResident(apartment: ApartmentSummary): void {
    const ref = this.dialog.open<AssignResidentDialog, AssignResidentDialogData, boolean>(
      AssignResidentDialog,
      { width: '420px', data: { siteId: this.siteId(), apartmentId: apartment.id } },
    );
    ref.afterClosed().subscribe((assigned) => {
      if (assigned) {
        void this.store.loadDetail(this.siteId());
      }
    });
  }

  /** Confirms then ends the active assignment, vacating the apartment. */
  async vacate(apartment: ApartmentSummary): Promise<void> {
    const occupant = this.occupantByApartmentId().get(apartment.id);
    if (!occupant) {
      return;
    }

    const confirmed = await firstValueFrom(
      this.dialog
        .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, {
          data: {
            titleKey: 'tenancy.vacate.title',
            messageKey: 'tenancy.vacate.message',
            confirmKey: 'tenancy.vacate.confirm',
          },
        })
        .afterClosed(),
    );

    if (confirmed) {
      const today = new Date().toISOString().slice(0, 10);
      await this.tenancy.endAssignment(this.siteId(), occupant.assignmentId, today);
      await this.store.loadDetail(this.siteId());
    }
  }

  openDuesPeriod(): void {
    this.dialog.open<DuesPeriodFormDialog, DuesPeriodFormDialogData>(DuesPeriodFormDialog, {
      width: '420px',
      data: { siteId: this.siteId() },
    });
  }

  openUtilityPeriod(): void {
    this.dialog.open<UtilityBillFormDialog, UtilityBillFormDialogData>(UtilityBillFormDialog, {
      width: '420px',
      data: { siteId: this.siteId() },
    });
  }

  duesItems(period: DuesPeriodListItem) {
    return this.billing.duesItems(period.id);
  }

  utilityItems(period: UtilityBillPeriodListItem) {
    return this.billing.utilityItems(period.id);
  }

  /** True once a period has been distributed into items. */
  hasItems(period: DuesPeriodListItem | UtilityBillPeriodListItem): boolean {
    return Number(period.itemCount) > 0;
  }

  /** Paid-to-total ratio of a period as a 0-100 percentage for the progress bar. */
  paidRatio(paid: number | string, total: number | string): number {
    const totalCount = Number(total);
    return totalCount > 0 ? (Number(paid) / totalCount) * 100 : 0;
  }

  isDuesExpanded(period: DuesPeriodListItem): boolean {
    return this.expandedDuesIds().has(period.id);
  }

  isUtilityExpanded(period: UtilityBillPeriodListItem): boolean {
    return this.expandedUtilityIds().has(period.id);
  }

  /** Reveals/hides a dues period's item table, loading the items the first time. */
  toggleDuesItems(period: DuesPeriodListItem): void {
    this.expandedDuesIds.update((ids) => {
      const next = new Set(ids);
      if (next.has(period.id)) {
        next.delete(period.id);
      } else {
        next.add(period.id);
        void this.billing.loadDuesItems(period.id);
      }
      return next;
    });
  }

  /** Reveals/hides a utility period's item table, loading the items the first time. */
  toggleUtilityItems(period: UtilityBillPeriodListItem): void {
    this.expandedUtilityIds.update((ids) => {
      const next = new Set(ids);
      if (next.has(period.id)) {
        next.delete(period.id);
      } else {
        next.add(period.id);
        void this.billing.loadUtilityItems(period.id);
      }
      return next;
    });
  }

  /** Distributes a dues period and reveals the resulting items right away. */
  async distributeDues(period: DuesPeriodListItem): Promise<void> {
    await this.billing.distributeDues(this.siteId(), period.id);
    this.expandedDuesIds.update((ids) => new Set(ids).add(period.id));
  }

  closeDues(period: DuesPeriodListItem): void {
    void this.billing.closeDues(this.siteId(), period.id);
  }

  /** Distributes a utility period and reveals the resulting items right away. */
  async distributeUtility(period: UtilityBillPeriodListItem): Promise<void> {
    await this.billing.distributeUtility(this.siteId(), period.id);
    this.expandedUtilityIds.update((ids) => new Set(ids).add(period.id));
  }

  closeUtility(period: UtilityBillPeriodListItem): void {
    void this.billing.closeUtility(this.siteId(), period.id);
  }

  payDuesItem(period: DuesPeriodListItem, item: PeriodItem): void {
    void this.billing.payDuesItem(this.siteId(), period.id, item.itemId);
  }

  payUtilityItem(period: UtilityBillPeriodListItem, item: PeriodItem): void {
    void this.billing.payUtilityItem(this.siteId(), period.id, item.itemId);
  }
}
