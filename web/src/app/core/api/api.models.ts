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

// Tenancy (assignments / occupants)
export type ApartmentOccupant = Schemas['ApartmentOccupantDto'];
export type ResidentAssignment = Schemas['ResidentAssignmentDto'];
export type AssignResidentRequest = Schemas['AssignResidentRequest'];
export type AssignResidentResponse = Schemas['AssignResidentResult'];
export type EndAssignmentRequest = Schemas['EndAssignmentRequest'];

// Billing (dues / utility periods + items)
export type DuesPeriodListItem = Schemas['DuesPeriodListItemDto'];
export type UtilityBillPeriodListItem = Schemas['UtilityBillPeriodListItemDto'];
export type PeriodItem = Schemas['PeriodItemDto'];
export type SiteDebtSummary = Schemas['SiteDebtSummaryDto'];
export type OpenDuesPeriodRequest = Schemas['OpenDuesPeriodRequest'];
export type OpenDuesPeriodResponse = Schemas['OpenDuesPeriodResult'];
export type OpenUtilityBillRequest = Schemas['OpenUtilityBillRequest'];
export type OpenUtilityBillResponse = Schemas['OpenUtilityBillPeriodResult'];
export type PayByCardRequest = Schemas['PayByCardRequest'];
export type ChangeDuesAmountRequest = Schemas['ChangeDuesAmountRequest'];
export type ChangeUtilityBillAmountRequest = Schemas['ChangeUtilityBillAmountRequest'];

// Resident self-service (own bills)
export type ResidentBill = Schemas['ResidentBillDto'];

// Shared
export type ProblemDetails = Schemas['ProblemDetails'];

/** A resident bill's kind discriminator, as the API serialises it. */
export const BillKind = {
  dues: 'Dues',
  utility: 'Utility',
} as const;

/**
 * Tenant type as the API binds it on assignment requests — an integer enum
 * (Owner=0, Tenant=1). The read DTOs serialise it back as the name string.
 */
export const TenantType = {
  owner: 0,
  tenant: 1,
} as const;

/** Picklist of tenant types with i18n label keys for the assignment dialog. */
export const TenantTypeOptions = [
  { value: TenantType.owner, labelKey: 'tenancy.tenantType.owner' },
  { value: TenantType.tenant, labelKey: 'tenancy.tenantType.tenant' },
] as const;

/**
 * Utility type as the API binds it on utility-bill requests — an integer enum
 * (Electricity=0, Water=1, NaturalGas=2). Read DTOs serialise it as the name.
 */
export const UtilityType = {
  electricity: 0,
  water: 1,
  naturalGas: 2,
} as const;

/**
 * Picklist of utility types with i18n label keys for the bill dialog. The key
 * suffixes match the API enum names (Electricity/Water/NaturalGas) so the same
 * keys translate both the dialog options and the period header labels.
 */
export const UtilityTypeOptions = [
  { value: UtilityType.electricity, labelKey: 'billing.utilityType.Electricity' },
  { value: UtilityType.water, labelKey: 'billing.utilityType.Water' },
  { value: UtilityType.naturalGas, labelKey: 'billing.utilityType.NaturalGas' },
] as const;

/** Billing item payment-status values as the API serialises them. */
export const BillingItemStatus = {
  unpaid: 'Unpaid',
  paid: 'Paid',
} as const;

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
