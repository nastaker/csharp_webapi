using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Function
    {
        public int Id { get; set; }
        public int BusinessClassid { get; set; }
        public string ChartBlock { get; set; }
        public string ChartLine { get; set; }
        public string ChartLineBlock { get; set; }
        public string ChartMap { get; set; }
        public string ChartPie { get; set; }
        public string Code { get; set; }
        public int? CreateBy { get; set; }
        public string CreateLogin { get; set; }
        public string CreateName { get; set; }
        public DateTime DtCreate { get; set; }
        public string Desc { get; set; }
        public int Dimension { get; set; }
        public int DomainClassid { get; set; }
        public int IconBlob { get; set; }
        public int Level1Classid { get; set; }
        public int Level2Classid { get; set; }
        public int MoudelClassid { get; set; }
        public string Name { get; set; }
        public string NameAbb { get; set; }
        public int SystemClassid { get; set; }
        public string Title { get; set; }
        public int ValueClassid { get; set; }
        public int ValueDigits { get; set; }
        public int ValueType { get; set; }
    }
}
