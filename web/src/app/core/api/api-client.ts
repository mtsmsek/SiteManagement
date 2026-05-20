import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

/**
 * Thin typed wrapper over HttpClient that prefixes the configured API base URL
 * and the /api segment. Feature-specific API services (SitesApi, ResidentsApi)
 * build on it so they only spell out the resource path and the request/response
 * types, which come from the OpenAPI-generated schema.
 */
@Injectable({ providedIn: 'root' })
export class ApiClient {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api`;

  get<T>(path: string): Observable<T> {
    return this.http.get<T>(this.baseUrl + path);
  }

  post<T>(path: string, body?: unknown): Observable<T> {
    return this.http.post<T>(this.baseUrl + path, body ?? {});
  }

  put<T>(path: string, body?: unknown): Observable<T> {
    return this.http.put<T>(this.baseUrl + path, body ?? {});
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(this.baseUrl + path);
  }
}
