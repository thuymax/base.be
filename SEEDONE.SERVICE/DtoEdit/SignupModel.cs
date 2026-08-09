using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.DtoEdit
{
    public class SignupModel
    {
        /// <summary>
        /// mã người dùng
        /// <summary>
        public string code { get; set; }
        /// <summary>
        /// Tên người dùng
        /// <summary>
        public string name { get; set; }

        /// <summary>
        /// Địa chỉ email của người dùng
        /// <summary>
        public string email { get; set; }

        /// <summary>
        /// Mật khẩu đăng nhập
        /// <summary>
        public string password { get; set; }

        /// <summary>
        /// Số điện thoại người dùng
        /// <summary>
        public string phone { get; set; }
    }
}
