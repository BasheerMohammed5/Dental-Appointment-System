using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DentalAppointmentSystem.Controllers
{
	public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Login(LoginRequest model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// تحقق من صحة بيانات المستخدم
			if (model.Email == "Basheer" && model.Password == "123456")
			{
				string userRole = "Admin"; // افترض أن الدور هو "Admin" بشكل افتراضي

				// يمكنك تحديد الدور بناءً على البريد الإلكتروني أو غيره من المنطق الخاص بك
				if (model.Email == "employee@example.com")
				{
					userRole = "Employee";
				}

				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, model.Email),
			new Claim(ClaimTypes.Role, userRole)
		};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
					issuer: _configuration["JwtSettings:Issuer"],
					audience: _configuration["JwtSettings:Audience"],
					claims: claims,
					expires: DateTime.Now.AddHours(1), // توكن صالح لمدة ساعة
					signingCredentials: creds);

				var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

				// تخزين التوكن في TempData لاستخدامه في الـ View
				TempData["JwtToken"] = tokenString;

                // توجيه المستخدم بناءً على دوره

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                //if (userRole == "Admin")
                //{
                //                TempData["JwtToken"] = tokenString;
                //                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                //}
                //else if (userRole == "Employee")
                //{
                //	return RedirectToAction("Index", "EmployeeDashboard", new { area = "Employee" });
                //}
            }

			// إذا كانت بيانات الدخول غير صحيحة
			ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			return View(model);
		}


		public async Task<IActionResult> Logout()
        {
            // تسجيل الخروج
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

		private readonly IConfiguration _configuration;

		public AccountController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
	}
}
