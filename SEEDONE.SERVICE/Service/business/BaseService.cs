using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace SEEDONE.SERVICE.Service.Business
{
    public class BaseService<TEntity, TRepo> : IBaseService<TEntity>
        where TRepo : IBaseRepo
    {
        #region DECLARE
        protected TRepo _repo;
        protected ITypeService _typeService;
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Phương thức khởi tạo
        /// </summary>
        /// <param name="baseRepo"> Base repo</param>
        public BaseService(TRepo repo, IServiceProvider serviceProvider)
        {
            _repo = repo;
            _typeService = serviceProvider.GetRequiredService<ITypeService>();
        }

        #endregion

        public async Task<DAResult> GetDataTable(FilterTable filterTable)
        {
            return await _repo.GetDataTable(typeof(TEntity),filterTable);
        }
        public async Task<IList> GetComboboxPaging(string colums, string filter, string sort)
        {
            return await _repo.GetComboboxPaging(typeof(TEntity), colums, filter, sort);
        }

        

    }
}
