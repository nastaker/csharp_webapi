using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChartsWebApi.models
{
    public partial class BuildSurveyGroupTool
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ToolId { get; set; }
        public string GroupName { get; set; }
        public string ToolName { get; set; }
        public DateTime DtCreate { get; set; }
        public string Img { get; set; }
        public string ImgRotate { get; set; }
        public string PositionLeftTop { get; set; }
        public string PositionRightBottom { get; set; }
        [NotMapped]
        public int Angle { get; set; }
        public int RightAngle { get; set; }

        [JsonIgnore]
        public BuildSurveyGroup Group { get; set; }
    }
}
