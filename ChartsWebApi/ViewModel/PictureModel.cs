using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.ViewModel
{
    public class PictureModel
    {
        public int ExamId { get; set; }
        public string Type { get; set; }
        public int PictureId { get; set; }
        public string FileName { get; set; }
    }
}
