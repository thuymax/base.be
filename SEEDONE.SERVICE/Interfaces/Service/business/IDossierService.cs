using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface IDossierService : ICrudBaseService<Guid, DossierEntity, DossierDtoEdit>
    {
        Task<DAResult> GetDataExport(FilterTable filterTable);
        Task<TimeSheetEntity> GetTimesheetByDossierID(Guid dossierId);
    }
}
