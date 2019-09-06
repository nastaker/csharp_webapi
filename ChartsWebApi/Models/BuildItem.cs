using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChartsWebApi.models
{
    public partial class BuildItem
    {
        public int Id { get; set; }
        public int BuildId { get; set; }
        public int PartId { get; set; }
        public string Code { get; set; }
        public string PartName { get; set; }
        public string PartNameAlias1 { get; set; }
        public string PartNameAlias2 { get; set; }
        public string Name { get; set; }
        public string NameAlias1 { get; set; }
        public string NameAlias2 { get; set; }
        public string Features { get; set; }
        public string IsPart { get; set; }
        public string IsAssemble { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public DateTime DtCreate { get; set; }
        public string Type { get; set; }
        public string Desc { get; set; }
    }
}
