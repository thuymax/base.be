using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;

namespace SEEDONE.SERVICE.Interfaces.Repo.Business
{
    public interface ITimeSheetRepo : IBaseRepo
    {
        Task<DAResult> GetTimeline(Type type, FilterTable filterTable);
        Task<DAResult> GetCalculator(Type type, FilterTable filterTable);
        Task<DAResult> GetDataExport(Type type, FilterTable filterTable);
    }
}
