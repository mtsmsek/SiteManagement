using Moq;
using SiteManagement.Application.Pagination.Responses;
using SiteManagement.Application.Services.Repositories.Commons;
using SiteManagement.Domain.Entities.Commons;
using System.Linq.Expressions;

namespace SiteManagement.XUnitTests.Application.Helpers
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
            SetUpGetSingleAsync(mockRepository, list);
            SetUpDeleteAsync(mockRepository, list);
            SetUpUpdateAsync(mockRepository, list);
            SetUpAnyAsync(mockRepository, list);
            SetUpGetByIdAsync(mockRepository, list);
        }

        private static void SetUpGetByIdAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
               .Setup(
               s =>
               s.GetByIdAsync(It.IsAny<Guid>(), 
               It.IsAny<bool>(),
               It.IsAny<CancellationToken>(), 
               It.IsAny<Expression<Func<TEntity, object>>[]>()))
               .ReturnsAsync((Guid id, bool noTracking, CancellationToken cancellationToken, Expression<Func<TEntity, object>>[] includes) =>
               {
                  

                   TEntity? entity =  entityList.FirstOrDefault(x => x.Id == id);
                   return entity;
               });
        }

        private static void SetUpAnyAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
                .Setup(
                s =>
                s.AnyAsync(It.IsAny<Expression<Func<TEntity,bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<TEntity,bool>> predicate ,bool noTracking, CancellationToken cancellationToken) =>
                {
                    //return true;

                    return entityList.Any(predicate.Compile());

                });
        }

        private static void SetUpUpdateAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> list)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
                .Setup(
                s =>
                s.UpdateAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TEntity entity, CancellationToken cancellationToken) =>
                {
                    entity.UpdatedDate = DateTime.Now;
                    return 1;
                });
        }

        private static void SetUpDeleteAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> list)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
                .Setup(
                s =>
                s.DeleteAsync(It.IsAny<TEntity>(),
                               It.IsAny<bool>(),
                               It.IsAny<CancellationToken>()))
                .ReturnsAsync((TEntity entity, bool permenant, CancellationToken cancellationToken) =>
                {
                    if (!permenant)
                        entity.DeletedDate = DateTime.Now;

                    else
                        list.Remove(entity);

                    return 1;
                }
                );
        }

        public static void SetUpGetSingleAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
          .Setup(
              s =>
                  s.GetSingleAsync(
                      It.IsAny<Expression<Func<TEntity, bool>>>(),
                      It.IsAny<bool>(),
                      It.IsAny<CancellationToken>(),
                      It.IsAny<Expression<Func<TEntity, object>>[]>()
                  )
          )!
          .ReturnsAsync(
                 (
                   Expression<Func<TEntity, bool>> predicate,
                   bool enableTracking,
                   CancellationToken cancellationToken,
                   Expression<Func<TEntity, object>>[] include
               ) =>
                 {

                     TEntity? entity = entityList.FirstOrDefault(predicate.Compile());

                     return entity;
                 }
                 );

        }

        private static void SetupAddAsync<TRepository, TEntity>(Mock<TRepository> mockRepository, List<TEntity> entityList)
            where TRepository : class, IAsyncRepository<TEntity>
            where TEntity : BaseEntity
        {
            mockRepository
                .Setup(r => r.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TEntity entity,CancellationToken cancellationToken) =>
                {
                    entity.Id = Guid.NewGuid();
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
                       It.IsAny<Expression<Func<TEntity, object>>[]>()
                   )
           )
           .ReturnsAsync(
               (
                   Expression<Func<TEntity, bool>> expression,
                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
                   int index,
                   int size,
                   bool enableTracking,
                   CancellationToken cancellationToken,
                   Expression<Func<TEntity, object>>[] include
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
