using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class ExamGroup
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public DateTime DtCreate { get; set; }
    }
}
