import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  ResidentListItem,
  ResidentDetails,
  RegisterResidentRequest,
  RegisterResidentResponse,
  UpdateContactRequest,
  AddVehicleRequest,
} from '../../core/api/api.models';

/**
 * HTTP surface for the Residency bounded context (residents, vehicles).
 * Stateless — returns Observables; the ResidentsStore owns the resulting state.
 */
@Injectable({ providedIn: 'root' })
export class ResidentsApi {
  private readonly api = inject(ApiClient);

  list(): Observable<ResidentListItem[]> {
    return this.api.get<ResidentListItem[]>('/residents');
  }

  getById(residentId: string): Observable<ResidentDetails> {
    return this.api.get<ResidentDetails>(`/residents/${residentId}`);
  }

  register(body: RegisterResidentRequest): Observable<RegisterResidentResponse> {
    return this.api.post<RegisterResidentResponse>('/residents', body);
  }

  updateContact(residentId: string, body: UpdateContactRequest): Observable<void> {
    return this.api.put<void>(`/residents/${residentId}/contact`, body);
  }

  addVehicle(residentId: string, body: AddVehicleRequest): Observable<void> {
    return this.api.post<void>(`/residents/${residentId}/vehicles`, body);
  }

  removeVehicle(residentId: string, plate: string): Observable<void> {
    return this.api.delete<void>(`/residents/${residentId}/vehicles/${encodeURIComponent(plate)}`);
  }
}
