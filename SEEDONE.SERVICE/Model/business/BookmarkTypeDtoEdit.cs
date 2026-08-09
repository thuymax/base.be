using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    public class BookmarkTypeDtoEdit : BookmarkTypeEntity, IRecordState, IRecordVersion
    {
        public ModelState state { get; set; }
        [EditVersion]
        public long RecordVersion { get; set; }

        public BookmarkTypeDtoEdit Clone()
        {
            return (BookmarkTypeDtoEdit)MemberwiseClone();
        }
    }
}
