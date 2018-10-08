using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using ApiNCoreApplication1.Entity;
using ApiNCoreApplication1.Entity.UnitofWork;

namespace ApiNCoreApplication1.Domain.Service
{

    public class UserService<Tv, Te> : GenericService<Tv, Te>
                                                where Tv : UserViewModel
                                                where Te : User
    {
        //DI must be implemented in specific service as well beside GenericService constructor
        public UserService(IUnitOfWork unitOfWork)
        {
            if (_unitOfWork == null)
                _unitOfWork = unitOfWork;
        }

        //add any custom service method or override generic service method
        //...test, it can be removed
        public bool DoNothing()
        {
            return true;
        }
    }

    public class UserServiceAsync<Tv, Te> : GenericServiceAsync<Tv, Te>
                                                where Tv : UserViewModel
                                                where Te : User
    {
        //DI must be implemented specific service as well beside GenericAsyncService constructor
        public UserServiceAsync(IUnitOfWork unitOfWork)
        {
            if (_unitOfWork == null)
                _unitOfWork = unitOfWork;
        }

        //add any custom service method or override genericasync service method
        //...
    }

}
