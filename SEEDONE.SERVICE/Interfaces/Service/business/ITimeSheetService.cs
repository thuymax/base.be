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
    public interface ITimeSheetService : ICrudBaseService<Guid, TimeSheetEntity, TimeSheetDtoEdit>
    {
        Task<DAResult> GetTimeline(FilterTable filterTable);
        Task<DAResult> GetCalculator(FilterTable filterTable);
        Task<DAResult> GetDataExport(FilterTable filterTable);
    }
}
