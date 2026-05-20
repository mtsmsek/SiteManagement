import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ResidentsApi } from './residents.api';
import type {
  ResidentListItem,
  ResidentDetails,
  RegisterResidentRequest,
  UpdateContactRequest,
  AddVehicleRequest,
} from '../../core/api/api.models';

/**
 * Signal-based store for the residents feature. Mirrors SitesStore: holds the
 * list + open detail, exposes loading/empty, and re-fetches after mutations.
 */
@Injectable({ providedIn: 'root' })
export class ResidentsStore {
  private readonly api = inject(ResidentsApi);

  private readonly residentsSignal = signal<ResidentListItem[]>([]);
  private readonly detailSignal = signal<ResidentDetails | null>(null);
  private readonly loadingSignal = signal(false);

  readonly residents = this.residentsSignal.asReadonly();
  readonly detail = this.detailSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly isEmpty = computed(() => !this.loadingSignal() && this.residentsSignal().length === 0);

  async loadList(): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.residentsSignal.set(await firstValueFrom(this.api.list()));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  async loadDetail(residentId: string): Promise<void> {
    this.loadingSignal.set(true);
    try {
      this.detailSignal.set(await firstValueFrom(this.api.getById(residentId)));
    } finally {
      this.loadingSignal.set(false);
    }
  }

  async register(body: RegisterResidentRequest): Promise<void> {
    await firstValueFrom(this.api.register(body));
    await this.loadList();
  }

  async updateContact(residentId: string, body: UpdateContactRequest): Promise<void> {
    await firstValueFrom(this.api.updateContact(residentId, body));
    await this.loadDetail(residentId);
  }

  async addVehicle(residentId: string, body: AddVehicleRequest): Promise<void> {
    await firstValueFrom(this.api.addVehicle(residentId, body));
    await this.loadDetail(residentId);
  }

  async removeVehicle(residentId: string, plate: string): Promise<void> {
    await firstValueFrom(this.api.removeVehicle(residentId, plate));
    await this.loadDetail(residentId);
  }
}
