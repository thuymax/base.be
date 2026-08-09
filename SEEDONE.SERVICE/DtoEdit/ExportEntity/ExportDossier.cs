using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.DtoEdit.ExportEntity
{
    public class ExportDossier
    {
        public string? dossier_code { get; set; }
        public string? checker_code { get; set; }
        public string? contract_code { get; set; }
        public string? bookmark_type_code { get; set; }
        public string? description { get; set; }
        public string? customer { get; set; }
        public string? dosage_form { get; set; }
        public string? product_name { get; set; }
        public string? api { get; set; }
        public string? manufacturer { get; set; }
        public string? applicant { get; set; }
        public string? submission_code { get; set; }
        public string? submission_date_string { get; set; }
        public string? tt1 { get; set; }
        public string? tt2 { get; set; }
        public string? tt3 { get; set; }
        public string? visa_no { get; set; }
        public string? note { get; set; }
    }
}
