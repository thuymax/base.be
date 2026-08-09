using SEEDONE.SERVICE.Interfaces.Repo.Business;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Properties;
using Dapper;
using System.Collections;

namespace SEEDONE.REPO.Repo.Business
{
    public class TaiLieuGocRepo : BaseRepo, ITaiLieuGocRepo
    {
        public TaiLieuGocRepo(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
        }
        public override async Task<DAResult> GetDataTable(Type type, FilterTable filterTable)
        {
            var table = this.GetTableName(type);
            var columnSql = this.ParseColumn(string.Join(",", filterTable.fields), "ts");

            var param = new Dictionary<string, object>();
            var where = this.ParseWhere(filterTable.filter, param, "a");

            IDbConnection cnn = null;
            IList result = null;
            int totalRecord = 0;
            try
            {
                cnn = this.Provider.GetOpenConnection();

                var sb = new StringBuilder($"SELECT * FROM (SELECT {columnSql}, ts.created_Date" +
                    $", DATE_FORMAT(ts.`ngay_nhan`, '%Y-%m-%d') AS ngay_nhan_string" +
                    $", DATE_FORMAT(ts.`ngay_gui`, '%Y-%m-%d') AS ngay_gui_string" +
                    $" FROM {table}  ts ) a");
                var sqlSummary = new StringBuilder($"SELECT COUNT(*) FROM ({sb.ToString()}");

                if (!string.IsNullOrWhiteSpace(where))
                {
                    sb.Append($" WHERE {where}");
                    sqlSummary.Append($" WHERE {where}");
                }
                sqlSummary.Append(" ) AS SA");


                // Sắp xếp
                if (filterTable.sortBy?.Count > 0 && filterTable.sortType?.Count > 0)
                {
                    sb.Append($" ORDER BY ");
                    for (int i = 0; i < filterTable.sortBy.Count; i++)
                    {
                        sb.Append($" {filterTable.sortBy[i]} {filterTable.sortType[i]}");
                        if (i != filterTable.sortBy.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                }
                else
                {
                    if (this.GetTableColumns(type).Any(n => n.Equals("created_date")))
                    {
                        sb.Append($" ORDER BY a.created_date DESC");
                    }
                }

                if (filterTable.page > 0 && filterTable.size > 0)
                {
                    sb.Append($" LIMIT {filterTable.size} OFFSET {(filterTable.page - 1) * filterTable.size}");
                }

                result = await this.Provider.QueryAsync(cnn, sb.ToString(), param);
                totalRecord = await cnn.ExecuteScalarAsync<int>(sqlSummary.ToString(), param);
            }
            finally
            {
                this.Provider.CloseConnection(cnn);
            }

            return new DAResult(200, Resources.getDataSuccess, "", result, totalRecord);
        }


    }
}
