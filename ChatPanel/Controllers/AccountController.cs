using Microsoft.AspNetCore.Mvc;

namespace ChatPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config) => _config = config;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var expectedUser = _config["AdminCredentials:Username"];
            var expectedPass = _config["AdminCredentials:Password"];
            var displayName = _config["AdminCredentials:DisplayName"];

            if (username == expectedUser && password == expectedPass)
            {
                HttpContext.Session.SetString("AdminUser", displayName);
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
