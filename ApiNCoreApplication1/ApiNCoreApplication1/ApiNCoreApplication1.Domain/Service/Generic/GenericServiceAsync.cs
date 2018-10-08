using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ApiNCoreApplication1.Entity;
using ApiNCoreApplication1.Entity.UnitofWork;


namespace ApiNCoreApplication1.Domain.Service
{

    public class GenericServiceAsync<Tv, Te> : IServiceAsync<Tv, Te> where Tv : BaseDomain
                                      where Te : BaseEntity
    {
        protected IUnitOfWork _unitOfWork;
        public GenericServiceAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GenericServiceAsync()
        {
        }

        public virtual async Task<IEnumerable<Tv>> GetAll()
        {
            var entities = await _unitOfWork.GetRepositoryAsync<Te>()
                .GetAll();
            return Mapper.Map<IEnumerable<Tv>>(source: entities);
        }

        public virtual async Task<Tv> GetOne(int id)
        {
            var entity = await _unitOfWork.GetRepositoryAsync<Te>()
                .GetOne(predicate: x => x.Id == id);
            return Mapper.Map<Tv>(source: entity);
        }

        public virtual async Task<int> Add(Tv view)
        {
            var entity = Mapper.Map<Te>(source: view);
            await _unitOfWork.GetRepositoryAsync<Te>().Insert(entity);
            await _unitOfWork.SaveAsync();
            return entity.Id;
        }

        public async Task<bool> Update(Tv view)
        {
            await _unitOfWork.GetRepositoryAsync<Te>().Update(view.Id, Mapper.Map<Te>(source: view));
            return await _unitOfWork.SaveAsync();
        }

        public virtual async Task<bool> Remove(int id)
        {
            await _unitOfWork.GetRepositoryAsync<Te>().Delete(id);
            return await _unitOfWork.SaveAsync();
        }

        public virtual async Task<IEnumerable<Tv>> Get(Expression<Func<Te, bool>> predicate)
        {
            var items = await _unitOfWork.GetRepositoryAsync<Te>()
                .Get(predicate: predicate);
            return Mapper.Map<IEnumerable<Tv>>(source: items);
        }
    }


}
