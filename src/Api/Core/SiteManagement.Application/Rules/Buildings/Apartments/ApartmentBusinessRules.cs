﻿using SiteManagement.Application.CrossCuttingConcerns.Exceptions.Types;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Domain.Constants.Buildings.Apartments;
using SiteManagement.Domain.Constants.Buildings.Blocks;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Utulity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Application.Rules.Buildings.Apartments
{
    public class ApartmentBusinessRules : BaseBusinessRules
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IBlockRepository _blockRepository;

        public ApartmentBusinessRules(IApartmentRepository apartmentRepository, IBlockRepository blockRepository)
        {
            _apartmentRepository = apartmentRepository;
            _blockRepository = blockRepository;
        }
        
        public async Task ApartmentNumberCannotBeDuplicateForSameBlock(Guid blockId, int apartmentNumber, CancellationToken cancellationToken = default)
        {

            var apartmentsInBLock = await _apartmentRepository.GetListAsync(predicate: predicate => predicate.BlockId == blockId,
                                                                            includes: a => a.Block);

            bool isApartmentNumberExist =  apartmentsInBLock.Results.Select(apartment => apartment.ApartmentNumber)
                                                                    .Contains(apartmentNumber);

            if (isApartmentNumberExist)
            {
                throw new BusinessException(ApartmentMessages.RuleMessages.ApartmentNumberCannotDuplicateForSameBlock);
            }


        }
        public async Task<Apartment> ApartmentShouldExistInDatabase(Guid apartmentId,CancellationToken cancellationToken = default)
        {
            var apartment = await _apartmentRepository.GetSingleAsync(apartment => apartment.Id == apartmentId);

            Ensure.NotNull(apartment, new BusinessException(ApartmentMessages.RuleMessages.ApartmentCannotBeFound));

            return apartment;
            


        }


    }
}
