using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// nhà chế tạo
    /// <summary>
    [Table("manufacturer")]
    public class ManufacturerEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid manufacturer_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("stt")]
        [Require]
        [AutoId(Constants.AutoID.Manufacturer)]
        public string? stt { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? cong_ty { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? dia_chi { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? thong_tin_nhan_su { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? chuyen_phat { get; set; }
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
