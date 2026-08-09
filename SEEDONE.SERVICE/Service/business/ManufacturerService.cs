using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SEEDONE.SERVICE.Cruds;

namespace SEEDONE.SERVICE.Service.Business
{
    public class ManufacturerService : CrudBaseService<IManufacturerRepo, Guid, ManufacturerEntity, ManufacturerDtoEdit>, IManufacturerService
    {
        public ManufacturerService(IManufacturerRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }
    }
}
