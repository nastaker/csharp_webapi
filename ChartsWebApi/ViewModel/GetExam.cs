using ChartsWebApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.ViewModel
{
    public class GetExam
    {
        public string userguid { get; set; }
        public string Type { get; set; }
        public List<OrgUser> GroupUsers { get; set; }
    }

    public class PartInfo
    {
        public string part { get; set; }
        public string item { get; set; }
        public string desc { get; set; }
        public string desc1 { get; set; }
        public string desc2 { get; set; }
        public int[] picture { get; set; }
        public decimal score { get; set; }
        public int order { get; set; }
    }

    public class ExamBomTable
    {
        public int ExamId { get; set; }
        public List<PartInfo> boms { get; set; }
    }

    public class ToolsInfo
    {
        public int ExamId { get; set; }
        public List<BuildTool> tools { get; set; }
        public Exam02Bill bill { get; set; }
    }
}
