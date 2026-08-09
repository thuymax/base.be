using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Cruds
{
    public class DeleteParameter<TKey, TEntity> : CrudParameter
    {
        public List<TEntity> Models { get; set; }

        public object CustomParam { get; set; }
        public DateTime? DeleteDate { get; set; } = null;
    }
    /// <summary>
    /// delete cho xóa nhiều
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDtoEdit"></typeparam>
    public class DeleteParameterForDto<TKey, TDtoEdit> : CrudParameter
    {
        public List<TDtoEdit> Models { get; set; }

    }
}
