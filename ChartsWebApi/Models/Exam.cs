using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Exam
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int BuildId { get; set; }
        public string BuildCode { get; set; }
        public string BuildName { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string ExaminerType { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public decimal? Score { get; set; }
        public string Remark { get; set; }
        public DateTime? DtRemark { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }
    }
}
