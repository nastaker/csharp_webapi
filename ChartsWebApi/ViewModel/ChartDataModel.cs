using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.ViewModel
{
    public class ChartDataModel
    {
        public List<string> columns { get; set; }
        public List<JObject> rows { get; set; }
    }
}
