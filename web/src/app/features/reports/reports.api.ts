import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type { AdminDashboard } from '../../core/api/api.models';

/** Admin reporting API surface (cross-cutting read projections). */
@Injectable({ providedIn: 'root' })
export class ReportsApi {
  private readonly api = inject(ApiClient);

  /** System-wide admin dashboard totals. */
  adminDashboard(): Observable<AdminDashboard> {
    return this.api.get<AdminDashboard>('/reports/dashboard');
  }
}
