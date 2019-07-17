using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChartsWebApi.models
{
    public partial class Exam01Bom
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Part { get; set; }
        public string Item { get; set; }
        public decimal Score { get; set; }
        public int Order { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }

        [NotMapped]
        public int[] PictureIds { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool isMapped { get; set; }
    }
}
