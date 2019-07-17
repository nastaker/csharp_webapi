using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChartsWebApi.models
{
    public partial class BuildSurveyGroup
    {
        public int Id { get; set; }
        public int BuildId { get; set; }
        public int SurveyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Desc { get; set; }
        public DateTime DtCreate { get; set; }
        
        [JsonIgnore]
        public List<BuildSurveyGroupTool> GroupTools { get; set; }
    }
}
