using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// 
    /// <summary>
    [Table("time_sheet_target")]
    public class TimeSheetTargetEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid time_sheet_target_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public Guid time_sheet_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public DateTime? deadline_checker_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Require]
        public string? target { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public int? sort_order { get; set; }
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
