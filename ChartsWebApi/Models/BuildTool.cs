using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChartsWebApi.models
{
    public partial class BuildTool
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }

        [NotMapped]
        public int count { get; set; }
    }
}
