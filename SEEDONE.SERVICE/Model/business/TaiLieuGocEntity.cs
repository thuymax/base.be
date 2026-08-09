using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// Tài liệu gốc
    /// <summary>
    [Table("tai_lieu_goc")]
    public class TaiLieuGocEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public Guid? tai_lieu_goc_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("stt")]
        [Require]
        [AutoId(Constants.AutoID.TaiLieuGoc)]
        public string? stt { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? ngay_nhan { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? ngay_gui { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? loai_giay_to { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? san_pham { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? ma_tra_cuu_online { get; set; }
        /// <summary>
        /// 
        /// <summary>
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
