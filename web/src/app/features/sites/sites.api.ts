import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/api/api-client';
import type {
  SiteListItem,
  SiteDetails,
  CreateSiteRequest,
  CreateSiteResponse,
  AddBlockRequest,
  AddBlockResponse,
  AddApartmentRequest,
  AddApartmentResponse,
} from '../../core/api/api.models';

/**
 * HTTP surface for the Property bounded context (sites, blocks, apartments).
 * Stateless — returns Observables; the SitesStore owns the resulting state.
 */
@Injectable({ providedIn: 'root' })
export class SitesApi {
  private readonly api = inject(ApiClient);

  list(): Observable<SiteListItem[]> {
    return this.api.get<SiteListItem[]>('/sites');
  }

  getById(siteId: string): Observable<SiteDetails> {
    return this.api.get<SiteDetails>(`/sites/${siteId}`);
  }

  create(body: CreateSiteRequest): Observable<CreateSiteResponse> {
    return this.api.post<CreateSiteResponse>('/sites', body);
  }

  remove(siteId: string): Observable<void> {
    return this.api.delete<void>(`/sites/${siteId}`);
  }

  addBlock(siteId: string, body: AddBlockRequest): Observable<AddBlockResponse> {
    return this.api.post<AddBlockResponse>(`/sites/${siteId}/blocks`, body);
  }

  addApartment(blockId: string, body: AddApartmentRequest): Observable<AddApartmentResponse> {
    return this.api.post<AddApartmentResponse>(`/blocks/${blockId}/apartments`, body);
  }

  occupyApartment(apartmentId: string): Observable<void> {
    return this.api.post<void>(`/apartments/${apartmentId}/occupy`);
  }

  vacateApartment(apartmentId: string): Observable<void> {
    return this.api.post<void>(`/apartments/${apartmentId}/vacate`);
  }
}
