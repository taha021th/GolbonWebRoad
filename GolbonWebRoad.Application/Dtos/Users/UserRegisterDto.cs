using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Application.Dtos.Users
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "ایمیل الزامی است")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        public string Password { get; set; }
        [Required(ErrorMessage = "تکراری رمز عبور الزامی است")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن با هم مطابقت ندارند")]
        public string ConfirmPassword { get; set; }

    }

}
