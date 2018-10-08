using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApiNCoreApplication1.Domain.Service
{
    public interface IService<Tv, Te>
    {
        IEnumerable<Tv> GetAll();
        int Add(Tv obj);
        bool Update(Tv obj);
        bool Remove(int id);
        Tv GetOne(int id);
        IEnumerable<Tv> Get(Expression<Func<Te, bool>> predicate);
    }
}
