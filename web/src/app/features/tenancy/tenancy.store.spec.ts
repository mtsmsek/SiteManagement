import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { TenancyStore } from './tenancy.store';
import { TenancyApi } from './tenancy.api';
import { TenantType, type ApartmentOccupant } from '../../core/api/api.models';

describe('TenancyStore', () => {
  const siteId = 'site-1';
  const occupant: ApartmentOccupant = {
    assignmentId: 'a-1',
    apartmentId: 'apt-1',
    residentId: 'res-1',
    residentFullName: 'Ada Lovelace',
    tenantType: 'Owner',
    startDate: '2026-01-01',
  };

  let api: {
    occupantsForSite: ReturnType<typeof vi.fn>;
    assign: ReturnType<typeof vi.fn>;
    endAssignment: ReturnType<typeof vi.fn>;
  };
  let store: TenancyStore;

  beforeEach(() => {
    // arrange — a stub API returning the single occupant
    api = {
      occupantsForSite: vi.fn().mockReturnValue(of([occupant])),
      assign: vi.fn().mockReturnValue(of({ assignmentId: 'a-1' })),
      endAssignment: vi.fn().mockReturnValue(of(void 0)),
    };
    TestBed.configureTestingModule({
      providers: [TenancyStore, { provide: TenancyApi, useValue: api }],
    });
    store = TestBed.inject(TenancyStore);
  });

  it('loads occupants and indexes them by apartment id', async () => {
    // act
    await store.loadOccupants(siteId);

    // assert
    expect(api.occupantsForSite).toHaveBeenCalledWith(siteId);
    expect(store.occupants()).toEqual([occupant]);
    expect(store.occupantByApartmentId().get('apt-1')).toEqual(occupant);
    expect(store.loading()).toBe(false);
  });

  it('assigns a resident then reloads the site occupants', async () => {
    // act
    await store.assign(siteId, {
      apartmentId: 'apt-1',
      residentId: 'res-1',
      tenantType: TenantType.owner,
      startDate: '2026-01-01',
    });

    // assert
    expect(api.assign).toHaveBeenCalledOnce();
    expect(api.occupantsForSite).toHaveBeenCalledWith(siteId);
  });

  it('ends an assignment then reloads the site occupants', async () => {
    // act
    await store.endAssignment(siteId, 'a-1', '2026-02-01');

    // assert
    expect(api.endAssignment).toHaveBeenCalledWith('a-1', { endDate: '2026-02-01' });
    expect(api.occupantsForSite).toHaveBeenCalledWith(siteId);
  });
});
