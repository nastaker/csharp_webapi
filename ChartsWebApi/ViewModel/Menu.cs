using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChartsWebApi.ViewModel
{
    public class Menu
    {
        public string icon { get; set; }
        [JsonProperty(PropertyName = "icon-alt")]
        public string iconAlt { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public bool model { get; set; }
        public List<Menu> children { get; set; }
    }
}
