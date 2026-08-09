using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Model
{
    /// <summary>
    /// Bảng đánh mã tự động tăng
    /// <summary>
    [Table("auto_id")]
    public class AutoIdEntity
    {
        /// <summary>
        /// PK
        /// <summary>
        [Key]
        public int ref_type_category_id { get; set; }
        /// <summary>
        /// tên
        /// <summary>
        public string? ref_type_category_name { get; set; }
        /// <summary>
        /// te=iền tố
        /// <summary>
        public string? prefix { get; set; }
        /// <summary>
        /// giá trị
        /// <summary>
        public decimal? value { get; set; }
        /// <summary>
        /// độ dài
        /// <summary>
        public int length_of_value { get; set; }
        /// <summary>
        /// hậu tố
        /// <summary>
        public string? suffix { get; set; }
        /// <summary>
        /// thứu tự
        /// <summary>
        public int? sort_order { get; set; }
    }
}
