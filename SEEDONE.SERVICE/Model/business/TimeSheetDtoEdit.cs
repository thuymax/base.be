using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Business
{
    public class TimeSheetDtoEdit : TimeSheetEntity, IRecordState, IRecordVersion
    {
        public ModelState state { get; set; }
        [Detail(nameof(TimeSheetTargetDtoEdit.time_sheet_id), typeof(TimeSheetTargetEntity))]
        public List<TimeSheetTargetDtoEdit> timeSheetTargets { get; set; } = new List<TimeSheetTargetDtoEdit>();
        public long RecordVersion { get; set; }

        public TimeSheetDtoEdit Clone()
        {
            return (TimeSheetDtoEdit)MemberwiseClone();
        }
    }
}
