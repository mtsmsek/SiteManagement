import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentsStore } from '../residents.store';
import { ResidentFormDialog } from '../resident-form-dialog/resident-form-dialog';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import type { ResidentListItem } from '../../../core/api/api.models';

/**
 * Admin residents list. mat-table over the ResidentsStore signal, with a
 * register dialog and row navigation to the detail page. No delete here —
 * the API exposes no resident deletion (residents are deactivated, not
 * removed, in a later milestone).
 */
@Component({
  selector: 'app-resident-list',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslatePipe,
    EmptyState,
  ],
  templateUrl: './resident-list.html',
  styleUrl: './resident-list.scss',
})
export class ResidentList implements OnInit {
  private readonly store = inject(ResidentsStore);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  readonly residents = this.store.residents;
  readonly loading = this.store.loading;
  readonly isEmpty = this.store.isEmpty;

  readonly columns = ['tcNo', 'name', 'email', 'phone', 'vehicles'] as const;

  ngOnInit(): void {
    void this.store.loadList();
  }

  openCreate(): void {
    this.dialog.open(ResidentFormDialog, { width: '520px' });
  }

  open(resident: ResidentListItem): void {
    void this.router.navigate(['/admin/residents', resident.id]);
  }
}
