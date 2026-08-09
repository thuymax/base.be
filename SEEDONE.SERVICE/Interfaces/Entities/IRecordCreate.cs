using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Interfaces.Entities
{
    public interface IRecordCreate
    {
        public DateTime? created_date { get; set; }
        public string created_by { get; set; }
    }
}
