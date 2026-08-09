using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    public class CustomerDtoEdit : CustomerEntity, IRecordState, IRecordVersion
    {
        [Description("Trạng thái")]
        public ModelState state { get; set; }
        public long RecordVersion { get; set; }

        public CustomerDtoEdit Clone()
        {
            return (CustomerDtoEdit)MemberwiseClone();
        }
    }
}
