using System;

namespace SEEDONE.SERVICE.DtoEdit
{
    public class ResetPassword
    {
        /// <summary>
        /// checker_id
        /// </summary>
        public Guid checker_id { get; set; }

        /// <summary>
        /// password hiện tại
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// new_password
        /// </summary>
        public string new_password { get; set; }
    }
}