import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe, PercentPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslatePipe } from '@ngx-translate/core';
import { ReportsApi } from '../reports.api';
import type { AdminDashboard as AdminDashboardData } from '../../../core/api/api.models';

/**
 * Admin landing dashboard. Read-only system-wide totals: how many sites and
 * residents, money accrued vs collected (and the collection rate), what's still
 * outstanding, and the credit owed back to residents.
 */
@Component({
  selector: 'app-admin-dashboard',
  imports: [DecimalPipe, PercentPipe, MatCardModule, MatIconModule, MatProgressBarModule, TranslatePipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard implements OnInit {
  private readonly api = inject(ReportsApi);

  /** The loaded totals, or null while loading. */
  readonly data = signal<AdminDashboardData | null>(null);

  /** True while the totals are loading. */
  readonly loading = signal(true);

  ngOnInit(): void {
    this.api.adminDashboard().subscribe((summary) => {
      this.data.set(summary);
      this.loading.set(false);
    });
  }
}
