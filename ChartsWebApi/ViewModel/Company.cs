using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.ViewModel
{
    public class FormData
    {
        public string type { get; set; }
        public string parenttype { get; set; }
        public string parentguid { get; set; }
        public string CN_LOGIN { get; set; }
        public string CN_PSW { get; set; }
        public bool CN_LEADER { get; set; }
        public string CN_SEX { get; set; }
        public string CN_ENTRYDAY { get; set; }
        public string CN_USERCODE { get; set; }
        public string CN_PHONE { get; set; }
        public string CN_EMAIL { get; set; }
        public string CN_GUID { get; set; }
        public string CN_NAME { get; set; }
        public string CN_ORDER { get; set; }
        public string CN_ADMIN { get; set; }
        public string CN_DESC { get; set; }
    }
}
