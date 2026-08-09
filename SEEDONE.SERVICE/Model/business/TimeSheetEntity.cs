using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// Hồ sơ
    /// <summary>
    [Table("time_sheet")]
    public class TimeSheetEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid time_sheet_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("Mã hồ sơ")]
        [Require]
        [AutoId(Constants.AutoID.TimeSheet)]
        public string? time_sheet_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? task { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public Guid? bookmark_type_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public Guid? checker_id { get; set; }
        /// <summary>
        /// ngày bắt đầu công việc
        /// <summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// mã đơn hàng
        /// <summary>
        public string? contract { get; set; }
        /// <summary>
        /// mã khách hàng
        /// <summary>
        public string? customer { get; set; }
        /// <summary>
        /// trạng thái công việc(todo, doing, pending, done, complete, cancel)
        /// <summary>
        public string? status { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? status_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public bool? complete { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public bool? plan { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? deadline_customer_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? detail { get; set; }
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
        [EditVersion]
        public DateTime? modified_date { get; set; }
        public Guid? dossier_id { get; set; }
    }
}
