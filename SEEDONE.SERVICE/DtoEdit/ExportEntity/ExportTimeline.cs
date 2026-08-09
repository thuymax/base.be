using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.DtoEdit.ExportEntity
{
    public class ExportTimeline
    {
        public string? checker { get; set; }
        public string? time_sheet_code { get; set; }
        public string? bookmark_type_code { get; set; }
        public string? task { get; set; }
        public string? target { get; set; }
        public string? deadline_customer_date { get; set; }
        public string? deadline_checker_date { get; set; }
        public string? warning { get; set; }
    }
}
