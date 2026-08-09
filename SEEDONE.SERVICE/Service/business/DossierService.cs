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
    public class DossierService : CrudBaseService<IDossierRepo, Guid, DossierEntity, DossierDtoEdit>, IDossierService
    {
        public DossierService(IDossierRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }

        public async Task<DAResult> GetDataExport(FilterTable filterTable)
        {
            return await _repo.GetDataExport(typeof(DossierEntity), filterTable);
        }

        public async Task<TimeSheetEntity> GetTimesheetByDossierID(Guid dossierId)
        {
            var data = await _repo.GetAsync<TimeSheetEntity>(nameof(TimeSheetEntity.dossier_id), dossierId);
            return data?.FirstOrDefault();
        }
    }
}
