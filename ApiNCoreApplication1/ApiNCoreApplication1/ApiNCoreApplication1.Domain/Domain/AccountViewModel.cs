using System;
using System.Collections.Generic;
using AutoMapper;

namespace ApiNCoreApplication1.Domain
{
    public class AccountViewModel : BaseDomain
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public bool IsTrial { get; set; }
        public bool IsActive { get; set; }
        public DateTime SetActive { get; set; }

        public virtual ICollection<UserViewModel> Users { get; set; }
    }

}


