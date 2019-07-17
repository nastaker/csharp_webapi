using System;
using System.Collections.Generic;

namespace ChartsWebApi.models
{
    public partial class Build
    {
        public int CnId { get; set; }
        public string CnCode { get; set; }
        public string CnName { get; set; }
        public string CnType { get; set; }
        public string CnStatus { get; set; }
        public DateTime CnDtCreate { get; set; }
        public string Cn3dFileName { get; set; }
        public string Cn3dFileDir { get; set; }
        public string CnDesc { get; set; }
        public string CnSysNote { get; set; }
    }
}
