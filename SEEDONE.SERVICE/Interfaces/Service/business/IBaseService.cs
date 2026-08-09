using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    public interface IBaseService<TEntity>
    {

        /// <summary>
        /// Lấy dữ liệu bảng
        /// </summary>
        Task<DAResult> GetDataTable(FilterTable filterTable);
        /// <summary>
        /// Lấy dữ combobox
        /// </summary>
        Task<IList> GetComboboxPaging(string colums, string filter, string sort);

        

    }
}
