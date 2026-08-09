using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface ITaiLieuGocService : ICrudBaseService<Guid, TaiLieuGocEntity, TaiLieuGocDtoEdit>
    { 
    }
}
