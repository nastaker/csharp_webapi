using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Build
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public bool IsEnable { get; set; }
        public DateTime DtCreate { get; set; }
        public string FaceImg { get; set; }
        public string MapImg { get; set; }
        public string MapImgSelected { get; set; }
        public string MapImgPositionLT { get; set; }
        public string MapImgPositionRB { get; set; }
        public string ModelName { get; set; }
        public string ModelSrc { get; set; }
        public string Desc { get; set; }
        public string SysNote { get; set; }
    }
}
