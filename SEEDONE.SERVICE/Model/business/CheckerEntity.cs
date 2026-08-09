using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// Người dùng
    /// <summary>
    [Table("checker")]
    public class CheckerEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid checker_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("Mã nhân viên")]
        [Require]
        public string checker_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string checker_name { get; set; }
        /// <summary>
        /// Khối lượng việc  được giao trong tháng
        /// <summary>
        public int ref_x1 { get; set; }
        /// <summary>
        /// Khối lượng việc đã đạt trong tháng
        /// <summary>
        public int ref_x2 { get; set; }
        /// <summary>
        /// Khối lượng việc phát sinh có tiền
        /// <summary>
        public int ref_x3 { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public Role role { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? email { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? phone_number { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? password { get; set; }
        public string? role_detail { get; set; }
        public bool is_block { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? created_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? created_by { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? modified_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? modified_by { get; set; }
    }
}
