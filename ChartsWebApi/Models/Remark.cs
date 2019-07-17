using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Remark
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [JsonProperty("value")]
        public string Name { get; set; }
        [JsonIgnore]
        public double? ScoreMin { get; set; }
        [JsonIgnore]
        public double? ScoreMan { get; set; }
        [JsonIgnore]
        public DateTime DtCreate { get; set; }
        [JsonIgnore]
        public string Desc { get; set; }
        [JsonIgnore]
        public string SysNote { get; set; }
    }
}
