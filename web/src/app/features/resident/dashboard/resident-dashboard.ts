import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentApi } from '../resident.api';
import type { ResidentDashboard as ResidentDashboardData } from '../../../core/api/api.models';

/**
 * Resident portal landing. Read-only summary tiles — outstanding balance (and
 * how many unpaid items), any credit in the resident's favour, and unread
 * messages — each linking into the matching section. Data is token-scoped
 * server-side.
 */
@Component({
  selector: 'app-resident-dashboard',
  imports: [
    DecimalPipe,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    TranslatePipe,
  ],
  templateUrl: './resident-dashboard.html',
  styleUrl: './resident-dashboard.scss',
})
export class ResidentDashboard implements OnInit {
  private readonly api = inject(ResidentApi);

  /** The loaded summary, or null while loading. */
  readonly data = signal<ResidentDashboardData | null>(null);

  /** True while the summary is loading. */
  readonly loading = signal(true);

  ngOnInit(): void {
    this.api.myDashboard().subscribe((summary) => {
      this.data.set(summary);
      this.loading.set(false);
    });
  }
}
