import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  ApartmentOccupant,
  AssignResidentRequest,
  AssignResidentResponse,
  EndAssignmentRequest,
} from '../../core/api/api.models';

/**
 * HTTP surface for the Tenancy bounded context (resident assignments).
 * Stateless — returns Observables; the TenancyStore owns the resulting state.
 */
@Injectable({ providedIn: 'root' })
export class TenancyApi {
  private readonly api = inject(ApiClient);

  occupantsForSite(siteId: string): Observable<ApartmentOccupant[]> {
    return this.api.get<ApartmentOccupant[]>(`/assignments/sites/${siteId}/occupants`);
  }

  assign(body: AssignResidentRequest): Observable<AssignResidentResponse> {
    return this.api.post<AssignResidentResponse>('/assignments', body);
  }

  endAssignment(assignmentId: string, body: EndAssignmentRequest): Observable<void> {
    return this.api.post<void>(`/assignments/${assignmentId}/end`, body);
  }
}
