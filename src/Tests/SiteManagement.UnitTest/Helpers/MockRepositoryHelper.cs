using Moq;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Commons;
using System.Linq.Expressions;

namespace SiteManagement.UnitTest.Helpers
{
    public static class MockRepositoryHelper
    {
        public static Mock<TRepository> GetRepository<TRepository, TEntity>(List<TEntity> list)
            where TEntity : BaseEntity
            where TRepository : class, IAsyncRepository<TEntity>
        {
            var mockRepository = new Mock<TRepository>();
            Build(mockRepository, list);
            return mockRepository;
        }

        private static void Build<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> list)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            SetupGetListAsync(mockRepository, list);
            SetupAddAsync(mockRepository, list);
        }

        private static void SetupAddAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (TEntity entity) =>
                    {
                        entityList.Add(entity);
                        return 1;
                    }
                );

        }

        private static void SetupGetListAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
           .Setup(
               s =>
                   s.GetListAsync(
                       It.IsAny<Expression<Func<TEntity, bool>>>(),
                       It.IsAny<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>(),
                       It.IsAny<int>(),
                       It.IsAny<int>(),
                       It.IsAny<bool>(),
                       It.IsAny<CancellationToken>(),
                       It.IsAny < Expression<Func<TEntity, object>>[]>()
                   )
           )
           .ReturnsAsync(
               (
                   Expression<Func<TEntity, bool>> expression,
                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
                   Expression<Func<TEntity, object>> include,
                   int index,
                   int size,
                   bool enableTracking,
                   CancellationToken cancellationToken
               ) =>
               {
                   IList<TEntity> list = new List<TEntity>();

                   if (expression == null)
                       list = entityList;
                   else
                       list = entityList.Where(expression.Compile()).ToList();

                   PagedViewModel<TEntity> paginateList = new() { Results = list };
                   return paginateList;
               }
           );
        }
    }
}
