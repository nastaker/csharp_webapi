using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class FunctionRel
    {
        public int Id { get; set; }
        public int? CreateBy { get; set; }
        public string CreateLogin { get; set; }
        public string CreateName { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public string FuncattrName { get; set; }
        public int Funcid { get; set; }
        public string ValueattrName { get; set; }
    }
}
