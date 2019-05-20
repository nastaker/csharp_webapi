using System.ComponentModel.DataAnnotations;

namespace ChartsWebApi.ViewModel
{
    public class LoginViewModel
    {
        //用户名
        [Required]
        public string Username { get; set; }
        //密码
        [Required]
        public string Password { get; set; }
    }
}
