using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface ICustomerService : ICrudBaseService<Guid, CustomerEntity, CustomerDtoEdit>
    { 
    }
}
