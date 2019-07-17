using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class BuildSurveyGroupTool
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ToolId { get; set; }
        public int Count { get; set; }
        public string GroupName { get; set; }
        public string ToolName { get; set; }
        public DateTime DtCreate { get; set; }

        [JsonIgnore]
        public BuildSurveyGroup Group { get; set; }
    }
}
