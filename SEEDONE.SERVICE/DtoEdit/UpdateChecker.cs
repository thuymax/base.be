using System;

namespace SEEDONE.SERVICE.DtoEdit
{
    public class UpdateChecker
    {
        /// <summary>
        /// checker_id
        /// </summary>
        public Guid checker_id { get; set; }
        public string checker_code { get; set; }
        public string checker_name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }

    }
}