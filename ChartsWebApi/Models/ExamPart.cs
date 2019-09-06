using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.models
{
    public class ExamPart
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
    }
}
