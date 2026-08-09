using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Cruds
{
    public class CrudParameter
    {
        /// <summary>
        /// Danh sách các rule valdiate bỏ qua
        /// </summary>
        public List<string>? Ignore { get; set; }

        /// <summary>
        /// Kiểm tra có bỏ qua kiểm tra rule validate này không
        /// </summary>
        /// <param name="code">mã rule</param>
        public virtual bool CheckIgnore(string code)
        {
            if (this.Ignore != null && !string.IsNullOrEmpty(code))
            {
                foreach (var item in this.Ignore)
                {
                    if (code.Equals(item, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
