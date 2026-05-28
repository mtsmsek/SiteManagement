import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { SitesStore } from '../sites.store';
import { SiteFormDialog } from '../site-form-dialog/site-form-dialog';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../../shared/empty-state/empty-state';
import type { SiteListItem } from '../../../core/api/api.models';

/**
 * Admin sites list. A responsive card grid over the SitesStore's signal, with
 * create (dialog), delete (confirm dialog), and card navigation to the detail
 * page.
 */
@Component({
  selector: 'app-site-list',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslatePipe,
    EmptyState,
  ],
  templateUrl: './site-list.html',
  styleUrl: './site-list.scss',
})
export class SiteList implements OnInit {
  private readonly store = inject(SitesStore);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  readonly sites = this.store.sites;
  readonly loading = this.store.loading;
  readonly isEmpty = this.store.isEmpty;

  ngOnInit(): void {
    void this.store.loadList();
  }

  openCreate(): void {
    this.dialog.open(SiteFormDialog, { width: '480px' });
  }

  open(site: SiteListItem): void {
    void this.router.navigate(['/admin/sites', site.id]);
  }

  async confirmDelete(site: SiteListItem, event: MouseEvent): Promise<void> {
    event.stopPropagation();
    const confirmed = await firstValueFrom(
      this.dialog
        .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, {
          data: { titleKey: 'sites.delete.title', messageKey: 'sites.delete.message' },
        })
        .afterClosed(),
    );
    if (confirmed) {
      await this.store.remove(site.id);
    }
  }
}
