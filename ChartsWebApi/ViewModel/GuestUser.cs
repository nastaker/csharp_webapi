using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.ViewModel
{
    public class GuestUser
    {
        public string Token { get; set; }
        public string Login { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class XlibUser
    {
        public int id { get; set; }
        public string un { get; set; }
        public string dis { get; set; }
    }

    public class XlibUserScore
    {
        public string username { get; set; }
        public string projectTitle { get; set; }
        public string childProjectTitle { get; set; }
        public int status { get; set; }
        public int score { get; set; }
        public long startDate { get; set; }
        public long endDate { get; set; }
        public int timeUsed { get; set; }
        public long issuerId { get; set; }
    }
}
