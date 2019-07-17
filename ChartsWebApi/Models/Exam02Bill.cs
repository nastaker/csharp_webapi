using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Exam02Bill
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int SurveyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public short? Score { get; set; }
        public string Remark { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }
    }
}
