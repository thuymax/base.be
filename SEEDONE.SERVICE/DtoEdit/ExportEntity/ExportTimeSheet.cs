using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.DtoEdit.ExportEntity
{
    public class ExportTimeSheet
    {
        public string? checker_code { get; set; }
        public string? checker_name { get; set; }
        public string? time_sheet_code { get; set; }
        public string? task { get; set; }
        public string? bookmark_type_code { get; set; }
        public string? start_date_string { get; set; }
        public string? contract { get; set; }
        public string? customer { get; set; }
        public string? status { get; set; }
        public string? status_date_string { get; set; }
        public string? complete { get; set; }
        public string? plan { get; set; }
        public string? deadline_customer_date_string { get; set; }
        public string? detail { get; set; }
        public string? target { get; set; }
        public string? deadline_checker_date_string { get; set; }
    }
}
