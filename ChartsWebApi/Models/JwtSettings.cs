using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsWebApi.Models
{
    public class JwtSettings
    {
        //token是谁颁发的
        public string Issuer { get; set; }
        //token可以给哪些客户端使用
        public string Audience { get; set; }
        //加密的key
        public string SecretKey { get; set; }
        //过期时间
        public int Expires { get; set; }
    }

    public class XJwtSettings
    {
        public long IssueId { get; set; }
        public string Secret { get; set; }
        public string AesKey { get; set; }
        public string Server { get; set; }
        public string ExamName { get; set; }
        public string UrlSubmitScore { get; set; }
        public string UserPad { get; set; }
    }
}
