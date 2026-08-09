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
using SEEDONE.SERVICE.DtoEdit.ExportEntity;

namespace SEEDONE.SERVICE.Service.Business
{
    public class TimeSheetService : CrudBaseService<ITimeSheetRepo, Guid, TimeSheetEntity, TimeSheetDtoEdit>, ITimeSheetService
    {
        public TimeSheetService(ITimeSheetRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }
        

        public async Task<DAResult> GetTimeline(FilterTable filterTable)
        {
            return await _repo.GetTimeline(typeof(TimeSheetEntity), filterTable);
        }
        public async Task<DAResult> GetCalculator(FilterTable filterTable)
        {
            return await _repo.GetCalculator(typeof(TimeSheetEntity), filterTable);
        }

        protected override async Task<TimeSheetDtoEdit> GetEditAsync(IDbConnection cnn, Guid id)
        {
            var model = await base.GetEditAsync(cnn, id);
            if (model.timeSheetTargets?.Count > 0)
            {
                model.timeSheetTargets = model.timeSheetTargets.OrderBy(x => x.sort_order).ThenBy(x => x.deadline_checker_date).ToList();
            }

            return model;
        }

        public async Task<DAResult> GetDataExport(FilterTable filterTable)
        {
            return await _repo.GetDataExport(typeof(TimeSheetEntity), filterTable);
        }
    }
}
