using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using ApiNCoreApplication1.Entity;
using ApiNCoreApplication1.Entity.UnitofWork;

namespace ApiNCoreApplication1.Domain.Service
{
    public class GenericService<Tv, Te> : IService<Tv, Te> where Tv : BaseDomain
                                      where Te : BaseEntity
    {

        protected IUnitOfWork _unitOfWork;
        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GenericService()
        {
        }

        public virtual IEnumerable<Tv> GetAll()
        {
            var entities = _unitOfWork.GetRepository<Te>()
            .GetAll();
            return Mapper.Map<IEnumerable<Tv>>(source: entities);
        }
        public virtual Tv GetOne(int id)
        {
            var entity = _unitOfWork.GetRepository<Te>()
                .GetOne(predicate: x => x.Id == id);
            return Mapper.Map<Tv>(source: entity);
        }

        public virtual int Add(Tv view)
        {
            var entity = Mapper.Map<Te>(source: view);
            _unitOfWork.GetRepository<Te>().Insert(entity);
            _unitOfWork.Save();
            return entity.Id;
        }

        public virtual bool Update(Tv view)
        {
            _unitOfWork.GetRepository<Te>().Update(view.Id, Mapper.Map<Te>(source: view));
            return _unitOfWork.Save();
        }


        public virtual bool Remove(int id)
        {
            _unitOfWork.GetRepository<Te>().Delete(id);
            return _unitOfWork.Save();
        }

        public virtual IEnumerable<Tv> Get(Expression<Func<Te, bool>> predicate)
        {
            var entities = _unitOfWork.GetRepository<Te>()
                .Get(predicate: predicate);
            return Mapper.Map<IEnumerable<Tv>>(source: entities);
        }
    }
}
