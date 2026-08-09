using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    public class DossierDtoEdit : DossierEntity, IRecordState, IRecordVersion
    {
        public ModelState state { get; set; }
        public long RecordVersion { get; set; }

        public DossierDtoEdit Clone()
        {
            return (DossierDtoEdit)MemberwiseClone();
        }
    }
}
