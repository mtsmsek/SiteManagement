﻿using MediatR;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Domain.Enumarations.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment
{
    public class CreateApartmentCommand : IRequest<CreateApartmentResponse>, ICacheRemoverRequest
    {
        public Guid BlockId { get; set; }
        public bool Status { get; set; }
        public ApartmentType ApartmentType { get; set; }
        public int ApartmentNumber { get; set; }
        public int FloorNumber { get; set; }
        public bool IsTenant { get; set; }

        public string? CacheKey => $"GetAllApartments";

        public bool BypassCache { get; }
    }
}
