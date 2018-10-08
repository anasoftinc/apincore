using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApiNCoreApplication1.Domain.Service
{
    public interface IServiceAsync<Tv, Te>
    {
        Task<IEnumerable<Tv>> GetAll();
        Task<int> Add(Tv obj);
        Task<bool> Update(Tv obj);
        Task<bool> Remove(int id);
        Task<Tv> GetOne(int id);
        Task<IEnumerable<Tv>> Get(Expression<Func<Te, bool>> predicate);
    }

}
