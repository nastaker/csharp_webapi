using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.models
{
    public class OrgUser
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string Guid { get; set; }
        [JsonIgnore]
        public string Login { get; set; }
        [JsonIgnore]
        public string Username { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Type { get; set; }
        [JsonIgnore]
        public string Status { get; set; }
        [JsonIgnore]
        public string ParGuid { get; set; }
    }
}
