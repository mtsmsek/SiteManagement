import { Component, inject, input, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { ResidentsStore } from '../residents.store';
import { ContactFormDialog, ContactFormDialogData } from '../contact-form-dialog/contact-form-dialog';
import { VehicleFormDialog, VehicleFormDialogData } from '../vehicle-form-dialog/vehicle-form-dialog';
import { ConfirmDialog, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import type { Vehicle } from '../../../core/api/api.models';

/**
 * Resident detail page. Shows identity + contact (with an edit dialog) and the
 * resident's vehicles as a table with add/remove. residentId comes from the
 * route via component input binding.
 */
@Component({
  selector: 'app-resident-detail',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressBarModule,
    TranslatePipe,
  ],
  templateUrl: './resident-detail.html',
  styleUrl: './resident-detail.scss',
})
export class ResidentDetail implements OnInit {
  private readonly store = inject(ResidentsStore);
  private readonly dialog = inject(MatDialog);
  private readonly location = inject(Location);

  /** Bound from the :residentId route param (withComponentInputBinding). */
  readonly residentId = input.required<string>();

  readonly resident = this.store.detail;
  readonly loading = this.store.loading;

  readonly vehicleColumns = ['plate', 'note', 'actions'] as const;

  ngOnInit(): void {
    void this.store.loadDetail(this.residentId());
  }

  back(): void {
    this.location.back();
  }

  editContact(): void {
    const r = this.resident();
    if (!r) {
      return;
    }
    this.dialog.open<ContactFormDialog, ContactFormDialogData>(ContactFormDialog, {
      width: '480px',
      data: { residentId: this.residentId(), email: r.email, phone: r.phone },
    });
  }

  addVehicle(): void {
    this.dialog.open<VehicleFormDialog, VehicleFormDialogData>(VehicleFormDialog, {
      width: '420px',
      data: { residentId: this.residentId() },
    });
  }

  async removeVehicle(vehicle: Vehicle): Promise<void> {
    const confirmed = await firstValueFrom(
      this.dialog
        .open<ConfirmDialog, ConfirmDialogData, boolean>(ConfirmDialog, {
          data: { titleKey: 'residents.vehicles.remove', messageKey: 'residents.vehicles.removeMessage' },
        })
        .afterClosed(),
    );
    if (confirmed) {
      await this.store.removeVehicle(this.residentId(), vehicle.plate);
    }
  }
}
