using System.ComponentModel;
using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// khách hàng
    /// <summary>
    [Table("customer")]
    public class CustomerEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid customer_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        [Unique("stt")]
        [AutoId(Constants.AutoID.Customer)]
        public string? stt { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? cong_ty { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? ten_viet_tat { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? dia_chi { get; set; }
        /// <summary>
        /// 
        /// <summary>
        /// 
        [ColumnName("Thông tin nhân sự")]
        public string? thong_tin_nhan_su { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? chuyen_phat { get; set; }
        /// <summary>
        /// 
        /// <summary>
        /// 
        [IgnoreExport]
        public string? ghi_chu { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? created_by { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? created_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? modified_by { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? modified_date { get; set; }
    }
}
