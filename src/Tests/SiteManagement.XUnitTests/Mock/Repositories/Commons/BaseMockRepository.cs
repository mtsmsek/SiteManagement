using AutoMapper;
using Moq;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.XUnitTests.Helpers;
using SiteManagement.XUnitTests.Mock.FakeDatas.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Mock.Repositories.Commons
{
    public abstract class BaseMockRepository<TRepository,TEntity,TMappingProfile,TBusinessRules,TFakeData> 
        where TRepository: class, IAsyncRepository<TEntity>
        where TEntity : BaseEntity
        where TMappingProfile : Profile, new()
        where TBusinessRules: BaseBusinessRules
        where TFakeData : BaseFakeData<TEntity>
    {
        public IMapper Mapper;
        public Mock<TRepository> MockRepository;
        public TBusinessRules BusinessRules;

        public BaseMockRepository(TFakeData fakeData)
        {
            MapperConfiguration mapperConfig =
                new(c =>
                {
                    c.AddProfile<TMappingProfile>();
                });
            Mapper = mapperConfig.CreateMapper();

            MockRepository = MockRepositoryHelper.GetRepository<TRepository, TEntity>(fakeData.Data);
            BusinessRules = (TBusinessRules)Activator.CreateInstance(typeof(TBusinessRules), MockRepository.Object)!;

        }

    }
}
