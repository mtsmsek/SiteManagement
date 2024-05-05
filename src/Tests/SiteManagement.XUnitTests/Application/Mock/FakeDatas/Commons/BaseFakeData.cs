using SiteManagement.Domain.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.Mock.FakeDatas.Commons
{
    public abstract class BaseFakeData<TEntity>
        where TEntity : BaseEntity
    {
        public List<TEntity> Data => CreateFakeData();
        public abstract List<TEntity> CreateFakeData();
    }
}
