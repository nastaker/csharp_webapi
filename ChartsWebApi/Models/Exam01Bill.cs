using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Exam01Bill
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Score { get; set; }
        public string Remark { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }
    }
}
