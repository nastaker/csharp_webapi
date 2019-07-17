using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Exam02Tool
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int BillId { get; set; }
        public int ToolId { get; set; }
        public int Count { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
    }
}
