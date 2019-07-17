using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Exam01Img
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Desc { get; set; }
        public DateTime DtCreate { get; set; }
    }
}
