import { components } from './api-types';

/**
 * Friendly aliases for the OpenAPI-generated schema types. The generator emits
 * everything under components['schemas'][...]; re-exporting here keeps feature
 * code free of that indirection and gives one place to spot drift when the
 * schema is regenerated via `npm run gen:api`.
 */
type Schemas = components['schemas'];

// Property (sites / blocks / apartments)
export type SiteListItem = Schemas['SiteListItemDto'];
export type SiteDetails = Schemas['SiteDetailsDto'];
export type BlockSummary = Schemas['BlockSummaryDto'];
export type ApartmentSummary = Schemas['ApartmentSummaryDto'];
export type CreateSiteRequest = Schemas['CreateSiteRequest'];
export type CreateSiteResponse = Schemas['CreateSiteResponse'];
export type AddBlockRequest = Schemas['AddBlockRequest'];
export type AddBlockResponse = Schemas['AddBlockResponse'];
export type AddApartmentRequest = Schemas['AddApartmentRequest'];
export type AddApartmentResponse = Schemas['AddApartmentResponse'];

// Residency (residents / vehicles)
export type ResidentListItem = Schemas['ResidentListItemDto'];
export type ResidentDetails = Schemas['ResidentDetailsDto'];
export type Vehicle = Schemas['VehicleDto'];
export type RegisterResidentRequest = Schemas['RegisterResidentRequest'];
export type RegisterResidentResponse = Schemas['RegisterResidentResponse'];
export type UpdateContactRequest = Schemas['UpdateContactRequest'];
export type AddVehicleRequest = Schemas['AddVehicleRequest'];

// Shared
export type ProblemDetails = Schemas['ProblemDetails'];

/** Apartment occupancy status values as the API serialises them. */
export const ApartmentStatus = {
  empty: 'Empty',
  occupied: 'Occupied',
} as const;

/**
 * Apartment type is the Turkish "rooms+living" notation (e.g. "2+1"), parsed
 * as a free string by the domain — not an enum. These are the common layouts
 * offered as a picklist; the field still accepts any valid "N+M" value.
 */
export const ApartmentTypeOptions = ['1+0', '1+1', '2+1', '3+1', '4+1', '4+2', '5+1'] as const;

/** Validates the "N+M" apartment-type shape client-side (N>=1, M>=0). */
export const APARTMENT_TYPE_PATTERN = /^\d+\+\d+$/;
