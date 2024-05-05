using AutoMapper;
using Moq;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.XUnitTests.Application.Helpers;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons;

namespace SiteManagement.XUnitTests.Application.Mock.Repositories.Commons
{
    public abstract class BaseMockRepository<TRepository, TEntity, TMappingProfile, TBusinessRules, TFakeData>
        where TRepository : class, IAsyncRepository<TEntity>
        where TEntity : BaseEntity
        where TMappingProfile : Profile, new()
        where TBusinessRules : BaseBusinessRules
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
