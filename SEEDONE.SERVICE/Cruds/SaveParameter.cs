using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Cruds
{
    public class SaveParameter<TEntityDtoEdit, TEntity> : CrudParameter
    {
        /// <summary>
        /// Dữ liệu cất
        /// </summary>
        public TEntityDtoEdit Model { get; set; }
        /// <summary>
        /// Trả về dữ liệu sau khi cất
        /// </summary>
        public bool ReturnRecord { get; set; }
        /// <summary>
        /// Giá trị kháo chính của model
        /// Gán 1 lần ở beforeSaveAsync để trong service dugnf đỡ phải đọc lại
        /// </summary>
        [JsonIgnore]
        public object Id { get; set; }
        /// <summary>
        /// Dữ liệu cũ, dùng cho edit
        /// </summary>
        [JsonIgnore]
        public TEntityDtoEdit Old { get; set; }
        /// <summary>
        /// False: update autoid, True: k update autoid
        /// </summary>
        public bool IsNotUpdateAutoId { get; set; }
        /// <summary>
        /// Dugnf cho autoID khi 1 bảng master có nhiều chứng từ dùng chung
        /// </summary>
        //public RefTypeParam refTypes { get; set; }
    }
}
