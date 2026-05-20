import { Component, inject, input, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { BlockFormDialog, BlockFormDialogData } from '../block-form-dialog/block-form-dialog';
import {
  ApartmentFormDialog,
  ApartmentFormDialogData,
} from '../apartment-form-dialog/apartment-form-dialog';
import { ApartmentStatus, type ApartmentSummary, type BlockSummary } from '../../../core/api/api.models';

/**
 * Site detail page. Shows the site header, its blocks as expansion panels, and
 * each block's apartments as a table with an occupy/vacate toggle. The siteId
 * comes from the route via component input binding.
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
    TranslatePipe,
  ],
  templateUrl: './site-detail.html',
  styleUrl: './site-detail.scss',
})
export class SiteDetail implements OnInit {
  private readonly store = inject(SitesStore);
  private readonly dialog = inject(MatDialog);
  private readonly location = inject(Location);

  /** Bound from the :siteId route param (withComponentInputBinding). */
  readonly siteId = input.required<string>();

  readonly site = this.store.detail;
  readonly loading = this.store.loading;
  readonly occupied = ApartmentStatus.occupied;

  readonly apartmentColumns = ['number', 'floor', 'type', 'status', 'actions'] as const;

  ngOnInit(): void {
    void this.store.loadDetail(this.siteId());
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

  async toggleOccupancy(apartment: ApartmentSummary): Promise<void> {
    const makeOccupied = apartment.status !== this.occupied;
    await this.store.setApartmentOccupancy(this.siteId(), apartment.id, makeOccupied);
  }
}
