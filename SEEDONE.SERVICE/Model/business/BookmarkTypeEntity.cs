using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// Loại hồ sơ
    /// <summary>
    [Table("bookmark_type")]
    public class BookmarkTypeEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid bookmark_type_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("Mã loại hồ sơ")]
        [Require]
        [AutoId(Constants.AutoID.BookmarkType)]
        public string bookmark_type_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? description { get; set; }
        /// <summary>
        /// kl quy đổi mã hồ sơ
        /// <summary>
        public decimal workload { get; set; }
        /// <summary>
        /// đánh dấu mã việc đc thu tiền về
        /// <summary>
        public bool x3 { get; set; }
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
        [IgnoreUpdate]
        public DateTime? edit_version { get; set; }
    }
}
