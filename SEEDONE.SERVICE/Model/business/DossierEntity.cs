using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    /// <summary>
    /// Hồ sơ
    /// <summary>
    [Table("dossier")]
    public class DossierEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public Guid dossier_id { get; set; }
        /// <summary>
        /// 
        /// <summary>
        [Unique("dossier_code")]
        [Require]
        [AutoId(Constants.AutoID.Dossier)]
        public string? dossier_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? contract_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public Guid? bookmark_type_id { get; set; }
        public Guid? checker_id { get; set; }
        public string? description { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? customer { get; set; }
        public string? dosage_form { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? product_name { get; set; }
        
        /// <summary>
        /// 
        /// <summary>
        public string? api { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? manufacturer { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? applicant { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? submission_code { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public DateTime? submission_date { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? tt1 { get; set; }
        public string? tt2 { get; set; }
        public string? tt3 { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? visa_no { get; set; }
        /// <summary>
        /// 
        /// <summary>
        public string? note { get; set; }
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
